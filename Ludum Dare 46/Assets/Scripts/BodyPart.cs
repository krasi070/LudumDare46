using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public BodyPartData data;
    public int vitality;

    private bool _isSelected;
    private SpriteRenderer _renderer;
    private Enemy _parent;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _parent = transform.parent.GetComponent<Enemy>();
    }

    private void OnMouseEnter()
    {
        _renderer.enabled = true;
    }

    private void OnMouseDown()
    {
        if (_isSelected)
        {
            Unselect();
        }
        else
        {
            Select();
        }
    }

    private void OnMouseExit()
    {
        if (!_isSelected)
        {
            _renderer.enabled = false;
        }
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
