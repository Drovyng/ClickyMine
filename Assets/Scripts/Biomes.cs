using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Biomes
{
    public static void NewMain()
    {
        List<string> BiomesMain = AssetLoader.BiomesMain.Keys.ToList();
        var lastList = Config.Instance.BiomeMainVisitsList();
        if (lastList.Count >= BiomesMain.Count) lastList.Clear();
        foreach (var item in lastList)
        {
            BiomesMain.Remove(item);
        }
        var getted = BiomesMain[Random.Range(0, BiomesMain.Count)];

        lastList.Add(getted);
        Config.Instance.BiomeMainVisitsList(lastList);

        Config.Instance.BiomeMainCur = getted;
        Config.Instance.BiomeMainCount = Random.Range(AssetLoader.BiomesMain[getted].countMinimum, AssetLoader.BiomesMain[getted].countMaximum + 1);
        Config.Instance.BiomeSubVisits = "";

        Game.Instance.PlayMusic(getted);

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
        var lastList = Config.Instance.BiomeSubVisitsList();
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
        Config.Instance.BiomeSubVisitsList(lastList);

        Game.RecalcResources();
        AssetLoader.HandleImages();
        AssetLoader.HandleSounds();
    }
}