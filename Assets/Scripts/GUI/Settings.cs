using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Text _textTop;
    [SerializeField] private Text _textLang;

    [SerializeField] private Text _textVersion;

    [SerializeField] private Button _btnLang;
    [SerializeField] private Text _btnLangText;

    [SerializeField] private Button _btnBack;
    [SerializeField] private Text _btnBackText;

    [SerializeField] private Button _btnAppear;

    public static float TargetScale = 0;
    private float _currentScale = 0;

    private void Reload()
    {
        _textTop.text = Localization.Localize("settings.text-top");
        _textLang.text = Localization.Localize("settings.text-lang");

        _btnLangText.text = Localization.Localize("settings.btn-lang");
        _btnBackText.text = Localization.Localize("settings.btn-back");
    }
    private void OnEnable()
    {
        _btnLang.onClick.AddListener(ChangeLanguage);
        _btnBack.onClick.AddListener(Change);
        _btnAppear.onClick.AddListener(Change);
    }
    private void OnDisable()
    {
        _btnLang.onClick.RemoveAllListeners();
        _btnBack.onClick.RemoveAllListeners();
        _btnAppear.onClick.RemoveAllListeners();
    }
    private void Start()
    {
        transform.localScale = Vector3.zero;
        _textVersion.text = "v" + Application.version;
        Reload();
    }
    private void Update()
    {
        _currentScale = Mathf.Lerp(_currentScale, TargetScale, Time.deltaTime * 20);
        transform.localScale = Vector3.one * _currentScale;
    }
    private void ChangeLanguage()
    {
        Config.Instance.Currentlanguage += 1;
        if ((byte)Config.Instance.Currentlanguage >= Enum.GetNames(typeof(Language)).Length)
        {
            Config.Instance.Currentlanguage = 0;
        }
        Block_Text.SetText(Localization.Localize("block." + Blocks.CurrentBlock.id));
        Biome_Text.SetText(Localization.Localize("biome." + Config.Instance.BiomeSubCur));
        Config.Save();
        Reload();
    }
    private void Change()
    {
        TargetScale = 1 - TargetScale;
    }
}
