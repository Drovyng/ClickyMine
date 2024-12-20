using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonPlus))]
public class Block_Animator : MonoBehaviour
{
    public static Block_Animator Instance;

    private ButtonPlus _buttonPlus;
    private float _currentScale = 0;
    private float _targetScale = 1;
    [SerializeField] private Image _image;
    [SerializeField] private Image _outline;

    public void SetSprite(string name, bool outline)
    {
        _image.sprite = AssetLoader.Images["Blocks/b_" + name];
        _outline.enabled = outline;
        _currentScale = 0;
    }
    private void Awake()
    {
        Instance = this;
        _buttonPlus = GetComponent<ButtonPlus>();
        _buttonPlus.OnClick = null;
    }
    private void OnEnable()
    {
        _buttonPlus.OnPress.AddListener(Game.Instance.BlockClick);
    }
    private void OnDisable()
    {
        _buttonPlus.OnPress.RemoveAllListeners();
    }
    private void FixedUpdate()
    {
        if (_buttonPlus.pressed && _buttonPlus.hovered)
        {
            _targetScale = 0.75f;
            return;
        }
        _targetScale = 1 + Mathf.Sin(Time.time * Mathf.PI * 2) * 0.05f;
    }
    private void Update()
    {
        _currentScale = Mathf.Lerp(_currentScale, _targetScale, Time.deltaTime * 10);
        _outline.transform.localScale = Vector3.one * _currentScale;
    }
}
