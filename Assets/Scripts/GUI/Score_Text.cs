using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Score_Text : MonoBehaviour
{
    private static Score_Text Instance;
    private Text _text;
    private float _lerp = 1;

    private void Awake()
    {
        Instance = this;
        _text = GetComponent<Text>();
        Instance._text.text = Config.Instance.Clicks.ToString();
    }
    private void Update()
    {
        _lerp = Mathf.Lerp(_lerp, 0.85f, Time.deltaTime * 5);
        transform.localScale = Vector3.one * _lerp;
    }
    public static void Click()
    {
        Instance._lerp += 0.1f;
        Config.Instance.Clicks++;
        Instance._text.text = Config.Instance.Clicks.ToString();
    }
}
