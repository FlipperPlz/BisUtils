namespace BisUtils.RvProcess.Utils;

using System.Text;
using FResults;
using FResults.Reasoning;
using Models.Directives;

public delegate IEnumerable<IReason> RVIncludeFinder(IRvIncludeDirective include, ref string includeContents);

