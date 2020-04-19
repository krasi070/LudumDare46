using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int vitality;
    public float demonMeter;
    public float demonMeterDepletionRate;

    public Slider demonMeterSlider;

    private void Awake()
    {
        if (MapStatus.PlayerPosition != null)
        {
            transform.position = MapStatus.PlayerPosition;
        }
    }

    private void Start()
    {
        if (PlayerStatus.BodyParts == null)
        {
            PlayerStatus.BodyParts = new Dictionary<BodyPartType, BodyPartData>();
        }

        if (PlayerStatus.Traits == null)
        {
            PlayerStatus.Traits = new Dictionary<BodyPartTrait, int>();
        }

        if (PlayerStatus.EncountersWith == null)
        {
            PlayerStatus.EncountersWith = new Dictionary<EnemyType, int>();
            EnemyType[] enemyTypes = (EnemyType[])Enum.GetValues(typeof(EnemyType));

            foreach (EnemyType type in enemyTypes)
            {
                PlayerStatus.EncountersWith.Add(type, 0);
            }
        }
    }

    public void UpdateDemonMeter()
    {
        demonMeterSlider.value = demonMeter;
    }

    public void PrepareForEncounter(EnemyType enemyType)
    {
        PlayerStatus.Vitality = vitality;
        PlayerStatus.DemonMeter = demonMeter;
        PlayerStatus.DemonMeterDepletionRate = demonMeterDepletionRate;
        PlayerStatus.FightingWith = enemyType;

        foreach (BodyPartType bodyPart in PlayerStatus.BodyParts.Keys)
        {
            foreach (BodyPartTrait trait in PlayerStatus.BodyParts[bodyPart].traits)
            {
                if (!PlayerStatus.Traits.ContainsKey(trait))
                {
                    PlayerStatus.Traits.Add(trait, 0);
                }

                PlayerStatus.Traits[trait]++;
            }
        }
    }
}
