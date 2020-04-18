using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public const int MAX_VITAITY = 100;
    public const int MAX_DEMON_METER = 360;
    public const int CRITICAL_HIT_CHANCE = 1;
    public const int CRITICAL_HIT_MULTIPLIER = 3;
    public const int EXTRA_DAMAGE = 4;

    public int vitality;
    public int attack;
    public float demonMeter;
    public float demonMeterDepletionRate;

    public Slider demonMeterSlider;

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
        PlayerStatus.MaxVitality = MAX_VITAITY;
        PlayerStatus.Vitality = vitality;

        PlayerStatus.MaxDemonMeter = MAX_DEMON_METER;
        PlayerStatus.DemonMeter = demonMeter;

        PlayerStatus.Attack = attack;
        PlayerStatus.ExtraDamage = EXTRA_DAMAGE;

        PlayerStatus.CriticalHitChance = CRITICAL_HIT_CHANCE;
        PlayerStatus.CriticalHitMultiplier = CRITICAL_HIT_MULTIPLIER;

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
