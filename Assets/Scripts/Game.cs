using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        foreach (var item in ToActivate)
        {
            item.SetActive(true);
        }

        Instance = this;

        Inventory.Load();
        Config.Save();
        Localization.Init();
        AssetLoader.Init();

        Blocks.Recalculate();

        _audioSources = new(SourcesCount);
        _audioSourcesPlaying = new(SourcesCount);
        for (int i = 0; i < SourcesCount; i++)
        {
            var source = new GameObject();
            _audioSources.Add(source.AddComponent<AudioSource>());
            source.transform.parent = transform;
        }
    }
    private void Start()
    {
        Background.Change(Config.Instance.BiomeSubCur, false);
        Biome_Text.SetText(Localization.Localize("biome." + Config.Instance.BiomeSubCur));

        Blocks.NewBlock();
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
        if (_audioSources.Count > 0)
        {
            var source = _audioSources[0];
            _audioSourcesPlaying.Add(source);
            _audioSources.Remove(source);
            source.pitch = pitch;
            source.PlayOneShot(AssetLoader.Sounds[name], volume);
        }
    }
}
