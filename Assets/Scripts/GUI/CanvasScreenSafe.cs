using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CanvasScreenSafe : MonoBehaviour
{
    private void Start()
    {
        var _transform = GetComponent<RectTransform>();
        var multy = Mathf.Max(
            1440f / Screen.width,
            2560f / Screen.height
        );
        var rect = Screen.safeArea;
        _transform.offsetMin += new Vector2(rect.x, rect.y) * multy;
        _transform.offsetMax -= new Vector2(Screen.width - rect.width, Screen.height - rect.height) * multy;
    }
}
