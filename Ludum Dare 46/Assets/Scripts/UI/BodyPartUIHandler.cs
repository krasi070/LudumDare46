using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BodyPartUIHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public BodyPartData data;
    public BodyPartType type;

    private bool _selected;
    private Transform _selectBorder;
    private DemonUIHandler _demonUIHandler;

    private void Start()
    {
        _selectBorder = transform.GetChild(0);
        _demonUIHandler = GameObject.Find("DemonUIHandler").GetComponent<DemonUIHandler>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetSelectBordersVisibility(true, new Color32(202, 9, 9, 255));
        _demonUIHandler.ShowBodyPartData(data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_demonUIHandler.selectedBodyPart == null)
        {
            SetSelectBordersVisibility(false, new Color32(202, 9, 9, 255));
            _demonUIHandler.HideBodyPartData();
        }
        else if (_demonUIHandler.selectedBodyPart != this)
        {
            SetSelectBordersVisibility(false, new Color32(202, 9, 9, 255));
            _demonUIHandler.ShowBodyPartData(_demonUIHandler.selectedBodyPart.data);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _selected = !_selected;

        if (_selected)
        {
            if (_demonUIHandler.selectedBodyPart != null)
            {
                _demonUIHandler.selectedBodyPart.Unselect();
            }

            SetSelectBordersVisibility(true, new Color32(202, 9, 9, 255));
            _demonUIHandler.selectedBodyPart = this;
        }
        else
        {
            SetSelectBordersVisibility(true, new Color32(202, 9, 9, 255));
            _demonUIHandler.selectedBodyPart = null;
            _selected = false;
        }
    }

    private void Unselect()
    {
        _selected = false;
        SetSelectBordersVisibility(false, new Color32(202, 9, 9, 255));
        _demonUIHandler.selectedBodyPart = null;
    }

    private void SetSelectBordersVisibility(bool isVisible)
    {
        foreach (Transform border in _selectBorder)
        {
            Image borderImage = border.GetComponent<Image>();
            borderImage.enabled = isVisible;
        }
    }

    private void SetSelectBordersVisibility(bool isVisible, Color color)
    {
        foreach (Transform border in _selectBorder)
        {
            Image borderImage = border.GetComponent<Image>();
            borderImage.color = color;
            borderImage.enabled = isVisible;
        }
    }
}
