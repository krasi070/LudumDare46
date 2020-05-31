using System.Collections;
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

    private Vector3 _hiddenPosition = new Vector3(0, -420);
    private Vector3 _visiblePosition = Vector3.zero;
    private bool _moving = false;

    private void Awake()
    {
        demonMenu.SetActive(true);
        demonMenu.transform.position = _hiddenPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !_moving)
        {
            PlayerStatus.IsPaused = !PlayerStatus.IsPaused;
            HideBodyPartData();

            if (PlayerStatus.IsPaused)
            {
                DestroyBodyParts();
                LoadBodyParts();
                StartCoroutine(Move(1));
            }
            else
            {
                StartCoroutine(Move(-1));
            }

            Cursor.visible = demonMenu.activeSelf;
        }
    }

    public void ShowBodyPartData(BodyPartUIHandler bodyPartObj)
    {
        BodyPartData data = bodyPartObj.data;
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
        ActionButton consumeActionButton = consumeButton.GetComponentInChildren<ActionButton>();
        consumeActionButton.description = $"Restore {data.consumptionAmount} Demon Life.";
        consumeButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Consume ({data.consumptionAmount})";
        consumeButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        consumeButton.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            bodyPartObj.Consume();
            consumeActionButton.HideTextBox(); 
        });

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

    private IEnumerator Move(int direction)
    {
        _moving = true;
        RectTransform rectTransform = demonMenu.GetComponent<RectTransform>();

        float duration = 0.15f;
        float timer = 0f;

        Vector3 startingPosition = rectTransform.localPosition;
        Vector3 destination = direction > 0 ? _visiblePosition : _hiddenPosition;

        while ((rectTransform.localPosition - destination).sqrMagnitude != 0)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0f, duration);
            float multiplier = timer / duration;
            rectTransform.localPosition = 
                new Vector3(0, startingPosition.y + direction * Mathf.Abs(startingPosition.y - destination.y) * multiplier);

            yield return null;
        }

        rectTransform.localPosition = destination;
        _moving = false;
    }
}
