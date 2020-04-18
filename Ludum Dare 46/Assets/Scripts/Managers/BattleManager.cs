using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private void Start()
    {
        // Test
        Debug.Log($"Vitality: {PlayerStatus.Vitality}");
        Debug.Log($"Demon meter: {PlayerStatus.DemonMeter}");
    }
}
