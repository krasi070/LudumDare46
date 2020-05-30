using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddBodyPartChoice : MonoBehaviour
{
    public GameObject bodyPartPanel;
    public GameObject bodyPartInfo;
    public GameObject buttonLayout;

    public Image bodyPartBackground;
    public Image bodyPartImage;

    private void Awake()
    {
        bodyPartPanel.SetActive(false);
    }

    public void MakeChoice(BodyPartData bodyPart)
    {
        bodyPartPanel.SetActive(true);
        AddBodyPartSprite(bodyPart);
        AddInfo(bodyPart);
        AddButtons(bodyPart);
        StartCoroutine(AnimateBodyPartImage());
        StartCoroutine(RotateBodyPartBacgroundImage());
    }

    public void ReplaceBodyPart(BodyPartData bodyPart, BodyPartType placeToBeReplaced)
    {
        PlayerStatus.BodyParts[placeToBeReplaced] = bodyPart;
        bodyPartPanel.SetActive(false);
        GetComponent<BattleManager>().UpdateState();
    }

    public void AddBodyPart(BodyPartData bodyPart, BodyPartType placeToAdd)
    {
        if (!PlayerStatus.BodyParts.ContainsKey(placeToAdd))
        {
            PlayerStatus.BodyParts.Add(placeToAdd, bodyPart);
        }
        else
        {
            PlayerStatus.BodyParts[placeToAdd] = bodyPart;
        }

        bodyPartPanel.SetActive(false);
        GetComponent<BattleManager>().UpdateState();
    }

    public void ConsumeBodyPart(int consumptionAmount)
    {
        PlayerStatus.DemonMeter = Mathf.Min(PlayerStatus.DemonMeter + consumptionAmount, PlayerStatus.MaxDemonMeter);
        bodyPartPanel.SetActive(false);
        GetComponent<BattleManager>().UpdateState();
    }

    private void AddBodyPartSprite(BodyPartData bodyPart)
    {
        bodyPartImage.sprite = bodyPart.sprite;
        bodyPartImage.GetComponent<RectTransform>().sizeDelta = bodyPart.winScreenSize;
    }

    private void AddInfo(BodyPartData bodyPart)
    {
        bodyPartInfo.GetComponentInChildren<TextMeshProUGUI>().text = $"{bodyPart.name}:";

        foreach (BodyPartTrait trait in bodyPart.traits)
        {
            GameObject traitTextInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/Text/TraitText"), bodyPartInfo.transform);

            ActionText actionText = traitTextInstance.GetComponent<ActionText>();
            actionText.text = trait.ToString();
            traitTextInstance.GetComponent<TextMeshProUGUI>().text = trait.ToString();
            actionText.description = TraitDescriptions.Get(trait);
        }
    }

    private void AddButtons(BodyPartData bodyPart)
    {
        foreach (BodyPartType type in bodyPart.type)
        {
            if (PlayerStatus.BodyParts.ContainsKey(type))
            {
                AddReplaceButton(bodyPart, type);
            }
            else
            {
                AddAddAsButton(bodyPart, type);
            }
        }

        AddConsumeButton(bodyPart);
    }

    private void AddReplaceButton(BodyPartData bodyPart, BodyPartType type)
    {
        GameObject buttonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/Buttons/ChoiceButton"), buttonLayout.transform);

        buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text =
                    $"Replace {EnumHelper.BodyPartTypeToString(type)}";

        buttonInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate { ReplaceBodyPart(bodyPart, type); });

        ActionButton actionButton = buttonInstance.transform.GetChild(0).gameObject.AddComponent<ActionButton>();
        actionButton.tiltDegrees = -3f;
        actionButton.description = $"Replaces {PlayerStatus.BodyParts[type].name}:\n";

        foreach (BodyPartTrait trait in PlayerStatus.BodyParts[type].traits)
        {
            actionButton.description += $"* {trait.ToString()}\n";
        }

        actionButton.description.TrimEnd();
    }

    private void AddAddAsButton(BodyPartData bodyPart, BodyPartType type)
    {
        GameObject buttonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/Buttons/ChoiceButton"), buttonLayout.transform);

        buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text =
                    $"Add as {EnumHelper.BodyPartTypeToString(type)}";

        buttonInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate { AddBodyPart(bodyPart, type); });

        ActionButton actionButton = buttonInstance.transform.GetChild(0).gameObject.AddComponent<ActionButton>();
        actionButton.tiltDegrees = -3f;
        actionButton.description = $"<i>Empty</i>";
    }

    private void AddConsumeButton(BodyPartData bodyPart)
    {
        GameObject consumeButtonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/Buttons/ChoiceButton"), buttonLayout.transform);

        consumeButtonInstance.GetComponentInChildren<TextMeshProUGUI>().text = $"Consume ({bodyPart.consumptionAmount})";
        consumeButtonInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate { ConsumeBodyPart(bodyPart.consumptionAmount); });
        
        ActionButton actionButton = consumeButtonInstance.transform.GetChild(0).gameObject.AddComponent<ActionButton>();
        actionButton.tiltDegrees = -3f;
        actionButton.description = $"Restore {bodyPart.consumptionAmount} Demon Life.";
    }

    private IEnumerator RotateBodyPartBacgroundImage()
    {
        RectTransform rectTransformBg = bodyPartBackground.GetComponent<RectTransform>();
        RectTransform rectTransformBodyPartImage = bodyPartImage.GetComponent<RectTransform>();
        float increment = 32f;
        float timeUntilNexRotation = 0.6f;
        float timer = 0f;

        while (bodyPartPanel.activeSelf)
        {
            timer += Time.deltaTime;

            if (timer >= timeUntilNexRotation)
            {
                timer = 0f;
                Vector3 eulers = new Vector3(0, 0, (rectTransformBg.localRotation.z + increment) % 360);
                rectTransformBg.Rotate(eulers);
                rectTransformBodyPartImage.Rotate(-eulers);
            }

            yield return null;
        }
    }

    private IEnumerator AnimateBodyPartImage()
    {
        RectTransform rectTransform = bodyPartImage.GetComponent<RectTransform>();
        Vector2 originalSize = rectTransform.sizeDelta;
        float relativeY = originalSize.y / originalSize.x;
        float multiplier = 1.5f;
        float speed = 8f;
        float sinValue = 0f;

        while (bodyPartPanel.activeSelf)
        {
            sinValue += Time.deltaTime * speed;
            float sizeDistortionAmount = Mathf.Sin(sinValue) * multiplier;
            Vector2 sizeDistortionVector = new Vector2(sizeDistortionAmount, sizeDistortionAmount * relativeY);
            rectTransform.sizeDelta = originalSize + sizeDistortionVector;

            yield return null;
        }

        rectTransform.sizeDelta = originalSize;
    }
}
