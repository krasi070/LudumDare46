using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickToContinue : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!BattleManager.Instance.battleMessage.IsPlayingMessage)
        {
            switch (BattleManager.Instance.State)
            {
                case BattleState.BattleStart:
                case BattleState.StartPlayerTurn:
                case BattleState.EndPlayerTurn:
                case BattleState.StartEnemyTurn:
                case BattleState.EndEnemyTurn:
                    BattleManager.Instance.UpdateState();
                    break;
                case BattleState.ChoiceMade:
                    LevelManager.instance.LoadScene("Map");
                    break;
            }
        }
    }
}
