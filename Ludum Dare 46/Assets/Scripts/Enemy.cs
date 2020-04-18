using UnityEngine;

public class Enemy : MonoBehaviour
{
    public BodyPart[] bodyParts;
    public BodyPart selectedBodyPart;

    public void UnselectBodyPart()
    {
        if (selectedBodyPart != null)
        {
            selectedBodyPart.Unselect();
        }
        else
        {
            Debug.LogError("Selected body part is null!");
        }
    }
}
