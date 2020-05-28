using System.Collections;
using UnityEngine;
using TMPro;

public class BodyPart : MonoBehaviour
{
    public BodyPartData data;
    public int vitality;

    private bool _isSelected;
    private Enemy _parent;
    private TextMeshPro _damageTextMesh;

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
        _damageTextMesh = GetComponentInChildren<TextMeshPro>();
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

    public void ShowDamage(int damage)
    {
        StartCoroutine(MoveDamageText(damage));
    }

    private void SetSelectBordersVisibility(bool isVisible)
    {
        foreach (Transform border in transform)
        {
            SpriteRenderer borderRenderer = border.GetComponent<SpriteRenderer>();

            if (borderRenderer != null)
            {
                borderRenderer.enabled = isVisible;
            }
        }
    }

    private void SetSelectBordersVisibility(bool isVisible, Color color)
    {
        foreach (Transform border in transform)
        {
            SpriteRenderer borderRenderer = border.GetComponent<SpriteRenderer>();

            if (borderRenderer != null)
            {
                borderRenderer.color = color;
                borderRenderer.enabled = isVisible;
            }
        }
    }

    private IEnumerator MoveDamageText(int damage)
    {
        _damageTextMesh.text = $"-{damage}";
        Vector3 originalPosition = _damageTextMesh.transform.position;
        float duration = 0.3f;
        float timer = 0f;
        float alpha = 1f;
        float speed = 2.5f;

        _damageTextMesh.color = new Color(_damageTextMesh.color.r, _damageTextMesh.color.g, _damageTextMesh.color.b, alpha);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            _damageTextMesh.transform.position += new Vector3(0f, speed * Time.deltaTime);

            yield return null;
        }

        while (alpha > 0)
        {
            alpha = Mathf.Clamp(alpha - Time.deltaTime * speed * 2, 0f, 1f);
            _damageTextMesh.transform.position += new Vector3(0f, speed * Time.deltaTime);
            _damageTextMesh.color = new Color(_damageTextMesh.color.r, _damageTextMesh.color.g, _damageTextMesh.color.b, alpha);

            yield return null;
        }

        _damageTextMesh.transform.position = originalPosition;
        _damageTextMesh.color = new Color(_damageTextMesh.color.r, _damageTextMesh.color.g, _damageTextMesh.color.b, 0);
    }
}
