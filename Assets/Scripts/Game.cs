using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class Game : MonoBehaviour
{
    private const float DamageTimer = 0.1f;
    private const int SourcesCount = 20;
    private const float SaveTimer = 2.5f;

    public static Game Instance;
    [SerializeField] private GameObject[] ToActivate;
    private float _damageTimer;
    private bool _damageTimerHit;
    private float _saveTimer = SaveTimer;

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
            AssetLoader.ToLoadImages.Add("Blocks/b_" + item);
            AssetLoader.ToLoadImages.Add("Blocks/p_" + item);

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

        AssetLoader.Init();
        RecalcResources();
        AssetLoader.HandleImages();
        AssetLoader.HandleSounds();

        foreach (var item in ToActivate)
        {
            item.SetActive(true);
        }

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
    }
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Game))]
    public class GameEditorScript : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Check Resources", new GUIStyle(GUI.skin.button) { fontSize = 20, fontStyle = FontStyle.Bold }))
            {
                Debug.Log("Checking Resources...");
                AssetLoader.Init();
                Localization.Init();
                foreach (var item in AssetLoader.Blocks.Values)
                {
                    if (Resources.Load<Sprite>("Images/Blocks/b_" + item.id) == null) Debug.LogError("NO IMAGE: Blocks/b_" + item.id);
                    if (Resources.Load<Sprite>("Images/Blocks/p_" + item.id) == null) Debug.LogError("NO IMAGE: Blocks/p_" + item.id);
                    foreach (var snd in item.soundsTouch)
                    {
                        if (Resources.Load<AudioClip>("Sounds/t_" + snd) == null) Debug.LogError("NO AUDIO: t_" + snd);
                    }
                    foreach (var snd in item.soundsBreak)
                    {
                        if (Resources.Load<AudioClip>("Sounds/b_" + snd) == null) Debug.LogError("NO AUDIO: b_" + snd);
                    }

                    if (!Localization.Languages[Language.ENGLISH].ContainsKey("block." + item.id)) Debug.LogError("NO LANG [ENGLISH]: block." + item.id);
                    if (!Localization.Languages[Language.RUSSIAN].ContainsKey("block." + item.id)) Debug.LogWarning("NO LANG [RUSSIAN]: block." + item.id);
                }
                foreach (var item in AssetLoader.BiomesMain)
                {
                    foreach (var sub in item.Value.blocksOrBiomes)
                    {
                        if (!AssetLoader.BiomesSub.ContainsKey(sub)) Debug.LogError("NO BIOME: " + sub);
                    }
                    if (Resources.Load<AudioClip>("Music/" + item.Key) == null) Debug.LogError("NO Music: " + item.Key);
                }
                foreach (var item in AssetLoader.BiomesSub)
                {
                    foreach (var sub in item.Value.blocksOrBiomes)
                    {
                        if (!AssetLoader.Blocks.ContainsKey(sub)) Debug.LogError("NO BLOCK: " + sub);
                    }
                    if (!AssetLoader.Backgrounds.ContainsKey(item.Key)) Debug.LogError("NO BACKGROUND: " + item.Key);
                    if (!Localization.Languages[Language.ENGLISH].ContainsKey("biome." + item.Key)) Debug.LogError("NO LANG [ENGLISH]: biome." + item.Key);
                    if (!Localization.Languages[Language.RUSSIAN].ContainsKey("biome." + item.Key)) Debug.LogError("NO LANG [RUSSIAN]: biome." + item.Key);
                }
                Debug.Log("Checking Resources Completed!!!");
            }
            base.OnInspectorGUI();
        }
    }
#endif
    private void Start()
{
        Background.Change(Config.Instance.BiomeSubCur, false);
        Biome_Text.SetText(Localization.Localize("biome." + Config.Instance.BiomeSubCur));

        Blocks.NewBlock(true);

        PlayMusic(Config.Instance.BiomeMainCur);

        Application.quitting += Config.Save;
        Application.unloading += Config.Save;
    }
    public void BlockClick()
    {
        if (_damageTimer > 0)
        {
            _damageTimerHit = true;
            return;
        }
        _damageTimer = DamageTimer;
        Score_Text.Click();
        Blocks.CurrentBlockHealth -= 1;
        Config.Instance.BlockHealth = Blocks.CurrentBlockHealth;
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
            _damageTimer = DamageTimer * 1.5f;
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
            if (_damageTimer <= 0 && _damageTimerHit)
            {
                BlockClick();
                _damageTimerHit = false;
            }
        }
        _saveTimer -= Time.fixedDeltaTime;
        if (_saveTimer <= 0)
        {
            Config.Save();
            _saveTimer = SaveTimer;
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
