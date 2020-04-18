using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Enemy enemy;

    private void Update()
    {
        if (enemy.selectedBodyPart != null)
        {
            Debug.Log(enemy.selectedBodyPart.data.name);
        }
    }
}
