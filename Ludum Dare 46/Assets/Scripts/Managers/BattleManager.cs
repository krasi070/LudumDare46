using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public TextMeshProUGUI playerVitalityText;
    public TextMeshProUGUI demonMeterText;
    public GameObject buttonLayout;
    public TextMeshProUGUI info;
    public ShakeBehaviour cameraShake;

    private static Dictionary<EnemyType, Dictionary<string, int>> _enemiesTable; 

    private Enemy _enemy;

    public static BattleState State { get; private set; }

    private void Awake()
    {
        Cursor.visible = true;
    }

    private void Start()
    {
        State = BattleState.BattleStart;
        InitEnemiesTable();
        InitEnemy();
        SetUpPlayer();
        ShowInfoText($"Encountered {_enemy.name}! Steal a body part!");
    }

    private void Update()
    {
        ExecuteActionBasedOnState();
    }

    public void UpdateState()
    {
        switch (State)
        {
            case BattleState.BattleStart:
                State = BattleState.PlayerTurn;
                break;
            case BattleState.StartPlayerTurn:
                StartPlayerTurnActions();
                break;
            case BattleState.PlayerTurn:
                State = BattleState.EndPlayerTurn;
                break;
            case BattleState.EndPlayerTurn:
                EndPlayerTurnActions();
                break;
            case BattleState.StartEnemyTurn:
                StartEnemyTurnActions();
                break;
            case BattleState.EnemyTurn:
                State = BattleState.EndEnemyTurn;
                break;
            case BattleState.EndEnemyTurn:
                EndEnemyTurnActions();
                break;
            case BattleState.Win:
                State = BattleState.ChoiceMade;
                break;
            case BattleState.ChoiceMade:
                break;
            case BattleState.Lose:
                break;
        }
    }

    // PLAYER ACTIONS START HERE

    /// <summary>
    /// Deal damage equal to player attack + extra damage.
    /// </summary>
    public void Stab()
    {
        bool isCriticalHit = Random.Range(1, 101) <= PlayerStatus.CriticalHitChance;
        int damageToDeal = PlayerStatus.Attack + Random.Range(0, PlayerStatus.ExtraDamage + 1);

        if (isCriticalHit)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
            // TODO: Give feedback to player that attack was critical
        }

        AudioManager.instance.Play("Stab", true);
        DamageEnemy(Mathf.CeilToInt(damageToDeal));
        UpdateState();

        if (isCriticalHit)
        {
            ShowInfoText($"Critical stab! {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()} took <color=#CA0909>{damageToDeal}</color> damage!");
        }
        else
        {
            ShowInfoText($"You stabbed {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()} for <color=#CA0909>{damageToDeal}</color> damage!");
        }

        ShowTextIfEnemyIsDefeated();
    }

    /// <summary>
    /// 50% chance player will evade the next attack.
    /// </summary>
    public void Anticipate()
    {
        PlayerStatus.EvadeChance = 50;
        UpdateState();

        ShowInfoText($"Anticipating {_enemy.name}'s next attack...");
    }

    /// <summary>
    /// Deal twice the damage to left or right eyes.
    /// </summary>
    public void Claws()
    {
        bool isCriticalHit = Random.Range(1, 101) <= PlayerStatus.CriticalHitChance;
        bool isEye = false;
        int damageToDeal = PlayerStatus.Attack;

        if (Random.Range(1, 101) <= PlayerStatus.CriticalHitChance)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
        }

        if (_enemy.selectedBodyPart != null)
        {
            foreach (BodyPartType type in _enemy.selectedBodyPart.data.type)
            {
                if (type == BodyPartType.Eyes)
                {
                    isEye = true;
                    damageToDeal *= 2;
                    break;
                }
            }
        }

        DamageEnemy(Mathf.CeilToInt(damageToDeal));
        UpdateState();

        if (isCriticalHit)
        {
            ShowInfoText($"Critical claw attack! {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()} took {damageToDeal} damage!");
        }
        else if (isEye)
        {
            ShowInfoText($"Gouged {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()} for {damageToDeal} damage!");
        }
        else
        {
            ShowInfoText($"{_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()} for {damageToDeal} damage. Should've went for the eyes.");
        }

        ShowTextIfEnemyIsDefeated();
    }

    /// <summary>
    /// Apply poison to the enemy.
    /// </summary>
    public void Poison()
    {
        // TODO: Give feedback to player that enemy is immune to poison
        if (!_enemy.Traits.ContainsKey(BodyPartTrait.Immunity))
        {
            _enemy.IsPoisoned = true;
        }

        UpdateState();

        ShowInfoText($"Poisoned {_enemy.name}! It will spread to every body part.");
    }

    /// <summary>
    /// Deal 1.5 times the damage you would normally deal in exchange for demon meter.
    /// </summary>
    public void Wrath()
    {
        int amountRequired = 5;

        if (PlayerStatus.DemonLife - amountRequired <= 0)
        {
            ShowInfoText($"Not enough demon life force to use Wrath.");

            return;
        }

        PlayerStatus.DemonLife -= amountRequired;
        StartCoroutine(DemonLifeDamageEffect());
        UpdatePlayerUi();
        float damageToDeal = (PlayerStatus.Attack + Random.Range(0, PlayerStatus.ExtraDamage + 1)) * 1.5f;
        bool isCriticalHit = Random.Range(1, 101) <= PlayerStatus.CriticalHitChance;

        if (isCriticalHit)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
        }

        DamageEnemy(Mathf.CeilToInt(damageToDeal));
        UpdateState();

        if (isCriticalHit)
        {
            ShowInfoText($"Critical attack! {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()} took <color=#CA0909>{Mathf.CeilToInt(damageToDeal)}</color> damage from your wrath!");
        }
        else
        {
            ShowInfoText($"You used Wrath against {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()}! Dealt <color=#CA0909>{Mathf.CeilToInt(damageToDeal)}</color> damage!");
        }

        ShowTextIfEnemyIsDefeated();
    }

    // PLAYER ACTIONS END HERE

    private void ExecuteActionBasedOnState()
    {
        if (State == BattleState.PlayerTurn)
        {
            if (_enemy.selectedBodyPart == null)
            {
                ShowInfoText("Select a body part to steal!");
            }
            else
            {
                ShowActionButtons();
            }
        }

        if (State == BattleState.EnemyTurn)
        {
            int playerVitality = PlayerStatus.Vitality;
            string attackInfo = _enemy.Act();
            ShowInfoText(attackInfo);

            if (playerVitality > PlayerStatus.Vitality)
            {
                cameraShake.TriggerShake();
                StartCoroutine(TextDamageEffect(
                    playerVitalityText, 
                    string.Empty, 
                    PlayerStatus.Vitality.ToString(), 
                    $" / {PlayerStatus.MaxVitality}"));
            }
            else
            {
                UpdatePlayerUi();
            }
            
            UpdateState();
        }

        if (State == BattleState.Win && !GetComponent<AddBodyPartChoice>().bodyPartPanel.activeSelf)
        {
            _enemy.gameObject.SetActive(false);
            DestroyButtons();
            GetComponent<AddBodyPartChoice>().MakeChoice(_enemy.selectedBodyPart.data);
            ShowActionButtons();
        }

        if (State == BattleState.ChoiceMade)
        {
            buttonLayout.transform.parent.gameObject.SetActive(true);
            UpdatePlayerUi();
            ShowInfoText("On to the next one...");
        }

        if (State == BattleState.Lose)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void InitEnemiesTable()
    {
        _enemiesTable = new Dictionary<EnemyType, Dictionary<string, int>>();

        // Tree enemies
        _enemiesTable.Add(EnemyType.Tree, new Dictionary<string, int>());
        _enemiesTable[EnemyType.Tree].Add("Dead Sapling", 0);
    }

    private void InitEnemy()
    {
        GameObject enemyInstance = Instantiate(PlayerStatus.CurrentEnemy);
        _enemy = enemyInstance.GetComponent<Enemy>();
    }

    private void SetUpPlayer()
    {
        UpdatePlayerUi();
        DestroyButtons();

        // Stab
        GameObject stabButtonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/Buttons/StabButton"), buttonLayout.transform);

        stabButtonInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate { Stab(); });

        // Wrath
        GameObject wrathButtonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/Buttons/WrathButton"), buttonLayout.transform);

        wrathButtonInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate { Wrath(); });

        // Poison
        if (PlayerStatus.Traits.ContainsKey(BodyPartTrait.Poison))
        {
            GameObject poisonButtonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/Buttons/PoisonButton"), buttonLayout.transform);

            poisonButtonInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate { Poison(); });
        }

        // TODO: Add more trait related actions
    }

    private void UpdatePlayerUi()
    {
        playerVitalityText.text = $"{PlayerStatus.Vitality} / {PlayerStatus.MaxVitality}";
        demonMeterText.text = $"{Mathf.FloorToInt(PlayerStatus.DemonLife)}";
    }

    private void DestroyButtons()
    {
        foreach (Transform button in buttonLayout.transform)
        {
            Destroy(button.gameObject);
        }
    }

    private void ShowInfoText(string text)
    {
        buttonLayout.GetComponentInChildren<ActionButton>().HideTextBox();
        buttonLayout.SetActive(false);
        info.gameObject.SetActive(true);

        info.text = text;
    }

    private void ConcatInfoText(string toConcat)
    {
        info.text += toConcat;
    }

    private void ShowActionButtons()
    {
        buttonLayout.SetActive(true);
        info.gameObject.SetActive(false);
    }

    private void StartEnemyTurnActions()
    {
        if (!_enemy.IsAlive)
        {
            State = BattleState.Win;

            return;
        }

        if (_enemy.IsPoisoned)
        {
            int poisonDamageToDeal = PlayerStatus.Traits[BodyPartTrait.Poison];
            _enemy.TakeDamageAllBodyParts(poisonDamageToDeal);

            if (!_enemy.IsAlive)
            {
                State = BattleState.Win;

                return;
            }
        }

        State = BattleState.EnemyTurn;
    }

    private void StartPlayerTurnActions()
    {
        if (!PlayerStatus.IsAlive)
        {
            State = BattleState.Lose;

            return;
        }

        if (PlayerStatus.IsPoisoned)
        {
            int poisonDamageToDeal = _enemy.Traits[BodyPartTrait.Poison];
            PlayerStatus.Vitality -= poisonDamageToDeal;

            if (!PlayerStatus.IsAlive)
            {
                State = BattleState.Lose;

                return;
            }
        }

        State = BattleState.PlayerTurn;
    }

    private void EndEnemyTurnActions()
    {
        State = BattleState.StartPlayerTurn;
        UpdateState();
    }

    private void EndPlayerTurnActions()
    {
        State = BattleState.StartEnemyTurn;
        UpdateState();
    }

    private void DamageEnemy(int amount)
    {
        if (_enemy.selectedBodyPart != null)
        {
            _enemy.selectedBodyPart.vitality = Mathf.Max(_enemy.selectedBodyPart.vitality - amount, 0);
            _enemy.selectedBodyPart.ShowDamage(amount);
            StartCoroutine(TextDamageEffect(
                _enemy.uiText, 
                $"{_enemy.selectedBodyPart.data.name} ", 
                _enemy.selectedBodyPart.vitality.ToString()));
            _enemy.ShakeSprite();
        }
    }

    private void ShowTextIfEnemyIsDefeated()
    {
        if (!_enemy.IsAlive)
        {
            ConcatInfoText($"\nSuccessfully stole {_enemy.name}'s {_enemy.selectedBodyPart.data.name}. Where do you want to attach it?");
        }
    }

    private IEnumerator DemonLifeDamageEffect()
    {
        float originalfontSize = demonMeterText.fontSize;
        float currFontSize = originalfontSize;
        float increaseTo = originalfontSize * 1.5f;
        float increaseRate = 100f;
        bool increasing = true;

        while (increasing || currFontSize != originalfontSize)
        {
            if (increasing)
            {
                currFontSize = Mathf.Clamp(currFontSize + increaseRate * Time.deltaTime, originalfontSize, increaseTo);
                increasing = currFontSize < increaseTo;
            }
            else
            {
                currFontSize = Mathf.Clamp(currFontSize - increaseRate * Time.deltaTime, originalfontSize, increaseTo);
            }

            demonMeterText.text = $"<size={currFontSize}><color=#CA0909>{Mathf.FloorToInt(PlayerStatus.DemonLife)}</color></size>";

            yield return null;
        }

        demonMeterText.text = $"{Mathf.FloorToInt(PlayerStatus.DemonLife)}";
    }

    private IEnumerator TextDamageEffect(TextMeshProUGUI textField, string beforeText, string redText, string afterText = "")
    {
        float originalfontSize = textField.fontSize;
        float currFontSize = originalfontSize;
        float increaseTo = originalfontSize * 1.5f;
        float increaseRate = 100f;
        bool increasing = true;

        while (increasing || currFontSize != originalfontSize)
        {
            if (increasing)
            {
                currFontSize = Mathf.Clamp(currFontSize + increaseRate * Time.deltaTime, originalfontSize, increaseTo);
                increasing = currFontSize < increaseTo;
            }
            else
            {
                currFontSize = Mathf.Clamp(currFontSize - increaseRate * Time.deltaTime, originalfontSize, increaseTo);
            }

            textField.text = $"{beforeText}<size={currFontSize}><color=#CA0909>{redText}</color></size>{afterText}";

            yield return null;
        }

        textField.text = beforeText + redText + afterText;
    }
}
