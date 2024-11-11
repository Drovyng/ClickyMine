using System.Collections.Generic;
using UnityEngine;
using static Utils;

public static class AssetLoader
{
    public static IReadOnlyDictionary<string, Sprite> Images;
    public static IReadOnlyDictionary<string, Sprite> Backgrounds;
    public static IReadOnlyDictionary<string, BlockData> Blocks;
    public static IReadOnlyDictionary<string, AudioClip> Sounds;
    public static IReadOnlyDictionary<string, BiomeData> BiomesMain;
    public static IReadOnlyDictionary<string, BiomeData> BiomesSub;
    public static void Init()
    {
        {
            var asset = Resources.Load<TextAsset>("blocks");
            var lines = asset.text.Split("\n");
            Resources.UnloadAsset(asset);

            var blocks = new Dictionary<string, BlockData>(1);

            foreach (var line in lines)
            {
                var args = line.Replace(" ", "").Replace("\t", "").Split("|");
                if (args.Length != 7) continue;
                var id = args[0].Replace("(NO)", "");
                blocks[id] = new BlockData()
                {
                    id = id,
                    health = int.Parse(args[1]),
                    rarity = ParseFloat(args[2]),
                    soundsTouch = args[3].Split(","),
                    soundTouchPitch = ParseFloat(args[4]),
                    soundsBreak = args[5].Split(","),
                    soundBreakPitch = ParseFloat(args[6]),
                    outline = !args[0].Contains("(NO)")
                };
            }
            Blocks = blocks;
        }
        {
            var asset = Resources.Load<TextAsset>("biomes");
            var lines = asset.text.Split("\n");
            Resources.UnloadAsset(asset);

            var biomesMain = new Dictionary<string, BiomeData>(1);
            var biomesSub = new Dictionary<string, BiomeData>(1);

            foreach (var line in lines)
            {
                var args = line.Replace(" ", "").Replace("\t", "").Split("|");
                if (args.Length != 5) continue;
                
                (args[1].ToLower() == "true" ? biomesMain : biomesSub)[args[0]] = new BiomeData()
                {
                    id = args[0],
                    isMain = args[1].ToLower() == "true",
                    blocksOrBiomes = args[2].Split(","),
                    countMinimum = int.Parse(args[3]),
                    countMaximum = int.Parse(args[4])
                };
            }
            BiomesMain = biomesMain;
            BiomesSub = biomesSub;
        }



        var sprites = Resources.LoadAll<Sprite>("Images/");
        var images = new Dictionary<string, Sprite>(1);
        foreach (var sprite in sprites)
        {
            images[sprite.name] = sprite;
        }
        Images = images;



        var bg = Resources.LoadAll<Sprite>("Backgrounds/");
        var bgs = new Dictionary<string, Sprite>(1);
        foreach (var sprite in bg)
        {
            bgs[sprite.name] = sprite;
        }
        Backgrounds = bgs;



        var audioClips = Resources.LoadAll<AudioClip>("Sounds/");
        var sounds = new Dictionary<string, AudioClip>(1);
        foreach (var audioClip in audioClips)
        {
            sounds[audioClip.name] = audioClip;
        }
        Sounds = sounds;
    }
}