using System;
using System.Net.Http;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CheckUpdates : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Text _txtUpdate;
    [SerializeField] private Text _txtSkip;
    static HttpClient httpClient = new HttpClient();
    private static CheckUpdates Instance;
    public const int GameVersion = 2;
    private float _currentScale = 0;
    private float TargetScale = 0;
    private static async void CheckUpdate()
    {
        try
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://raw.githubusercontent.com/Drovyng/ClickyMine/refs/heads/main/LATEST_VERSION");
            var result = await httpClient.SendAsync(request);
            var text = await result.Content.ReadAsStringAsync();
            var version = int.Parse(text.Replace(" ", "").Split("-b")[1]);
            if (version > int.Parse(Application.version.Replace(" ", "").Split("-b")[1]))
            {
                Instance.TargetScale = 1;
                Instance._text.text = Localization.Localize("outdated.title");
                Instance._txtUpdate.text = Localization.Localize("outdated.update");
                Instance._txtSkip.text = Localization.Localize("outdated.skip");
            }
            else
            {
                Instance.Close();
            }
        }
        finally { }
    }
    public static void OpenGamePage()
    {
        Application.OpenURL("https://www.rustore.ru/catalog/app/com.Drovyng.ClickyMine");
        Application.Quit();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    private void Start()
    {
        Instance = this;

        transform.localScale = Vector3.zero;

        CheckUpdate();
    }
    private void Update()
    {
        _currentScale = Mathf.Lerp(_currentScale, TargetScale, Time.deltaTime * 10);
        transform.localScale = Vector3.one * _currentScale;
    }
}