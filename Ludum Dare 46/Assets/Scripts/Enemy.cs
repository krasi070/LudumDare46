﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public new string name;
    public EnemyType type;

    public BodyPart[] bodyParts;
    public BodyPart selectedBodyPart;

    public TextMeshProUGUI uiText;

    public bool IsAlive
    {
        get
        {
            if (selectedBodyPart != null)
            {
                return selectedBodyPart.IsAlive;
            }

            return true;
        }
    }

    public bool IsPoisoned { get; set; }

    public Dictionary<BodyPartTrait, int> Traits { get; private set; }

    private void Start()
    {
        SetTraits();
    }

    public string Act()
    {
        // TODO: Implement enemy AI
        PlayerStatus.Vitality -= 10;
        AudioManager.instance.Play("TreeAttack", true);

        return $"{name} attacks you for 10 damage.";
    }

    public bool TakeDamageAllBodyParts(int amount)
    {
        foreach (BodyPart bodyPart in bodyParts)
        {
            bodyPart.vitality -= amount;
        }

        if (selectedBodyPart != null)
        {
            uiText.text = $"{selectedBodyPart.data.name} {selectedBodyPart.vitality}";
        }

        return selectedBodyPart != null && selectedBodyPart.vitality <= 0;
    }

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
        uiText.text = $"{bodyPart.data.name} {bodyPart.vitality}";
        uiText.gameObject.SetActive(true);
    }

    private void SetTraits()
    {
        Traits = new Dictionary<BodyPartTrait, int>();

        if (bodyParts != null)
        {
            foreach (BodyPart bodyPart in bodyParts)
            {
                foreach (BodyPartTrait trait in bodyPart.data.traits)
                {
                    if (!Traits.ContainsKey(trait))
                    {
                        Traits.Add(trait, 0);
                    }

                    Traits[trait]++;
                }
            }
        }
        else
        {
            Debug.Log("Body parts are not set!");
        }
    }
}
