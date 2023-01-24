#define IS_FLIPPER

#define DEBUG
#define DEBUG_2

#region debugStages debug define hierarchy
#if DEBUG_2
#if !DEBUG
    #define DEBUG
#endif
#endif
#endregion

// ReSharper disable once CheckNamespace

using System.Diagnostics;
using System.Text;
using Mono.Options;

namespace BisUtils.Generated.Generator; 

internal class GrammarBaker {

    public const string ANTLR_VERSION = "4.10.1";
    public const string PROJECT_PREFIX = "BisUtils.Generated";

    public const bool GENERATE_VISITOR = true;
    public const bool GENERATE_LISTENER = true;

    public const string ANTLR_JAR_NAME = $"antlr-{ANTLR_VERSION}-complete.jar";
    public const string ANTLR_DOWNLOAD_URL = $"https://www.antlr.org/download/{ANTLR_JAR_NAME}";

    private string _javaLocation;
    public string JavaLocation => !string.IsNullOrEmpty(_javaLocation) ? _javaLocation : "java.exe";
    
    public short CleaningDepth = 0;

    private DirectoryInfo? _projectsDirectory;

    public DirectoryInfo ProjectsDirectory {
        get {
            if (_projectsDirectory is null /*&& Debugger.IsAttached*/)
                _projectsDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent!.Parent!.Parent!.Parent!;

            return _projectsDirectory;
        }
    }


    private string _antlrLocation;
    public string AntlrLocation {
        get {
            var path = !string.IsNullOrEmpty(_antlrLocation) ? _antlrLocation : Path.Combine(Environment.CurrentDirectory, ANTLR_JAR_NAME);
            if (File.Exists(path)) return path;
            var downloadAntlrTask = DownloadAntlr(path);
            downloadAntlrTask.Wait();
            Console.WriteLine("Download complete!");
            return path;
        }
    }
    

    public GrammarBaker(string[] arguments) {
        if(arguments.Length == 0) {
#if IS_FLIPPER
            arguments = new[] {
                "--javaLocation=\"C:\\Program Files\\Java\\jdk-18.0.1\\bin\\java.exe\"",
                $"--antlrLocation=\"C:\\Users\\developer\\Documents\\BisUtils\\{ANTLR_JAR_NAME}\"",
                "--c", "--c"
            };
#endif

        }

        if (arguments.Length != 0) {
            var bakeOptions = new OptionSet() {
                {
                    "javaLocation|jl=",
                    "The location of java.exe used for baking",
                    value => {
                        value = value.TrimStart('"').TrimEnd('"');
                        if (!File.Exists(value)) throw new Exception("Defined java executable not found");
                        _javaLocation = value;
                    }
                }, 
                {
                    "antlrLocation|al=",
                    $"The location of antlr used for baking (will be downloaded from {ANTLR_DOWNLOAD_URL} if not found.",
                    value => {
                        value = value.TrimStart('"').TrimEnd('"');
                        _antlrLocation = value;
                    }
                },
                {
                    "projectNest|pn=",
                    $"The location of CS Projects with the correct BisUtils Grammar structure.",
                    value => {
                        value = value.TrimStart('"').TrimEnd('"');

                        var dir = new DirectoryInfo(value);
                        if (!dir.Exists) throw new Exception("Project nest does not exist!");
                        _projectsDirectory = dir;
                    }
                },
                {
                    "c|clean",
                    "Increase cleaning depth after baking (default 0; max 3)",
                    value => {
                        ++CleaningDepth;
                    }
                }
            };
            bakeOptions.Parse(arguments);
        }
        
#if DEBUG
        Console.WriteLine($" * GrammarBaker is starting with the following options.:");
        Console.WriteLine($" * CleaningDepth={CleaningDepth}");
        Console.WriteLine($" * ProjectNest={ProjectsDirectory}");
        Console.WriteLine($" * AntlrLocation={AntlrLocation}");
        Console.WriteLine($" * AntlrLocation={JavaLocation}");
#endif

        foreach (var csProj in ProjectsDirectory.GetFiles($"{PROJECT_PREFIX}*.csproj", SearchOption.AllDirectories)) {
            Console.WriteLine($"Located project {csProj.FullName}");
            Console.WriteLine($"Starting baking process on {csProj.Name}");
            BakeProject(csProj.Directory!);

        }
        
    }

