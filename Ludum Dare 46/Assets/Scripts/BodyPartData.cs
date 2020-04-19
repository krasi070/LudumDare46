using UnityEngine;

[CreateAssetMenu(fileName = "New Body Part", menuName = "Body Part")]
public class BodyPartData : ScriptableObject
{
    public new string name;
    public BodyPartType[] type;
    public Sprite sprite;
    public int vitality;
    public int consumptionAmount;
    public int attachAmount;
    public BodyPartTrait[] traits;
}
