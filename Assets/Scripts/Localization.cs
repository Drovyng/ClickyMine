using System;
using System.Collections.Generic;
using UnityEngine;

public static class Localization
{
    public static Dictionary<Language, Dictionary<string, string>> Languages;
    public static void Init()
    {
        Languages = new();
        var count = Enum.GetNames(typeof(Language)).Length;
        for (int i = 0; i < count; i++)
        {
            Languages.Add((Language)i, new());
        }
        var assetLocalization = Resources.Load<TextAsset>("localization");
        Parse(assetLocalization.text);
        Resources.UnloadAsset(assetLocalization);
    }
    public static string Localize(string key)
    {
        if (Languages[Config.Instance.Currentlanguage].ContainsKey(key))
        {
            return Languages[Config.Instance.Currentlanguage][key];
        }
        return Languages[Language.ENGLISH][key];
    }
    private static void Parse(string asset)
    {
        var lines = asset.Split("\n");
        Language curLang = Language.ENGLISH;
        string? content = null;
        string key = "";
        foreach (var line in lines)
        {
            if (content != null)
            {
                if (line.Contains("'''"))
                {
                    Languages[curLang].Add(key, content);
                    content = null;
                    continue;
                }
                content += "\n" + line;
                continue;
            }
            if (line.StartsWith("# "))
            {
                Enum.TryParse(line.Substring(2).Replace("\n", ""), out curLang);
                continue;
            }
            var divider = line.IndexOf(':');
            if (line.Replace(" ", "").Replace("\n", "").Length < 1 || divider < 0)
            {
                continue;
            }
            key = line.Substring(0, divider);
            content = line.Substring(divider + 1);
            if (content.Contains("'''"))
            {
                content = content.Substring(content.IndexOf("'''") + 3);
                continue;
            }
            if (content.StartsWith(" ")) content = content.Substring(1);
            Languages[curLang].Add(key, content);
            content = null;
        }
    }
}
