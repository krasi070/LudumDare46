using UnityEngine;

public static class PlayerActions 
{
    /// <summary>
    /// Deal damage equal to player attack + extra damage.
    /// </summary>
    public static void Stab()
    {
        Enemy enemy = BattleManager.Instance.CurrentEnemy;
        bool isCriticalHit = Random.Range(1, 101) <= PlayerStatus.CriticalHitChance;
        int damageToDeal = PlayerStatus.Attack + Random.Range(0, PlayerStatus.ExtraDamage + 1);

        if (isCriticalHit)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
            // TODO: Give feedback to player that attack was critical
        }

        AudioManager.instance.Play("Stab", true);
        BattleManager.Instance.DamageEnemy(Mathf.CeilToInt(damageToDeal));
        BattleManager.Instance.UpdateState();

        string message = string.Empty;

        if (isCriticalHit)
        {
            message += $"Critical stab! {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} took {damageToDeal} damage!";
        }
        else
        {
            message += $"You stabbed {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} for {damageToDeal} damage!";
        }

        message = BattleManager.Instance.ConcatEnemyDefeatedMessageIfTrue(message);
        BattleManager.Instance.ShowInfoText(message);
    }

    /// <summary>
    /// 50% chance player will evade the next attack.
    /// </summary>
    public static void Anticipate()
    {
        PlayerStatus.EvadeChance = 50;
        BattleManager.Instance.UpdateState();

        BattleManager.Instance.ShowInfoText($"Anticipating {BattleManager.Instance.CurrentEnemy.name}'s next attack...");
    }

    /// <summary>
    /// Deal twice the damage to left or right eyes.
    /// </summary>
    public static void Claws()
    {
        Enemy enemy = BattleManager.Instance.CurrentEnemy;
        bool isCriticalHit = Random.Range(1, 101) <= PlayerStatus.CriticalHitChance;
        bool isEye = false;
        int damageToDeal = PlayerStatus.Attack;

        if (Random.Range(1, 101) <= PlayerStatus.CriticalHitChance)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
        }

        if (enemy.selectedBodyPart != null)
        {
            foreach (BodyPartType type in enemy.selectedBodyPart.data.type)
            {
                if (type == BodyPartType.Eyes)
                {
                    isEye = true;
                    damageToDeal *= 2;
                    break;
                }
            }
        }

        BattleManager.Instance.DamageEnemy(Mathf.CeilToInt(damageToDeal));
        BattleManager.Instance.UpdateState();

        string message = string.Empty;

        if (isCriticalHit)
        {
            message += $"Critical claw attack! {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} took {damageToDeal} damage!";
        }
        else if (isEye)
        {
            message += $"Gouged {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} for {damageToDeal} damage!";
        }
        else
        {
            message += $"{enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} for {damageToDeal} damage. Should've went for the eyes.";
        }

        message = BattleManager.Instance.ConcatEnemyDefeatedMessageIfTrue(message);
        BattleManager.Instance.ShowInfoText(message);
    }

    /// <summary>
    /// Apply poison to the enemy.
    /// </summary>
    public static void Poison()
    {
        Enemy enemy = BattleManager.Instance.CurrentEnemy;

        // TODO: Give feedback to player that enemy is immune to poison
        if (!enemy.Traits.ContainsKey(BodyPartTrait.Immunity))
        {
            enemy.IsPoisoned = true;
        }

        BattleManager.Instance.UpdateState();

        BattleManager.Instance.ShowInfoText($"Poisoned {enemy.name}! It will spread to every body part.");
    }

    /// <summary>
    /// Deal 1.5 times the damage you would normally deal in exchange for demon life.
    /// </summary>
    public static void Wrath()
    {
        Enemy enemy = BattleManager.Instance.CurrentEnemy;
        int amountRequired = 5;

        if (PlayerStatus.DemonLife - amountRequired <= 0)
        {
            BattleManager.Instance.ShowInfoText($"Not enough demon life force to use Wrath.");

            return;
        }

        BattleManager.Instance.SubtractDemonLife(amountRequired);
        float damageToDeal = (PlayerStatus.Attack + Random.Range(0, PlayerStatus.ExtraDamage + 1)) * 1.5f;
        bool isCriticalHit = Random.Range(1, 101) <= PlayerStatus.CriticalHitChance;

        if (isCriticalHit)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
        }

        BattleManager.Instance.DamageEnemy(Mathf.CeilToInt(damageToDeal));
        BattleManager.Instance.UpdateState();

        string message = string.Empty;

        if (isCriticalHit)
        {
            message +=
                $"Critical attack! {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} took {Mathf.CeilToInt(damageToDeal)} damage from your wrath!";
        }
        else
        {
            message +=
                $"You used Wrath against {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()}! Dealt {Mathf.CeilToInt(damageToDeal)} damage!";
        }

        message = BattleManager.Instance.ConcatEnemyDefeatedMessageIfTrue(message);
        BattleManager.Instance.ShowInfoText(message);
    }
}
