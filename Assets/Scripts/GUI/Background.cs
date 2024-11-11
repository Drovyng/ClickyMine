using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    private static Background Instance;
    private Image _image;
    [SerializeField] private Image _imageToLerp;
    private float _lerp = 0f;
    private float _dir = 1f;
    private float _cur = 0f;
    private float _target = 0f;
    private void Awake()
    {
        Instance = this;

        _image = GetComponent<Image>();

        if (Random.Range(0, 2) == 0)
        {
            _dir = -1f;
        }
    }
    private void Update()
    {
        var div = Time.time / 70f + 0.5f;
        _target = ((div % 2 < 1) ? div % 1 : 1 - div % 1) * 2 - 1;

        _cur = Mathf.Lerp(_cur, _target, Time.deltaTime * 0.4f);
        transform.localPosition = Vector3.right * _cur * 1000f * _dir;
        if (_lerp > 0)
        {
            _lerp -= Time.deltaTime;
            Instance._imageToLerp.color = new Color(1, 1, 1, _lerp);
        }
    }
    public static void Change(string name, bool smooth = true)
    {
        Instance._imageToLerp.sprite = Instance._image.sprite;
        Instance._image.sprite = AssetLoader.Backgrounds[name];
        if (smooth) 
        {
            Instance._imageToLerp.color = new Color(1, 1, 1, 1); 
            Instance._lerp = 1f; 
        }
    }
}
