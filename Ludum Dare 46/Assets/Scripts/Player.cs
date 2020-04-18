using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float health;
    public float demonMeter;
    public float demonMeterDepletionRate;

    public Slider demonMeterSlider;

    public void UpdateDemonMeter()
    {
        demonMeterSlider.value = demonMeter;
    }
}
