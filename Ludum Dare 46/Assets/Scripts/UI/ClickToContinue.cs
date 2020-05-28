using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToContinue : MonoBehaviour, IPointerClickHandler
{
    private BattleManager _battleManager;

    private void Start()
    {
        _battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (BattleManager.State)
        {
            case BattleState.BattleStart:
            case BattleState.StartPlayerTurn:
            case BattleState.EndPlayerTurn:
            case BattleState.StartEnemyTurn:
            case BattleState.EndEnemyTurn:
            case BattleState.Win:
            case BattleState.Lose:
                _battleManager.UpdateState();
                break;
            case BattleState.ChoiceMade:
                LevelManager.instance.LoadScene("Map");
                break;
        }
    }
}
