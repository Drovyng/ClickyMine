using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Biome_Text : MonoBehaviour
{
    private static Biome_Text Instance;
    private Text _text;
    private float _lerp;

    private void Awake()
    {
        Instance = this;
        _text = GetComponent<Text>();
    }
    private void Update()
    {
        if (_lerp < 1)
        {
            _lerp += Time.deltaTime * 1.5f;
            var color = _text.color;
            color.a = _lerp;
            _text.color = color;
        }
    }
    public static void SetText(string text)
    {
        Instance._text.text = text;
        Instance._lerp = 0;
    }
}
