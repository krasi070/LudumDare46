using System.Collections.Generic;

public static class PlayerStatus
{
    public static bool IsPaused { get; set; }

    public static int MaxVitality { get; set; } = 100;

    public static int Vitality { get; set; } = 100;

    public static float MaxDemonMeter { get; set; } = 360;

    public static float DemonMeter { get; set; } = 180;

    public static float DemonMeterDepletionRate { get; set; } = 2;

    public static bool IsAlive
    {
        get
        {
            return Vitality > 0 && DemonMeter > 0;
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

    public static EnemyType FightingWith { get; set; }

    public static Dictionary<EnemyType, int> EncountersWith { get; set; }

    public static void Reset()
    {
        MaxVitality = 100;
        Vitality = 100;
        MaxDemonMeter = 360;
        DemonMeter = 180;
        DemonMeterDepletionRate = 2;
        Attack = 6;
        ExtraDamage = 4;
        CriticalHitChance = 1;
        CriticalHitMultiplier = 3;
        EvadeChance = 0;
        IsPoisoned = false;
        BodyParts = null;
        Traits = null;
        EncountersWith = null;
    }
}
