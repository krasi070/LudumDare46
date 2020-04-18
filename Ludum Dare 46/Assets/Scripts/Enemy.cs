using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    public BodyPart[] bodyParts;
    public BodyPart selectedBodyPart;

    public TextMeshProUGUI bodyPartNameUi;
    public Slider bodyPartVitalitySlider;

    public void UnselectBodyPart()
    {
        if (selectedBodyPart != null)
        {
            selectedBodyPart.Unselect();
        }
        else
        {
            Debug.LogError("Selected body part is null!");
        }
    }

    /// <summary>
    /// Show selected body part's UI.
    /// </summary>
    public void ShowBodyPartUi()
    {
        ShowBodyPartUi(selectedBodyPart);
    }

    public void ShowBodyPartUi(BodyPart bodyPart)
    {
        bodyPartNameUi.text = bodyPart.data.name;
        bodyPartNameUi.gameObject.SetActive(true);

        bodyPartVitalitySlider.maxValue = bodyPart.data.vitality;
        bodyPartVitalitySlider.value = bodyPart.vitality;
        bodyPartVitalitySlider.gameObject.SetActive(true);
    }
}
