using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Biomes
{
    public static void NewMain()
    {
        List<string> BiomesMain = AssetLoader.BiomesMain.Keys.ToList();
        var lastList = Config.Instance.BiomeMainLastList();
        foreach (var item in lastList)
        {
            if (BiomesMain.Contains(item)) BiomesMain.Remove(item);
        }
        var getted = BiomesMain[Random.Range(0, BiomesMain.Count)];

        Config.Instance.BiomeMainCur = getted;
        Config.Instance.BiomeMainCount = Random.Range(AssetLoader.BiomesMain[getted].countMinimum, AssetLoader.BiomesMain[getted].countMaximum + 1);
        Config.Instance.BiomeSubLast = "";

        lastList.Add(getted);
        while (lastList.Count > 1)
        {
            lastList.RemoveAt(0);
        }
        Config.Instance.BiomeMainLastList(lastList);

        NewSub();
    }
    public static void NewSub()
    {
        Config.Instance.BiomeMainCount--;

        if (Config.Instance.BiomeMainCount <= 0)
        {
            NewMain();
            return;
        }

        List<string> BiomesSub = AssetLoader.BiomesMain[Config.Instance.BiomeMainCur].blocksOrBiomes.ToList();
        var lastList = Config.Instance.BiomeSubLastList();
        foreach (var item in lastList)
        {
            if (BiomesSub.Contains(item)) BiomesSub.Remove(item);
        }
        var getted = BiomesSub[Random.Range(0, BiomesSub.Count)];

        Config.Instance.BiomeSubCur = getted;
        Config.Instance.BiomeSubCount = Random.Range(AssetLoader.BiomesSub[getted].countMinimum, AssetLoader.BiomesSub[getted].countMaximum + 1);

        Background.Change(getted);

        Biome_Text.SetText(Localization.Localize("biome." + getted));

        Blocks.Recalculate();

        lastList.Add(getted);
        Config.Instance.BiomeSubLastList(lastList);
    }
}