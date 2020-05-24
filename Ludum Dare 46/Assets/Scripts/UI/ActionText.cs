using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ActionText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text;

    [TextArea]
    public string description;

    private Image _descriptionTextBox;
    private TextMeshProUGUI _descriptionTextMesh;

    private TextMeshProUGUI _textMesh;

    private void Start()
    {
        _descriptionTextBox = GameObject.Find("DescriptionTextBox")?.GetComponent<Image>();
        _descriptionTextMesh = GameObject.Find("Description")?.GetComponent<TextMeshProUGUI>();

        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_descriptionTextBox != null && _descriptionTextBox.enabled)
        {
            _descriptionTextBox.transform.position = Input.mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_descriptionTextBox != null)
        {
            _descriptionTextBox.enabled = true;
            _descriptionTextMesh.enabled = true;
            _descriptionTextMesh.text = description;
            _textMesh.text = $"<u>{text}</u>";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_descriptionTextBox != null)
        {
            _descriptionTextBox.enabled = false;
            _descriptionTextMesh.enabled = false;
            _textMesh.text = text;
        }
    }
}
