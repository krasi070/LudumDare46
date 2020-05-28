using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public int vitality;
    public float demonLife;
    public float demonMeterDepletionRate;

    public TextMeshProUGUI demonLifeText;

    [HideInInspector]
    public bool isMoving;

    private void Awake()
    {
        Cursor.visible = false;

        if (MapStatus.PlayerPosition != null)
        {
            transform.position = MapStatus.PlayerPosition;
        }

        vitality = PlayerStatus.Vitality;
        demonLife = PlayerStatus.DemonMeter;
        demonMeterDepletionRate = PlayerStatus.DemonMeterDepletionRate;
        UpdateDemonMeter();
        StartCoroutine(HeartBeatEffect());
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

    public void UpdateDemonMeter()
    {
        demonLifeText.text = $"{Mathf.FloorToInt(demonLife)}";
    }

    public void PrepareForEncounter(GameObject enemy)
    {
        PlayerStatus.Vitality = vitality;
        PlayerStatus.DemonMeter = demonLife;
        PlayerStatus.DemonMeterDepletionRate = demonMeterDepletionRate;
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

    // These shouldn't be here...
    public void AddDemonLife(int toAdd)
    {
        StartCoroutine(AddDemonLifeEffect(toAdd));
    }

    public void ExecuteBloodDropletEffect()
    {
        StartCoroutine(BloodDropletEffect());
    }

    private IEnumerator HeartBeatEffect()
    {
        int speed = 20;
        float originalFontSize = demonLifeText.fontSize;
        float increaseTo = demonLifeText.fontSize;
        float multiplier = 2f;
        float amount = 0;

        while (true)
        {
            if (isMoving)
            {
                amount = (amount + speed * Time.deltaTime) % Mathf.PI;
                demonLifeText.fontSize = originalFontSize + multiplier * Mathf.Sin(amount);
            }
            else
            {
                demonLifeText.fontSize = originalFontSize;
                amount = 0;
            }

            yield return null;
        }
    }

    private IEnumerator BloodDropletEffect()
    {
        GameObject textInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Text/BloodDropletText"), demonLifeText.transform);
        TextMeshProUGUI text = textInstance.GetComponent<TextMeshProUGUI>();
        RectTransform rect = textInstance.GetComponent<RectTransform>();

        int alphaSpeed = 2;
        int movementSpeed = 150;
        float alpha = 1;

        while (text.color.a > 0)
        {
            alpha = Mathf.Clamp(alpha - alphaSpeed * Time.deltaTime, 0f, 1f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

            rect.anchoredPosition -= new Vector2(0f, movementSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(textInstance);
    }

    private IEnumerator AddDemonLifeEffect(int toAdd)
    {
        int added = 0;

        while (added < toAdd)
        {
            demonLifeText.text = $"{Mathf.FloorToInt(demonLife)}\n(+{toAdd - added})";
            demonLife++;
            added++;

            yield return new WaitForSeconds(0.1f);
        }

        demonLifeText.text = $"{Mathf.FloorToInt(demonLife)}";
    }
}
