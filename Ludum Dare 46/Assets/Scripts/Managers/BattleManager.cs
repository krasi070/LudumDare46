using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public TextMeshProUGUI playerVitalityText;
    public TextMeshProUGUI demonLifeText;
    public GameObject buttonLayout;
    public SequentialText battleMessage;
    public ShakeBehaviour cameraShake;

    private bool _showingSelectBodyPartMessage = false;
    private bool _showingLastMessage = false;

    public static BattleManager Instance { get; private set; }

    public BattleState State { get; private set; }

    public Enemy CurrentEnemy { get; private set; }

    private void Awake()
    {
        Cursor.visible = true;

        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        State = BattleState.BattleStart;
        InitEnemy();
        SetUpPlayer();
        ShowInfoText($"Encountered {CurrentEnemy.name}! Steal a body part!");
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

    public void DamageEnemy(int amount)
    {
        if (CurrentEnemy.selectedBodyPart != null)
        {
            CurrentEnemy.selectedBodyPart.vitality = Mathf.Max(CurrentEnemy.selectedBodyPart.vitality - amount, 0);
            CurrentEnemy.selectedBodyPart.ShowDamage(amount);
            StartCoroutine(TextDamageEffect(
                CurrentEnemy.uiText,
                $"{CurrentEnemy.selectedBodyPart.data.name} ",
                CurrentEnemy.selectedBodyPart.vitality.ToString()));
            CurrentEnemy.ShakeSprite();
        }
    }

    public void ShowInfoText(string text, bool showContinueImageWhenDone = true)
    {
        buttonLayout.GetComponentInChildren<ActionButton>().HideTextBox();
        buttonLayout.SetActive(false);
        battleMessage.gameObject.SetActive(true);

        battleMessage.Clear();
        battleMessage.PlayMessage(text, showContinueImageWhenDone);
    }

    public string ConcatEnemyDefeatedMessageIfTrue(string message)
    {
        if (!CurrentEnemy.IsAlive)
        {
            return $"{message}\n{CurrentEnemy.GetDefeatedMessage()}";
        }

        return message;
    }

    public void AddDemonLife(int amount)
    {
        StartCoroutine(AddDemonLifeEffect(amount));
    }

    public void SubtractDemonLife(int amount)
    {
        StartCoroutine(SubtractDemonLifeEffect(amount));
    }

    public void UpdatePlayerUI()
    {
        playerVitalityText.text = $"{PlayerStatus.Vitality} / {PlayerStatus.MaxVitality}";
        demonLifeText.text = $"{Mathf.FloorToInt(PlayerStatus.DemonLife)}";
    }

    private void ExecuteActionBasedOnState()
    {
        if (State == BattleState.PlayerTurn)
        {
            if (CurrentEnemy.selectedBodyPart == null)
            {
                if (!_showingSelectBodyPartMessage)
                {
                    _showingSelectBodyPartMessage = true;
                    ShowInfoText("Select a body part to steal!", false);
                }
            }
            else
            {
                _showingSelectBodyPartMessage = false;
                ShowActionButtons();
            }
        }

        if (State == BattleState.EnemyTurn)
        {
            int playerVitality = PlayerStatus.Vitality;
            string attackInfo = CurrentEnemy.Act();
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
                UpdatePlayerUI();
            }
            
            UpdateState();
        }

        if (State == BattleState.Win && !GetComponent<AddBodyPartChoice>().bodyPartPanel.activeSelf)
        {
            CurrentEnemy.gameObject.SetActive(false);
            DestroyButtons();
            GetComponent<AddBodyPartChoice>().ShowOptionsAndBodyPart(CurrentEnemy.selectedBodyPart.data);
            ShowActionButtons();
        }

        if (State == BattleState.ChoiceMade)
        {
            if (!_showingLastMessage)
            {
                buttonLayout.transform.parent.gameObject.SetActive(true);
                UpdatePlayerUI();
                ShowInfoText("On to the next one...");
                _showingLastMessage = true;
            }
        }

        if (State == BattleState.Lose)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void InitEnemy()
    {
        GameObject enemyInstance = Instantiate(PlayerStatus.CurrentEnemy);
        CurrentEnemy = enemyInstance.GetComponent<Enemy>();
    }

    private void SetUpPlayer()
    {
        UpdatePlayerUI();
        DestroyButtons();

        // Stab
        GameObject stabButtonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/UI/Buttons/StabButton"), buttonLayout.transform);

        stabButtonInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate { PlayerActions.Stab(); });

        // Wrath
        GameObject wrathButtonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/UI/Buttons/WrathButton"), buttonLayout.transform);

        wrathButtonInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate { PlayerActions.Wrath(); });

        // Poison
        if (PlayerStatus.Traits.ContainsKey(BodyPartTrait.Poison))
        {
            GameObject poisonButtonInstance = Instantiate(
                Resources.Load<GameObject>("Prefabs/UI/Buttons/PoisonButton"), buttonLayout.transform);

            poisonButtonInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate { PlayerActions.Poison(); });
        }

        // TODO: Add more trait related actions
    }

    private void DestroyButtons()
    {
        foreach (Transform button in buttonLayout.transform)
        {
            Destroy(button.gameObject);
        }
    }

    private void ShowActionButtons()
    {
        buttonLayout.SetActive(true);
        battleMessage.gameObject.SetActive(false);
    }

    private void StartEnemyTurnActions()
    {
        if (!CurrentEnemy.IsAlive)
        {
            State = BattleState.Win;

            return;
        }

        if (CurrentEnemy.IsPoisoned)
        {
            int poisonDamageToDeal = PlayerStatus.Traits[BodyPartTrait.Poison];
            CurrentEnemy.TakeDamageAllBodyParts(poisonDamageToDeal);

            if (!CurrentEnemy.IsAlive)
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
            int poisonDamageToDeal = CurrentEnemy.Traits[BodyPartTrait.Poison];
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

    private IEnumerator DemonLifeDamageEffect()
    {
        float originalfontSize = demonLifeText.fontSize;
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

            demonLifeText.text = $"<size={currFontSize}><color=#CA0909>{Mathf.FloorToInt(PlayerStatus.DemonLife)}</color></size>";

            yield return null;
        }

        demonLifeText.text = $"{Mathf.FloorToInt(PlayerStatus.DemonLife)}";
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

    private IEnumerator AddDemonLifeEffect(int toAdd)
    {
        float overallTime = Mathf.Min(0.5f, toAdd * 0.1f);
        float waitTime = overallTime / toAdd;

        int added = 0;
        int demonLifeToDisplay = Mathf.FloorToInt(PlayerStatus.DemonLife);
        PlayerStatus.DemonLife += toAdd;

        while (added < toAdd)
        {
            demonLifeText.text = $"{demonLifeToDisplay} (+{toAdd - added})";
            demonLifeToDisplay++;
            added++;

            yield return new WaitForSeconds(waitTime);
        }

        UpdatePlayerUI();
    }

    private IEnumerator SubtractDemonLifeEffect(int toSubtract)
    {
        float overallTime = Mathf.Min(0.5f, toSubtract * 0.1f);
        float waitTime = overallTime / toSubtract;

        int subtracted = 0;
        int demonLifeToDisplay = Mathf.FloorToInt(PlayerStatus.DemonLife);
        PlayerStatus.DemonLife = Mathf.Max(0, PlayerStatus.DemonLife - toSubtract);

        while (subtracted < toSubtract)
        {
            demonLifeText.text = $"{demonLifeToDisplay} (-{toSubtract - subtracted})";
            demonLifeToDisplay = Mathf.Max(0, demonLifeToDisplay - 1);
            subtracted++;

            yield return new WaitForSeconds(waitTime);
        }

        UpdatePlayerUI();
    }
}
