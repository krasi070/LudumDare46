using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public BodyPartData data;
    public int vitality;

    private bool _isSelected;
    private Enemy _parent;

    public bool IsAlive
    {
        get
        {
            return vitality > 0;
        }
    }

    private void Start()
    {
        vitality = data.vitality;
        _parent = transform.parent.GetComponent<Enemy>();
    }

    private void OnMouseEnter()
    {
        SetSelectBordersVisibility(true);
        _parent.ShowBodyPartUi(this);
    }

    private void OnMouseDown()
    {
        if (_isSelected)
        {
            _isSelected = false;
            SetSelectBordersVisibility(false, Color.black);
            _parent.selectedBodyPart = null;
        }
        else
        {
            Select();
        }
    }

    private void OnMouseExit()
    {
        if (_parent.selectedBodyPart == null)
        {
            SetSelectBordersVisibility(false, Color.black);
            _parent.uiText.gameObject.SetActive(false);
            _parent.uiText.gameObject.SetActive(false);

            return;
        }
        else if (!_isSelected) 
        {
            SetSelectBordersVisibility(false, Color.black);
        }

        _parent.ShowBodyPartUi();
    }

    public void Select()
    {
        _isSelected = true;
        SetSelectBordersVisibility(true, new Color32(202, 9, 9, 255));

        if (_parent.selectedBodyPart != null)
        {
            _parent.UnselectBodyPart();
        }

        _parent.selectedBodyPart = this;
    }

    public void Unselect()
    {
        _isSelected = false;
        SetSelectBordersVisibility(false, Color.black);
        _parent.selectedBodyPart = null;
    }

    private void SetSelectBordersVisibility(bool isVisible)
    {
        foreach (Transform border in transform)
        {
            SpriteRenderer borderRenderer = border.GetComponent<SpriteRenderer>();
            borderRenderer.enabled = isVisible;
        }
    }

    private void SetSelectBordersVisibility(bool isVisible, Color color)
    {
        foreach (Transform border in transform)
        {
            SpriteRenderer borderRenderer = border.GetComponent<SpriteRenderer>();
            borderRenderer.color = color;
            borderRenderer.enabled = isVisible;
        }
    }
}
