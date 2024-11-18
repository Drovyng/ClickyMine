using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Block_Text : MonoBehaviour
{
    private static Block_Text Instance;
    private Text _text;
    private Vector3 _pos;
    private static float cur = 0;
    public static float Target = 1;

    private void Awake()
    {
        Instance = this;
        _text = GetComponent<Text>();
        _pos = transform.localPosition;
    }
    private void Update()
    {
        cur = Mathf.Lerp(cur, Target, Time.deltaTime * 20);
        var col = Instance._text.color;
        col.a = cur;
        Instance._text.color = col;
        transform.localPosition = Vector3.Lerp(transform.localPosition, _pos, Time.deltaTime * 20);
    }
    public static void SetText(string text)
    {
        Instance._text.text = text;
        Instance.transform.localPosition += Vector3.up * 60;
    }
}
