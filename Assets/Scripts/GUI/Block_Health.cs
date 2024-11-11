using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectMask2D), typeof(RectTransform))]
public class Block_Health : MonoBehaviour
{
    private RectMask2D _rectMask;

    private float _current = 0;
    private float width;
    public static float target = 1;

    private void Awake()
    {
        _rectMask = GetComponent<RectMask2D>();
        width = GetComponent<RectTransform>().sizeDelta.x;
    }
    private void Update()
    {
        _current = Mathf.Lerp(_current, target, Time.deltaTime * 10);
        _rectMask.padding = new Vector4(0, 0, width * (1 - _current), 0);
    }
}
