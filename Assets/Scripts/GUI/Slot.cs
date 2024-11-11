using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Image _border;
    [SerializeField] private Image _image;

    public void SetItem(string name, long count)
    {
        _image.sprite = AssetLoader.Images["b_" + name];
        _border.enabled = AssetLoader.Blocks[name].outline;
        _text.text = count.ToString();
    }
}
