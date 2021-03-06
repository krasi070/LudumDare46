﻿using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatus
{
    public static bool IsPaused { get; set; }

    public static bool IsMoving { get; set; }

    public static int MaxVitality { get; set; } = 100;

    public static int Vitality { get; set; } = 100;

    public static float DemonLife { get; set; } = 100;

    public static float DemonLifeDepletionRate { get; set; } = 2;

    public static bool IsAlive
    {
        get
        {
            return Vitality > 0 && DemonLife > 0;
        }
    }

    public static int Attack { get; set; } = 6;

    public static int ExtraDamage { get; set; } = 4;

    public static int CriticalHitChance { get; set; } = 1;

    public static int CriticalHitMultiplier { get; set; } = 3;

    public static int EvadeChance { get; set; } = 0;

    public static bool IsPoisoned { get; set; } = false;

    public static Dictionary<BodyPartType, BodyPartData> BodyParts { get; set; }

    public static Dictionary<BodyPartTrait, int> Traits { get; set; }

    public static GameObject CurrentEnemy { get; set; }

    public static void Reset()
    {
        MaxVitality = 100;
        Vitality = 100;
        DemonLife = 100;
        DemonLifeDepletionRate = 2;
        Attack = 6;
        ExtraDamage = 4;
        CriticalHitChance = 1;
        CriticalHitMultiplier = 3;
        EvadeChance = 0;
        IsPoisoned = false;
        BodyParts = null;
        Traits = null;
        CurrentEnemy = null;
    }
}
