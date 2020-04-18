using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public BodyPartData data;
    public int vitality;

    private bool _isSelected;
    private SpriteRenderer _renderer;
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
        _renderer = GetComponent<SpriteRenderer>();
        _parent = transform.parent.GetComponent<Enemy>();
    }

    private void OnMouseEnter()
    {
        _renderer.enabled = true;
        _parent.ShowBodyPartUi(this);
    }

    private void OnMouseDown()
    {
        if (_isSelected)
        {
            _isSelected = false;
            _renderer.color = Color.white;
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
            _renderer.enabled = false;
            _parent.bodyPartNameUi.gameObject.SetActive(false);
            _parent.bodyPartVitalitySlider.gameObject.SetActive(false);

            return;
        }
        else if (!_isSelected) 
        {
            _renderer.enabled = false;
        }

        _parent.ShowBodyPartUi();
    }

    public void Select()
    {
        _isSelected = true;
        _renderer.color = new Color32(243, 198, 35, 255);

        if (_parent.selectedBodyPart != null)
        {
            _parent.UnselectBodyPart();
        }

        _parent.selectedBodyPart = this;
    }

    public void Unselect()
    {
        _isSelected = false;
        _renderer.color = Color.white;
        _renderer.enabled = false;
        _parent.selectedBodyPart = null;
    }
}
