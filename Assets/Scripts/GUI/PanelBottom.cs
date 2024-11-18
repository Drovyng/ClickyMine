using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PanelBottom : MonoBehaviour
{
    private RectTransform _rectTransform;
    [SerializeField] RectTransform[] _rectTransforms;
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        float w = _rectTransform.rect.width;
        float x = w * -0.5f;
        for (int i = 0; i < _rectTransforms.Length; i++)
        {
            _rectTransforms[i].localPosition = Vector3.right * (x + w * (i + 1) / (_rectTransforms.Length + 1));
        }
    }
}
