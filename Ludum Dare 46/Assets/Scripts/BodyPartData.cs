using UnityEngine;

[CreateAssetMenu(fileName = "New Body Part", menuName = "Body Part")]
public class BodyPartData : ScriptableObject
{
    public new string name;
    public BodyPartType[] type;
    public Sprite sprite;
    // Separate for every body part type
    public GameObject[] demonMenuObject;
    public int vitality;
    public int consumptionAmount;
    public int attachAmount;
    public BodyPartTrait[] traits;
}
