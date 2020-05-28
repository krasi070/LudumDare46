using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public new string name;
    public EnemyType type;

    public BodyPart[] bodyParts;
    public BodyPart selectedBodyPart;

    public TextMeshProUGUI uiText;

    public bool IsAlive
    {
        get
        {
            if (selectedBodyPart != null)
            {
                return selectedBodyPart.IsAlive;
            }

            return true;
        }
    }

    public bool IsPoisoned { get; set; }

    public Dictionary<BodyPartTrait, int> Traits { get; private set; }

    private void Start()
    {
        SetTraits();
    }

    public string Act()
    {
        // TODO: Implement enemy AI
        int randomDamage = Random.Range(1, 13);
        PlayerStatus.Vitality -= randomDamage;
        AudioManager.instance.Play("TreeAttack", true);

        if (!PlayerStatus.IsAlive)
        {
            return $"{name} attacks you for <color=#CA0909>{randomDamage}</color> damage.\n<color=#CA0909>LETHAL BLOW!</color>";
        }

        return $"{name} attacks you for <color=#CA0909>{randomDamage}</color> damage.";
    }

    public bool TakeDamageAllBodyParts(int amount)
    {
        foreach (BodyPart bodyPart in bodyParts)
        {
            bodyPart.vitality -= amount;
        }

        if (selectedBodyPart != null)
        {
            uiText.text = $"{selectedBodyPart.data.name} {selectedBodyPart.vitality}";
        }

        return selectedBodyPart != null && selectedBodyPart.vitality <= 0;
    }

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

    /// <summary>
    /// Show selected body part's UI.
    /// </summary>
    public void ShowBodyPartUi()
    {
        ShowBodyPartUi(selectedBodyPart);
    }

    public void ShowBodyPartUi(BodyPart bodyPart)
    {
        uiText.text = $"{bodyPart.data.name} {bodyPart.vitality}";
        uiText.gameObject.SetActive(true);
    }

    public void ShakeSprite()
    {
        StartCoroutine(SpriteShakeEffect());
    }

    private void SetTraits()
    {
        Traits = new Dictionary<BodyPartTrait, int>();

        if (bodyParts != null)
        {
            foreach (BodyPart bodyPart in bodyParts)
            {
                foreach (BodyPartTrait trait in bodyPart.data.traits)
                {
                    if (!Traits.ContainsKey(trait))
                    {
                        Traits.Add(trait, 0);
                    }

                    Traits[trait]++;
                }
            }
        }
        else
        {
            Debug.Log("Body parts are not set!");
        }
    }

    private IEnumerator SpriteShakeEffect()
    {
        Vector3 originalPosition = transform.position;
        Vector3 originalTextPosition = uiText.transform.position;
        float duration = 0.25f;
        float timer = 0f;
        float speed = 60f;
        float multiplier = 0.075f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            Vector3 moveBy = new Vector3(Mathf.Sin(speed * timer) * multiplier, 0f);
            transform.position = originalPosition + moveBy;
            uiText.transform.position = originalTextPosition;

            yield return null;
        }

        transform.position = originalPosition;
        uiText.transform.position = originalTextPosition;
    }
}
