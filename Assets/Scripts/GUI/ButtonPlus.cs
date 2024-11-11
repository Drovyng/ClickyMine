using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonPlus : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    private bool _pressed;
    private bool _hovered;

    public bool pressed => _pressed;
    public bool hovered => _hovered;

    public UnityEvent OnClick;
    public UnityEvent OnPress;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        OnPress?.Invoke();
        _pressed = true;
    }
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        _hovered = true;
    }
}
