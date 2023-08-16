namespace BisUtils.Core.IO;

public static class RVPathUtilities
{
    public static string NormalizePboPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return path;
        }

        var result = new char[path.Length];
        var lastWasSeparator = true; // Initialize flag to track if the last character was a separator
        var charsWritten = 0; // counter to keep track of the number of characters written to the result array

        foreach (var c in path)
        {
            if (c is '/' or '\\')
            {
                if (lastWasSeparator)
                {
                    continue; // Skip if the last character was a seperator.
                }

                result[charsWritten++] = '\\';
                lastWasSeparator = true;

                continue;
            }

            lastWasSeparator = false;
            result[charsWritten++] = char.ToLowerInvariant(c);
        }

        if (charsWritten > 0 && result[charsWritten - 1] == '\\')
        {
            charsWritten--;
        }

        return new string(result, 0, charsWritten);
    }

    public static string GetFilename(string path) =>  path.Split('\\')[^1];


    public static string GetParent(string path) =>
        path.Remove(path.LastIndexOf('\\') + 1);
}
