using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Config
{
    public static readonly Config Instance = Load();

    public Language Currentlanguage = Language.ENGLISH;

    public string BiomeMainCur = "overworld";
    public int BiomeMainCount = 2;

    public string BiomeSubCur = "plains";
    public string BiomeSubLast = "plains";
    public int BiomeSubCount = 50;

    public string Items = "";
    public ulong Clicks = 0;

    public float music = 0.5f;
    public float sound = 1;

    private static Config Load()
    {
        if (PlayerPrefs.HasKey("Config"))
        {
            return JsonUtility.FromJson<Config>(PlayerPrefs.GetString("Config"));
        }
        return new Config();
    }
    public static void Save()
    {
        Inventory.Save();
        PlayerPrefs.SetString("Config", JsonUtility.ToJson(Instance));
    }
    private Config() { }

    #region HelpfulMethods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<string> BiomeSubLastList()
    {
        return BiomeSubLast.Split(",").ToList();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BiomeSubLastList(List<string> list)
    {
        BiomeSubLast = "";
        foreach (var item in list) BiomeSubLast += "," + item;
        BiomeSubLast = BiomeSubLast[1..];
    }
    #endregion
}