    private void BakeProject(DirectoryInfo project) {
        DirectoryInfo? parserDir = null, lexerDir = null, utilitiesDir = null;
        foreach (var directory in project.GetDirectories()) {
            switch (directory.Name) {
                case "Parser":
#if DEBUG
                    Console.WriteLine($" * Located Parser folder for project {project.Name}");
                    Console.WriteLine($" * {directory.FullName}");
#endif
                    parserDir = directory;
                    break;
                case "Lexer":
#if DEBUG
                    Console.WriteLine($" * Located Lexer folder for project {project.Name}");
                    Console.WriteLine($" * {directory.FullName}");
#endif
                    lexerDir = directory;
                    break;
                case "Utilities":
#if DEBUG
                    Console.WriteLine($" * Located Utilities folder for project {project.Name}");
                    Console.WriteLine($" * {directory.FullName}");
#endif
                    utilitiesDir = directory;
                    break;
            }
        }

        if (parserDir is null || lexerDir is null || utilitiesDir is null)
            throw new Exception("Invalid project structure.");

        var lexerParserMap = new Dictionary<string, KeyValuePair<FileInfo, FileInfo>>();
        foreach (var lexerGrammar in lexerDir.EnumerateFiles("*.g4", SearchOption.TopDirectoryOnly)) {
            var grammarName = lexerGrammar.Name[..^8];  //remove ".g4" extension and "lexer" from name
#if DEBUG
            Console.WriteLine($" * Located lexer {lexerGrammar.FullName}");
            Console.WriteLine($" * Assumed Grammar Name is {grammarName}");
            Console.WriteLine($" * Attempting to locate parser for {grammarName}");
#endif
            var parser = LocateParser(grammarName, parserDir);
#if DEBUG
            Console.WriteLine($" * Located Parser {parser.FullName}");
#endif
            lexerParserMap.Add(grammarName, new KeyValuePair<FileInfo, FileInfo>(lexerGrammar, parser));
        }
        
        if(lexerParserMap.Count == 0) return;
        DirectoryInfo generatedUtilitiesDir = new(Path.Combine(utilitiesDir.FullName, "Generated")), 
            generatedParsersDir = new(Path.Combine(parserDir.FullName, "Generated")),
            generatedLexersDir = new(Path.Combine(lexerDir.FullName, "Generated"));
        
        if(!generatedUtilitiesDir.Exists) generatedUtilitiesDir.Create();
        if(!generatedParsersDir.Exists) generatedParsersDir.Create();
        if(!generatedLexersDir.Exists) generatedLexersDir.Create();

        
        foreach (var lexerParserPair in lexerParserMap) {
            Console.WriteLine($"Processing {lexerParserPair.Key} grammars. ({lexerParserPair.Value.Key.Name} and {lexerParserPair.Value.Value.Name})");

            DirectoryInfo outFolder;
            try {
                outFolder = ProcessGrammarSet(lexerParserPair.Key, lexerParserPair.Value);
            } catch (Exception e) {
                Console.WriteLine("Continuing...");
                Console.WriteLine();
#if DEBUG
                Console.WriteLine($" * An exception ({nameof(e)}) occured while processing the lexerParserPair. ({lexerParserPair.Value.Key.Name} and {lexerParserPair.Value.Value.Name})");
                Console.WriteLine($" * {e}");

#endif

                continue;
            }
            
            var ctx_utilitiesDir = new DirectoryInfo(Path.Combine(generatedUtilitiesDir.FullName, lexerParserPair.Key));
            var ctx_parserDir = new DirectoryInfo(Path.Combine(generatedParsersDir.FullName, lexerParserPair.Key));
            var ctx_lexerDir = new DirectoryInfo(Path.Combine(generatedLexersDir.FullName, lexerParserPair.Key));

            if(ctx_utilitiesDir.Exists) ctx_utilitiesDir.Delete();
            if(ctx_parserDir.Exists) ctx_parserDir.Delete();
            if(ctx_lexerDir.Exists) ctx_lexerDir.Delete();
            ctx_utilitiesDir.Create(); ctx_parserDir.Create(); ctx_lexerDir.Create();
            
            foreach (var generatedFile in outFolder.EnumerateFiles()) {
                var fileName = Path.GetFileNameWithoutExtension(generatedFile.Name);
                switch (generatedFile.Extension) {
                    case ".cs": {
                        if (fileName.EndsWith("Listener") || fileName.EndsWith("Visitor")) {
                            generatedFile.MoveTo(Path.Combine(ctx_utilitiesDir.FullName, generatedFile.Name));
                            continue;
                        }
                        
                        if (fileName.EndsWith("Lexer")) {
                            generatedFile.MoveTo(Path.Combine(ctx_lexerDir.FullName, generatedFile.Name));
                            continue;
                        }

                        if (fileName.EndsWith("Parser")) {
                            generatedFile.MoveTo(Path.Combine(ctx_parserDir.FullName, generatedFile.Name));
                            continue;
                        }
                        
                        throw new Exception($"Found unknown generated CS file {fileName}");
                    }
                    case ".tokens" or ".interp": {
                        if (fileName.EndsWith("Lexer")) {
                            generatedFile.MoveTo(Path.Combine(ctx_lexerDir.FullName, generatedFile.Name));
                            continue;
                        }

                        if (fileName.EndsWith("Parser")) {
                            generatedFile.MoveTo(Path.Combine(ctx_parserDir.FullName, generatedFile.Name));
                            continue;
                        }
                        
                        throw new Exception($"Found unknown generated {generatedFile.Extension} file {fileName}");

                    }
                    default: throw new Exception($"Found unknown generated {generatedFile.Extension} file {fileName}");
                }
            }

            if (CleaningDepth >= 1) {
#if DEBUG
                Console.WriteLine($" * (CleanInstall>=1) Deleting temp folder for grammar {lexerParserPair.Key} {{Current Cleaning Depth Is Set To {CleaningDepth}}}");
#endif
                outFolder.Delete();
            }
        }
    }

