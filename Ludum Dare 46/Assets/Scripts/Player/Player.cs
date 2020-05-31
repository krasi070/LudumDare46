using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public DemonLifeUIHandler demonLifeUi;

    private void Awake()
    {
        Cursor.visible = false;

        if (MapStatus.PlayerPosition != null)
        {
            transform.position = MapStatus.PlayerPosition;
        }

        demonLifeUi.UpdateDemonLife();
    }

    private void Start()
    {
        if (PlayerStatus.BodyParts == null)
        {
            PlayerStatus.BodyParts = new Dictionary<BodyPartType, BodyPartData>();
        }

        if (PlayerStatus.Traits == null)
        {
            PlayerStatus.Traits = new Dictionary<BodyPartTrait, int>();
        }
    }

    public void PrepareForEncounter(GameObject enemy)
    {
        PlayerStatus.IsPoisoned = false;
        PlayerStatus.CurrentEnemy = enemy;

        foreach (BodyPartType bodyPart in PlayerStatus.BodyParts.Keys)
        {
            foreach (BodyPartTrait trait in PlayerStatus.BodyParts[bodyPart].traits)
            {
                if (!PlayerStatus.Traits.ContainsKey(trait))
                {
                    PlayerStatus.Traits.Add(trait, 0);
                }

                PlayerStatus.Traits[trait]++;
            }
        }
    }
}
