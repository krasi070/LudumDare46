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

        if (isCriticalHit)
        {
            BattleManager.Instance.ShowInfoText(
                $"Critical stab! {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} took <color=#CA0909>{damageToDeal}</color> damage!");
        }
        else
        {
            BattleManager.Instance.ShowInfoText(
                $"You stabbed {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} for <color=#CA0909>{damageToDeal}</color> damage!");
        }

        BattleManager.Instance.ShowTextIfEnemyIsDefeated();
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

        if (isCriticalHit)
        {
            BattleManager.Instance.ShowInfoText(
                $"Critical claw attack! {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} took {damageToDeal} damage!");
        }
        else if (isEye)
        {
            BattleManager.Instance.ShowInfoText(
                $"Gouged {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} for {damageToDeal} damage!");
        }
        else
        {
            BattleManager.Instance.ShowInfoText(
                $"{enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} for {damageToDeal} damage. Should've went for the eyes.");
        }

        BattleManager.Instance.ShowTextIfEnemyIsDefeated();
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
    /// Deal 1.5 times the damage you would normally deal in exchange for demon meter.
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

        PlayerStatus.DemonLife -= amountRequired;
        BattleManager.Instance.SacrificeDemonLife(amountRequired);
        float damageToDeal = (PlayerStatus.Attack + Random.Range(0, PlayerStatus.ExtraDamage + 1)) * 1.5f;
        bool isCriticalHit = Random.Range(1, 101) <= PlayerStatus.CriticalHitChance;

        if (isCriticalHit)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
        }

        BattleManager.Instance.DamageEnemy(Mathf.CeilToInt(damageToDeal));
        BattleManager.Instance.UpdateState();

        if (isCriticalHit)
        {
            BattleManager.Instance.ShowInfoText(
                $"Critical attack! {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()} took <color=#CA0909>{Mathf.CeilToInt(damageToDeal)}</color> damage from your wrath!");
        }
        else
        {
            BattleManager.Instance.ShowInfoText(
                $"You used Wrath against {enemy.name}'s {enemy.selectedBodyPart.data.name.ToLower()}! Dealt <color=#CA0909>{Mathf.CeilToInt(damageToDeal)}</color> damage!");
        }

        BattleManager.Instance.ShowTextIfEnemyIsDefeated();
    }
}
