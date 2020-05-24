using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DemonUIHandler : MonoBehaviour
{
    public GameObject demonMenu;
    public GameObject bodyPartPositions;
    public TextMeshProUGUI traitsTextMesh;
    public Button consumeButton;

    [HideInInspector]
    public BodyPartUIHandler selectedBodyPart;

    public Dictionary<BodyPartType, Vector3> BodyPartPositions { get; private set; }

    private void Start()
    {
        BodyPartPositions = new Dictionary<BodyPartType, Vector3>();

        BodyPartType[] bodyParts = (BodyPartType[])Enum.GetValues(typeof(BodyPartType));

        // Uncomment when the body part prefabs are done
        //
        // foreach (BodyPartType bodyPart in bodyParts)
        // {
        //     BodyPartPositions.Add(bodyPart, GameObject.Find($"{bodyPartPositions.name}/{bodyPart.ToString()}").transform.position);
        // }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerStatus.IsPaused = !PlayerStatus.IsPaused;
            demonMenu.SetActive(!demonMenu.activeSelf);
            Cursor.visible = demonMenu.activeSelf;
        }
    }

    public void ShowBodyPartData(BodyPartData data)
    {
        traitsTextMesh.text = $"{data.name}\n";

        foreach (BodyPartTrait trait in data.traits)
        {
            traitsTextMesh.text += $"* {trait.ToString()}\n";
        }

        consumeButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Consume {data.consumptionAmount}";

        traitsTextMesh.gameObject.SetActive(true);
        consumeButton.gameObject.SetActive(true);
    }

    public void HideBodyPartData()
    {
        traitsTextMesh.gameObject.SetActive(false);
        consumeButton.gameObject.SetActive(false);
    }
}
