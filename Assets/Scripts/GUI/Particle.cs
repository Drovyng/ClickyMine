using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Particle : MonoBehaviour
{
    private const int ParticleSizeMin = 50;
    private const int ParticleSizeMax = 80;
    private const float UVStep = 1f / 16f;

    private RectTransform _rectTransform;
    private RawImage _image;
    private float _timer;
    private Vector2 _force;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<RawImage>();
        _image.enabled = false;
    }
    public void Launch(Sprite sprite, Vector2 position, float force, float timer)
    {
        _image.texture = sprite.texture;
        transform.localPosition = new Vector3(position.x, position.y, 0);
        _force = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * force;
        _timer = timer;
        int size = Random.Range(2, 4);
        float x = Random.Range(0, 17 - size);
        float y = Random.Range(0, 17 - size);
        _image.uvRect = new Rect(x * UVStep, y * UVStep, size * UVStep, size * UVStep);
        _rectTransform.sizeDelta = Vector2.one * Random.Range(ParticleSizeMin, ParticleSizeMax);
        _image.enabled = true;
    }
    public bool UpdateMe(float delta)
    {
        _force.y -= 8000 * delta;
        transform.localPosition += new Vector3(_force.x, _force.y, 0) * delta;
        _timer -= delta;
        if (_timer <= 0)
        {
            _image.enabled = false;
            return true;
        }
        return false;
    }
}