    private DirectoryInfo ProcessGrammarSet(string grammarName, KeyValuePair<FileInfo, FileInfo> lexerParserPair) {
        var lexerGrammar = lexerParserPair.Key;
        var parserGrammar = lexerParserPair.Value;
        var tempDirectory = new DirectoryInfo(Path.Combine(lexerGrammar.Directory!.FullName, "temp"));
        var ctxTempDirectory = new DirectoryInfo(Path.Combine(tempDirectory.FullName, grammarName));
        if (!tempDirectory.Exists) tempDirectory.Create();
        
        if (ctxTempDirectory.Exists) ctxTempDirectory.Delete();
        ctxTempDirectory.Create();
        
        ProcessGrammar(lexerGrammar, ctxTempDirectory, ctxTempDirectory, null);
        ProcessGrammar(parserGrammar, ctxTempDirectory, ctxTempDirectory, lexerGrammar);

        return ctxTempDirectory;
    }

    private void ProcessGrammar(FileInfo mainGrammar, DirectoryInfo outputDirectory, DirectoryInfo libraryDirectory, FileInfo? lexerGrammar) {
        var processingParser = lexerGrammar is not null;
#if DEBUG
        Console.WriteLine($" * Processing {(processingParser ? "Parser" : "Lexer")} Grammar: {mainGrammar.FullName}");
        Console.WriteLine($" * Processing {(processingParser ? "Parser" : "Lexer")} Grammar: {mainGrammar}");

#endif
        var antlrProcess = new Process();
        antlrProcess.EnableRaisingEvents = false;
        antlrProcess.StartInfo.FileName = JavaLocation;
#if DEBUG_2
        Console.WriteLine($" * Using Java From: {antlrProcess.StartInfo.FileName}");
        Console.WriteLine($" * Using Antlr From: {AntlrLocation}");

#endif

        var argumentsBuilder = new StringBuilder("-jar ").Append(AntlrLocation).Append(' ');
        argumentsBuilder.Append("-o \"").Append(outputDirectory).Append("\" ");
        if (GENERATE_VISITOR) argumentsBuilder.Append("-visitor ");
        if (GENERATE_LISTENER) argumentsBuilder.Append("-listener ");
        argumentsBuilder.Append("-lib \"").Append(libraryDirectory).Append("\" ");
        argumentsBuilder.Append('"').Append(mainGrammar.FullName).Append("\" ");
        if (processingParser) argumentsBuilder.Append('"').Append(lexerGrammar!.FullName).Append('"');
        antlrProcess.StartInfo.Arguments = argumentsBuilder.ToString();

#if DEBUG
        Console.WriteLine($" * Arguments built for {mainGrammar.FullName}");
        Console.WriteLine($" * Executing antlr4 with following arguments (Antlr Version {ANTLR_VERSION}");
        Console.WriteLine($" * {antlrProcess.StartInfo.Arguments}");
#endif
        
        try {
            if(!antlrProcess.Start()) throw new Exception("System.Diagnostics.Progress::Start returned false.");
        } catch (Exception e) {
            Console.WriteLine($"An exception ({nameof(e)}) occured while processing the {(processingParser ? "parser" : "lexer")} grammar for {mainGrammar.Name}");
#if DEBUG
            Console.WriteLine($" * OutputDirectory: {outputDirectory.FullName}");
            Console.WriteLine($" * LibraryDirectory: {outputDirectory.FullName}");
            Console.WriteLine($" * EffectedGrammar: {mainGrammar.FullName}");
            Console.WriteLine(e);
#endif
            throw;
        }

    }

    private static FileInfo LocateParser(string grammarName, DirectoryInfo parserDirectory) {
        foreach (var grammarFile in parserDirectory.GetFiles("*.g4")) {
            if (grammarFile.Name.StartsWith(grammarName)) return grammarFile;
        }

        throw new Exception($"Failed to locate parser for {grammarName}");
    }
    
    private static async Task DownloadAntlr(string downloadPath) {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(ANTLR_DOWNLOAD_URL, HttpCompletionOption.ResponseHeadersRead);
        var total = response.Content.Headers.ContentLength;
        var initialCursorLeft = Console.CursorLeft;
        var initialCursorTop = Console.CursorTop;

        using (var fileStream = File.OpenWrite(downloadPath)) {
            using (var httpStream = await response.Content.ReadAsStreamAsync()) {
                var buffer = new byte[4096];
                int read;
                var totalRead = 0L;
                while ((read = await httpStream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                    await fileStream.WriteAsync(buffer, 0, read);
                    totalRead += read;
                    Console.SetCursorPosition(initialCursorLeft, initialCursorTop);
                    Console.Write("{0}% [{1}]", (int)(totalRead * 100 / total), new string('#', (int)(totalRead * 50 / total)));
                }
            }
        }
        Console.WriteLine();
    }
}