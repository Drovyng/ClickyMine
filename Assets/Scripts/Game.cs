using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class Game : MonoBehaviour
{
    private const float DamageTimer = 0.1f;
    private const int SourcesCount = 20;

    public static Game Instance;
    [SerializeField] private GameObject[] ToActivate;
    private float _damageTimer;

    private List<AudioSource> _audioSources;
    private List<AudioSource> _audioSourcesPlaying;
    public AudioSource musicSource;
    public float musicLerp;

    public static void RecalcResources()
    {
        AssetLoader.ToLoadImages = new(1);
        AssetLoader.ToLoadSounds = new(1);
        foreach (var item in AssetLoader.BiomesSub[Config.Instance.BiomeSubCur].blocksOrBiomes)
        {
            AssetLoader.ToLoadImages.Add("b_" + item);
            AssetLoader.ToLoadImages.Add("p_" + item);

            var block = AssetLoader.Blocks[item];
            foreach (var snd in block.soundsTouch)
            {
                AssetLoader.ToLoadSounds.Add("t_" + snd);
            }
            foreach (var snd in block.soundsBreak)
            {
                AssetLoader.ToLoadSounds.Add("b_" + snd);
            }
        }
    }

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.volume = 0;

        Instance = this;

        foreach (var item in ToActivate)
        {
            item.SetActive(true);
        }
        AssetLoader.Init();
        RecalcResources();
        AssetLoader.HandleImages();
        AssetLoader.HandleSounds();

        Inventory.Load();
        Config.Save();
        Localization.Init();

        Blocks.Recalculate();


        _audioSources = new(SourcesCount);
        _audioSourcesPlaying = new(SourcesCount);
        for (int i = 0; i < SourcesCount; i++)
        {
            var source = new GameObject();
            _audioSources.Add(source.AddComponent<AudioSource>());
            source.transform.parent = transform;
        }

#if UNITY_EDITOR
        foreach (var item in AssetLoader.Blocks.Values)
        {
            /*
            if (!AssetLoader.Images.ContainsKey("b_" + item.id)) Debug.LogError("NO IMAGE: b_" + item.id);
            if (!AssetLoader.Images.ContainsKey("p_" + item.id)) Debug.LogError("NO IMAGE: p_" + item.id);
            foreach (var snd in item.soundsTouch)
            {
                if (!AssetLoader.Sounds.ContainsKey("t_" + snd)) Debug.LogError("NO AUDIO: t_" + snd);
            }
            foreach (var snd in item.soundsBreak)
            {
                if (!AssetLoader.Sounds.ContainsKey("b_" + snd)) Debug.LogError("NO AUDIO: b_" + snd);
            }
            */
            if (!Localization.Languages[Language.ENGLISH].ContainsKey("block." + item.id)) Debug.LogError("NO LANG [ENGLISH]: block." + item.id);
            if (!Localization.Languages[Language.RUSSIAN].ContainsKey("block." + item.id)) Debug.LogError("NO LANG [RUSSIAN]: block." + item.id);
        }
#endif
    }
    private void Start()
    {
        Background.Change(Config.Instance.BiomeSubCur, false);
        Biome_Text.SetText(Localization.Localize("biome." + Config.Instance.BiomeSubCur));

        Blocks.NewBlock();

        PlayMusic(Config.Instance.BiomeMainCur);
    }
    public void BlockClick()
    {
        if (_damageTimer > 0)
        {
            return;
        }
        _damageTimer = DamageTimer;
        Score_Text.Click();
        Blocks.CurrentBlockHealth -= 1;
        if (Blocks.CurrentBlockHealth <= 0)
        {
            ParticlesGenerator.BlockBreak(Blocks.CurrentBlock.id);
            PlaySound("b_" + Blocks.CurrentBlock.soundsBreak[Random.Range(0, Blocks.CurrentBlock.soundsBreak.Length)], Blocks.CurrentBlock.soundBreakPitch);

            Config.Instance.BiomeSubCount--;
            if (Config.Instance.BiomeSubCount <= 0)
            {
                Biomes.NewSub();
            }
            Inventory.Add(Blocks.CurrentBlock.id);

            Blocks.NewBlock();
            _damageTimer = DamageTimer * 2;

            Config.Save();
            return;
        }
        ParticlesGenerator.BlockTouch(Blocks.CurrentBlock.id);
        PlaySound("t_" + Blocks.CurrentBlock.soundsTouch[Random.Range(0, Blocks.CurrentBlock.soundsTouch.Length)], Blocks.CurrentBlock.soundTouchPitch, 0.25f);
        Block_Health.target = Blocks.CurrentBlockHealth / (float)Blocks.CurrentBlock.health;
    }
    private void FixedUpdate()
    {
        if (_damageTimer > 0)
        {
            _damageTimer -= Time.fixedDeltaTime;
        }
        int i = 0;
        while (i < _audioSourcesPlaying.Count)
        {
            if (!_audioSourcesPlaying[i].isPlaying)
            {
                _audioSources.Add(_audioSourcesPlaying[i]);
                _audioSourcesPlaying.Remove(_audioSourcesPlaying[i]);
                continue;
            }
            i++;
        }
    }
    public void PlaySound(string name, float pitch = 1f, float volume = 1f)
    {
        if (AssetLoader.Sounds == null) return;
        if (_audioSources.Count > 0)
        {
            var source = _audioSources[0];
            _audioSourcesPlaying.Add(source);
            _audioSources.Remove(source);
            source.pitch = pitch;
            source.PlayOneShot(AssetLoader.Sounds[name], volume * Config.Instance.sound);
        }
    }
    public void PlayMusic(string name)
    {
        if ((int)(Config.Instance.music * 100) > 0) StartCoroutine(PlayMusicE(name));
    }
    private IEnumerator PlayMusicE(string name)
    {
        musicLerp = 1;
        while (musicLerp > 0)
        {
            yield return new WaitForFixedUpdate();
            musicLerp -= Time.fixedDeltaTime * 0.4f;
            musicSource.volume = musicLerp * 0.5f * Config.Instance.music;
        }
        AssetLoader.LoadMusic(name);
        yield return new WaitUntil(() => AssetLoader.Music != null);

        musicSource.clip = AssetLoader.Music;
        musicSource.Play();
        while (musicLerp < 1)
        {
            yield return new WaitForFixedUpdate();
            musicLerp += Time.fixedDeltaTime * 0.4f;
            musicSource.volume = Mathf.Min(musicLerp, 1) * 0.5f * Config.Instance.music;
        }
        yield break;
    }
}
