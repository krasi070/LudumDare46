using System.Collections.Generic;

public static class PlayerStatus
{
    public static int MaxVitality { get; set; }

    public static int Vitality { get; set; }

    public static float MaxDemonMeter { get; set; }

    public static float DemonMeter { get; set; }

    public static bool IsAlive
    {
        get
        {
            return Vitality > 0 && DemonMeter > 0;
        }
    }

    public static int Attack { get; set; }

    public static int ExtraDamage { get; set; }

    public static int CriticalHitChance { get; set; }
    
    public static int CriticalHitMultiplier { get; set; }

    public static int EvadeChance { get; set; }

    public static bool IsPoisoned { get; set; }

    public static Dictionary<BodyPartType, BodyPartData> BodyParts { get; set; }

    public static Dictionary<BodyPartTrait, int> Traits { get; set; }

    public static EnemyType FightingWith { get; set; }

    public static Dictionary<EnemyType, int> EncountersWith { get; set; }
}
