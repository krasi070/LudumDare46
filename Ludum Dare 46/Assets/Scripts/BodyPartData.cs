using UnityEngine;

[CreateAssetMenu(fileName = "New Body Part", menuName = "Body Part")]
public class BodyPartData : ScriptableObject
{
    public new string name;
    public BodyPartType[] type;
    public Sprite sprite;
    public Vector2 winScreenSize;
    // Separate for every body part type
    public GameObject[] demonMenuObject;
    public int vitality;
    public int consumptionAmount;
    public BodyPartTrait[] traits;
}
