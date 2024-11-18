using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utils;

public static class AssetLoader
{
    public static Dictionary<string, Sprite> Images;
    public static Dictionary<string, Sprite> Backgrounds;
    public static Dictionary<string, BlockData> Blocks;
    public static Dictionary<string, AudioClip> Sounds;
    public static Dictionary<string, BiomeData> BiomesMain;
    public static Dictionary<string, BiomeData> BiomesSub;
    public static AudioClip Music;
    public static List<string> ToLoadImages;
    public static List<string> ToLoadSounds;
    public static void LoadMusic(string name)
    {
        if (Music != null)
        {
            Resources.UnloadAsset(Music);
        }
        Music = null;
        var wait =  Resources.LoadAsync("Music/" + name, typeof(AudioClip));
        wait.completed += (lol) =>
        {
            Music = wait.asset as AudioClip;
        };
    }

    public static void Init()
    {
        {
            var asset = Resources.Load<TextAsset>("blocks");
            var lines = asset.text.Split("\n");
            Resources.UnloadAsset(asset);

            Blocks = new Dictionary<string, BlockData>(1);

            foreach (var line in lines)
            {
                var args = line.Replace(" ", "").Replace("\t", "").Split("|");
                if (args.Length != 7) continue;
                var id = args[0].Replace("(NO)", "");
                Blocks[id] = new BlockData()
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
        }
        {
            var asset = Resources.Load<TextAsset>("biomes");
            var lines = asset.text.Split("\n");
            Resources.UnloadAsset(asset);

            BiomesMain = new Dictionary<string, BiomeData>(1);
            BiomesSub = new Dictionary<string, BiomeData>(1);

            foreach (var line in lines)
            {
                var args = line.Replace(" ", "").Replace("\t", "").Split("|");
                if (args.Length != 4 && args.Length != 5) continue;
                
                (args.Length == 5 ? BiomesMain : BiomesSub)[args[0]] = new BiomeData()
                {
                    id = args[0],
                    blocksOrBiomes = args[1].Split(","),
                    countMinimum = int.Parse(args[2]),
                    countMaximum = int.Parse(args[3]),
                    specialItem = args.Length == 5 ? args[4] : null
                };
            }
        }

        var bg = Resources.LoadAll<Sprite>("Backgrounds/");
        var bgs = new Dictionary<string, Sprite>(1);
        foreach (var sprite in bg)
        {
            bgs[sprite.name] = sprite;
        }
        Backgrounds = bgs;
    }
    public static void HandleImages()
    {
        if (Images == null) Images = new(ToLoadImages.Count);
        else {
            var keys = Images.Keys.ToList();
            foreach (var item in keys)
            {
                if (!ToLoadImages.Contains(item))
                {
                    Resources.UnloadAsset(Images[item]);
                    Images.Remove(item);
                }
            }
        }
        foreach (var item in ToLoadImages)
        {
            if (!Images.ContainsKey(item))
            {
                Images[item] = Resources.Load<Sprite>("Images/" + item);
            }
        }
    }
    public static void HandleSounds()
    {
        if ((int)(Config.Instance.sound * 100) == 0) return;

        if (Sounds == null) Sounds = new(ToLoadSounds.Count);
        else {
            var keys = Sounds.Keys.ToList();
            foreach (var item in keys)
            {
                if (!ToLoadSounds.Contains(item))
                {
                    Resources.UnloadAsset(Sounds[item]);
                    Sounds.Remove(item);
                }
            }
        }
        foreach (var item in ToLoadSounds)
        {
            if (!Sounds.ContainsKey(item))
            {
                Sounds[item] = Resources.Load<AudioClip>("Sounds/" + item);
            }
        }
    }
    public static void UnloadSounds()
    {
        if (Sounds == null) return;
        foreach (var item in Sounds)
        {
            Resources.UnloadAsset(item.Value);
        }
        Sounds = null;
    }
}