using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image _descriptionTextBox;
    private TextMeshProUGUI _descriptionTextMesh;

    [TextArea]
    public string description;

    private void Start()
    {
        _descriptionTextBox = GameObject.Find("DescriptionTextBox").GetComponent<Image>();
        _descriptionTextMesh = GameObject.Find("Description").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_descriptionTextBox.enabled)
        {
            _descriptionTextBox.transform.position = Input.mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _descriptionTextBox.enabled = true;
        _descriptionTextMesh.enabled = true;
        _descriptionTextMesh.text = description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptionTextBox.enabled = false;
        _descriptionTextMesh.enabled = false;
    }
}
