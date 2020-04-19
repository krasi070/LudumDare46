using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public Slider playerVitalitySlider;
    public Slider demonMeterSlider;
    public GameObject buttonLayout;
    public TextMeshProUGUI info;

    private static Dictionary<EnemyType, Dictionary<string, int>> _enemiesTable; 

    private Enemy _enemy;

    public BattleState State { get; private set; }

    private void Awake()
    {
        Cursor.visible = true;
    }

    private void Start()
    {
        State = BattleState.Start;
        InitEnemiesTable();
        SetRandomEnemy();
        SetUpPlayer();
        ShowInfoText($"Encountered {_enemy.name}! Steal a body part!");
    }

    private void Update()
    {
        ExecuteActionBasedOnState();
    }

    public void UpdateState()
    {
        if (State == BattleState.Start)
        {
            State = BattleState.PlayerTurn;
        }
        else if (!PlayerStatus.IsAlive)
        {
            State = BattleState.Lose;
        }
        else if (!_enemy.IsAlive)
        {
            State = BattleState.Win;
        }
        else if (State == BattleState.PlayerTurn)
        {
            State = BattleState.EndPlayerTurn;
        }
        else if (State == BattleState.EndPlayerTurn)
        {
            State = BattleState.EnemyTurn;
            StartEnemyTurnActions();
        }
        else if (State == BattleState.EnemyTurn)
        {
            State = BattleState.EndEnemyTurn;
        }
        else if (State == BattleState.EndEnemyTurn)
        {
            State = BattleState.PlayerTurn;
            StartPlayerTurnActions();
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

        _enemy.TakeDamage(damageToDeal);
        UpdateState();

        if (isCriticalHit)
        {
            ShowInfoText($"Critical stab! {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()} took {damageToDeal} damage!");
        }
        else
        {
            ShowInfoText($"You stabbed {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()} for {damageToDeal} damage!");
        }
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
                if (type == BodyPartType.LeftEye || type == BodyPartType.RightEye)
                {
                    isEye = true;
                    damageToDeal *= 2;
                    break;
                }
            }
        }

        _enemy.TakeDamage(damageToDeal);
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

        if (PlayerStatus.DemonMeter - amountRequired <= 0)
        {
            ShowInfoText($"Not enough demon life force to use Wrath.");

            return;
        }

        PlayerStatus.DemonMeter -= amountRequired;
        UpdatePlayerUi();
        float damageToDeal = (PlayerStatus.Attack + Random.Range(0, PlayerStatus.ExtraDamage + 1)) * 1.5f;
        bool isCriticalHit = Random.Range(1, 101) <= PlayerStatus.CriticalHitChance;

        if (isCriticalHit)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
        }

        _enemy.TakeDamage(Mathf.CeilToInt(damageToDeal));
        UpdateState();

        if (isCriticalHit)
        {
            ShowInfoText($"Critical attack! {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()} took {Mathf.CeilToInt(damageToDeal)} damage from your wrath!");
        }
        else
        {
            ShowInfoText($"You used Wrath against {_enemy.name}'s {_enemy.selectedBodyPart.data.name.ToLower()}! Dealt {Mathf.CeilToInt(damageToDeal)} damage!");
        }
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
            string attackInfo = _enemy.Act();
            ShowInfoText(attackInfo);
            UpdatePlayerUi();
            UpdateState();
        }

        if (State == BattleState.Win)
        {
            SceneManager.LoadScene("Map");
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

    private void SetRandomEnemy()
    {
        PlayerStatus.EncountersWith[PlayerStatus.FightingWith]++;

        List<string> enemyNames = new List<string>();

        foreach (string name in _enemiesTable[PlayerStatus.FightingWith].Keys)
        {
            if (_enemiesTable[PlayerStatus.FightingWith][name] < PlayerStatus.EncountersWith[PlayerStatus.FightingWith])
            {
                enemyNames.Add(name);
            }
        }

        string enemyName = enemyNames[Random.Range(0, enemyNames.Count)];
        GameObject enemyInstance = Instantiate(
            Resources.Load<GameObject>($"Prefabs/{PlayerStatus.FightingWith}s/{enemyName}"), 
            Vector3.zero, 
            Quaternion.identity);
        _enemy = enemyInstance.GetComponent<Enemy>();
    }

    private void SetUpPlayer()
    {
        UpdatePlayerUi();

        if (PlayerStatus.Traits.ContainsKey(BodyPartTrait.Poison))
        {
            GameObject poisonButtonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/Buttons/PoisonButton"), buttonLayout.transform);

            poisonButtonInstance.GetComponent<Button>().onClick.AddListener(delegate { Poison(); });
        }

        // TODO: Add more trait related actions
    }

    private void UpdatePlayerUi()
    {
        playerVitalitySlider.maxValue = PlayerStatus.MaxVitality;
        playerVitalitySlider.value = PlayerStatus.Vitality;

        demonMeterSlider.maxValue = PlayerStatus.MaxDemonMeter;
        demonMeterSlider.value = PlayerStatus.DemonMeter;
    }

    private void ShowInfoText(string text)
    {
        buttonLayout.GetComponentInChildren<ActionButton>().HideTextBox();
        buttonLayout.SetActive(false);
        info.gameObject.SetActive(true);

        info.text = text;
    }

    private void ShowActionButtons()
    {
        buttonLayout.SetActive(true);
        info.gameObject.SetActive(false);
    }

    private void StartEnemyTurnActions()
    {
        if (_enemy.IsPoisoned)
        {
            int poisonDamageToDeal = PlayerStatus.Traits[BodyPartTrait.Poison];
            _enemy.TakeDamageAllBodyParts(poisonDamageToDeal);

            if (!_enemy.IsAlive)
            {
                State = BattleState.Win;
            }
        }
    }

    private void StartPlayerTurnActions()
    {
        if (PlayerStatus.IsPoisoned)
        {
            int poisonDamageToDeal = _enemy.Traits[BodyPartTrait.Poison];
            PlayerStatus.Vitality -= poisonDamageToDeal;

            if (!PlayerStatus.IsAlive)
            {
                State = BattleState.Lose;
            }
        }
    }
}
