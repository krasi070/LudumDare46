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
    private BattleState _state;

    private void Awake()
    {
        Cursor.visible = true;
    }

    private void Start()
    {
        _state = BattleState.Start;
        InitEnemiesTable();
        SetRandomEnemy();
        SetUpPlayer();
        ShowInfoText($"Encountered a {_enemy.type.ToString().ToLower()}! Steal a body part!");
    }

    private void Update()
    {
        ExecuteActionBasedOnState();
    }

    // PLAYER ACTIONS START HERE

    /// <summary>
    /// Deal damage equal to player attack + extra damage.
    /// </summary>
    public void Stab()
    {
        int damageToDeal = PlayerStatus.Attack + Random.Range(0, PlayerStatus.ExtraDamage + 1);

        if (Random.Range(1, 101) <= PlayerStatus.CriticalHitChance)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
            // TODO: Give feedback to player that attack was critical
        }

        _enemy.TakeDamage(damageToDeal);
        UpdateState();
    }

    /// <summary>
    /// 50% chance player will evade the next attack.
    /// </summary>
    public void Anticipate()
    {
        PlayerStatus.EvadeChance = 50;
        UpdateState();
    }

    /// <summary>
    /// Deal twice the damage to left or right eyes.
    /// </summary>
    public void Claws()
    {
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
                    damageToDeal *= 2;
                    break;
                }
            }
        }

        _enemy.TakeDamage(damageToDeal);
        UpdateState();
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
    }

    /// <summary>
    /// Deal 1.5 times the damage you would normally deal in exchange for demon meter.
    /// </summary>
    public void Wrath()
    {
        int amountRequired = 5;

        if (PlayerStatus.DemonMeter - amountRequired <= 0)
        {
            // Cannot activate action
            return;
        }

        PlayerStatus.DemonMeter -= amountRequired;
        UpdatePlayerUi();
        float damageToDeal = (PlayerStatus.Attack + Random.Range(0, PlayerStatus.ExtraDamage + 1)) * 1.5f;

        if (Random.Range(1, 101) <= PlayerStatus.CriticalHitChance)
        {
            damageToDeal *= PlayerStatus.CriticalHitMultiplier;
        }

        _enemy.TakeDamage(Mathf.CeilToInt(damageToDeal));
        UpdateState();
    }

    // PLAYER ACTIONS END HERE

    private void ExecuteActionBasedOnState()
    {
        if (_state == BattleState.Start && _enemy.selectedBodyPart != null)
        {
            _state = BattleState.PlayerTurn;
        }

        if (_state == BattleState.PlayerTurn)
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

        if (_state == BattleState.EnemyTurn)
        {
            _enemy.Act();
            UpdatePlayerUi();
            UpdateState();
        }

        if (_state == BattleState.Win)
        {
            SceneManager.LoadScene("Map");
        }

        if (_state == BattleState.Lose)
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
            Instantiate(Resources.Load<GameObject>("Prefabs/Buttons/Poison"), buttonLayout.transform);
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
            _enemy.TakeDamage(poisonDamageToDeal);

            if (!_enemy.IsAlive)
            {
                _state = BattleState.Win;
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
                _state = BattleState.Lose;
            }
        }
    }

    private void UpdateState()
    {
        if (!PlayerStatus.IsAlive)
        {
            _state = BattleState.Lose;
        }
        else if (!_enemy.IsAlive)
        {
            _state = BattleState.Win;
        }
        else if (_state == BattleState.PlayerTurn)
        {
            _state = BattleState.EnemyTurn;
            StartEnemyTurnActions();
        }
        else if (_state == BattleState.EnemyTurn)
        {
            _state = BattleState.PlayerTurn;
            StartPlayerTurnActions();
        }
    }
}
