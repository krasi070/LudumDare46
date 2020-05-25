using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DemonUIHandler : MonoBehaviour
{
    public GameObject demonMenu;
    public GameObject bodyPartPositions;
    public TextMeshProUGUI bodyPartNameTextMesh;
    public GameObject infoPanel;
    public GameObject consumeButton;

    [HideInInspector]
    public BodyPartUIHandler selectedBodyPart;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerStatus.IsPaused = !PlayerStatus.IsPaused;
            demonMenu.SetActive(!demonMenu.activeSelf);
            HideBodyPartData();

            if (demonMenu.activeSelf)
            {
                DestroyBodyParts();
                LoadBodyParts();
            }

            Cursor.visible = demonMenu.activeSelf;
        }
    }

    public void ShowBodyPartData(BodyPartData data)
    {
        bodyPartNameTextMesh.text = $"{data.name}:";

        // Destroy old instances of trait texts
        foreach (Transform child in infoPanel.transform)
        {
            if (child.tag == "Trait Text")
            {
                Destroy(child.gameObject);
            }
        }

        // Add current traits
        foreach (BodyPartTrait trait in data.traits)
        {
            GameObject traitTextInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Text/TraitText"), infoPanel.transform);
            RectTransform rect = traitTextInstance.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(110, 30);

            TextMeshProUGUI textMesh = traitTextInstance.GetComponent<TextMeshProUGUI>();
            textMesh.fontSize = bodyPartNameTextMesh.fontSize;
            textMesh.text = trait.ToString();

            ActionText actionText = traitTextInstance.GetComponent<ActionText>();
            actionText.description = TraitDescriptions.Get(trait);
            actionText.text = trait.ToString();
        }

        // Modify consume button
        consumeButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Consume ({data.consumptionAmount})";
        consumeButton.GetComponentInChildren<ActionButton>().description = $"Restore {data.consumptionAmount} Demon Life.";

        infoPanel.SetActive(true);
        consumeButton.gameObject.SetActive(true);
    }

    public void HideBodyPartData()
    {
        infoPanel.SetActive(false);
        consumeButton.gameObject.SetActive(false);
    }

    private void LoadBodyParts()
    {
        foreach (KeyValuePair<BodyPartType, BodyPartData> pair in PlayerStatus.BodyParts)
        {
            if (pair.Value != null)
            {
                int index = 0;

                for (int i = 0; i < pair.Value.type.Length; i++)
                {
                    if (pair.Key == pair.Value.type[i])
                    {
                        index = i;
                        break;
                    }
                }

                GameObject menuBodyPartInstance = Instantiate(pair.Value.demonMenuObject[index], demonMenu.transform);

                foreach (Transform selectBorderCorner in menuBodyPartInstance.transform.GetChild(0))
                {
                    selectBorderCorner.GetComponent<Image>().enabled = false;
                }
            }
        }
    }

    private void DestroyBodyParts()
    {
        foreach (Transform child in demonMenu.transform)
        {
            if (child.tag == "Body Part")
            {
                Destroy(child.gameObject);
            }
        }
    }
}
