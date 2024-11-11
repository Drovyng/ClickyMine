using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Block_Text : MonoBehaviour
{
    private static Block_Text Instance;
    private Text _text;
    private Vector3 _pos;

    private void Awake()
    {
        Instance = this;
        _text = GetComponent<Text>();
        _pos = transform.localPosition;
    }
    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, _pos, Time.deltaTime * 20);
    }
    public static void SetText(string text)
    {
        Instance._text.text = text;
        Instance.transform.localPosition += Vector3.up * 60;
    }
}
