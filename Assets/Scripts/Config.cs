using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Config
{
    public static readonly Config Instance = Load();

    public Language Currentlanguage = Language.ENGLISH;

    public string BiomeMainCur = "overworld";
    public string BiomeMainLast = "overworld";
    public int BiomeMainCount = 2;

    public string BiomeSubCur = "plains";
    public string BiomeSubLast = "plains";
    public int BiomeSubCount = 50;

    public string Items = "";
    public ulong Clicks = 0;

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
    public List<string> BiomeMainLastList()
    {
        return BiomeMainLast.Split(",").ToList();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BiomeMainLastList(List<string> list)
    {
        BiomeMainLast = "";
        foreach (var item in list) BiomeMainLast += "," + item;
        BiomeMainLast = BiomeMainLast[1..];
    }
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