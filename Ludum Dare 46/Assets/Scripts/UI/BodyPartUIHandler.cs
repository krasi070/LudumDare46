using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BodyPartUIHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
    IPointerClickHandler, IDragHandler, IEndDragHandler
{
    public BodyPartData data;
    public BodyPartType type;

    private bool _selected;
    private Image _selectImage;
    private DemonUIHandler _demonUIHandler;

    private void Start()
    {
        _selectImage = transform.GetChild(0).GetComponent<Image>();
        _demonUIHandler = GameObject.Find("DemonUIHandler").GetComponent<DemonUIHandler>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _selectImage.enabled = true;
        _demonUIHandler.ShowBodyPartData(data);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_demonUIHandler.selectedBodyPart == null)
        {
            _selectImage.enabled = false;
            _selectImage.color = Color.white;
            _demonUIHandler.HideBodyPartData();
        }
        else if (_demonUIHandler.selectedBodyPart != this)
        {
            _selectImage.enabled = false;
            _selectImage.color = Color.white;
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

            _selectImage.color = new Color32(243, 198, 35, 255);
            _demonUIHandler.selectedBodyPart = this;
        }
        else
        {
            _selectImage.color = Color.white;
            _demonUIHandler.selectedBodyPart = null;
            _selected = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = _demonUIHandler.BodyPartPositions[type];
    }

    private void Unselect()
    {
        _selectImage.color = Color.white;
        _selected = false;
        _selectImage.enabled = false;
        _demonUIHandler.selectedBodyPart = null;
    }
}
