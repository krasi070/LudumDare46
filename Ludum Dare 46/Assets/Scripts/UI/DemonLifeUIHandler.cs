using System.Collections;
using UnityEngine;
using TMPro;

public class DemonLifeUIHandler : MonoBehaviour
{
    public TextMeshProUGUI demonLifeText;

    private Coroutine _lastAddDemonLifeCoroutine;

    private void Awake()
    {
        StartCoroutine(HeartBeatEffect());
    }

    public void UpdateDemonLife()
    {
        demonLifeText.text = $"{Mathf.FloorToInt(PlayerStatus.DemonLife)}";
    }

    public void AddDemonLife(int toAdd)
    {
        if (_lastAddDemonLifeCoroutine != null)
        {
            StopCoroutine(_lastAddDemonLifeCoroutine);
        }

        _lastAddDemonLifeCoroutine = StartCoroutine(AddDemonLifeEffect(toAdd));
    }

    public void StopAddDemonLifeEffect()
    {
        if (_lastAddDemonLifeCoroutine != null)
        {
            StopCoroutine(_lastAddDemonLifeCoroutine);
            UpdateDemonLife();
            _lastAddDemonLifeCoroutine = null;
        }
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
            if (PlayerStatus.IsMoving)
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
        GameObject textInstance = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Text/BloodDropletText"), demonLifeText.transform);
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
        int demonLifeToDisplay = Mathf.FloorToInt(PlayerStatus.DemonLife);
        PlayerStatus.DemonLife += toAdd;

        while (added < toAdd)
        {
            demonLifeText.text = $"{demonLifeToDisplay}\n(+{toAdd - added})";
            demonLifeToDisplay++;
            added++;

            yield return new WaitForSeconds(0.1f);
        }

        UpdateDemonLife();
        _lastAddDemonLifeCoroutine = null;
    }
}
