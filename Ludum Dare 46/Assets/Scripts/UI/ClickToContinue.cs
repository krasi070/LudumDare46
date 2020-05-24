using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClickToContinue : MonoBehaviour, IPointerClickHandler
{
    private BattleManager _battleManager;

    private void Start()
    {
        _battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_battleManager.State == BattleState.EndPlayerTurn ||
            _battleManager.State == BattleState.EndEnemyTurn ||
            _battleManager.State == BattleState.Start)
        {
            _battleManager.UpdateState();
        }

        if (_battleManager.State == BattleState.ChoiceMade)
        {
            SceneManager.LoadScene("Map");
        }
    }
}
