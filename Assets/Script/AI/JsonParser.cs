using UnityEngine;
using System.Text.RegularExpressions;

public static class JsonParser
{
    public static string ExtractContent(string json)
    {
        var match = Regex.Match(json, "\"content\":\"(.*?)\"", RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value.Replace("\\n", "\n") : "[无回复]";
    }
}
