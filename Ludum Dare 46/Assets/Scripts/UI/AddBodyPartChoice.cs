using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddBodyPartChoice : MonoBehaviour
{
    public GameObject bodyPartPanel;
    public GameObject bodyPartInfo;
    public GameObject buttonLayout;
    public Image bodyPartImage;

    public void MakeChoice(BodyPartData bodyPart)
    {
        bodyPartPanel.SetActive(true);
        bodyPartImage.sprite = bodyPart.sprite;
        AddInfo(bodyPart);
        AddButtons(bodyPart);
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
}
