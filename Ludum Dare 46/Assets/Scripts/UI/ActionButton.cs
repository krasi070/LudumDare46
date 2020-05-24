using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Range(-360f, 360f)]
    public float tiltDegrees;

    [TextArea]
    public string description;

    private Image _descriptionTextBox;
    private TextMeshProUGUI _descriptionTextMesh;

    private Image _buttonImage;
    private TextMeshProUGUI _buttonText;

    private void Start()
    {
        _descriptionTextBox = GameObject.Find("DescriptionTextBox")?.GetComponent<Image>();
        _descriptionTextMesh = GameObject.Find("Description")?.GetComponent<TextMeshProUGUI>();

        _buttonImage = transform.parent.GetComponent<Image>();
        _buttonText = transform.GetComponentInChildren<TextMeshProUGUI>();
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
        }
        
        AudioManager.instance.Play("ButtonHover", true);
        Tilt(tiltDegrees);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Tilt(-tiltDegrees);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTextBox();
        Tilt(-tiltDegrees);
    }

    public void HideTextBox()
    {
        if (_descriptionTextBox != null)
        {
            _descriptionTextBox.enabled = false;
            _descriptionTextMesh.enabled = false;
        }
    }

    private void Tilt(float amount)
    {
        Vector3 eulers = new Vector3(0, 0, amount);

        _buttonImage.transform.Rotate(eulers);
        transform.Rotate(-eulers);
        _buttonText.transform.Rotate(eulers);
    }
}
