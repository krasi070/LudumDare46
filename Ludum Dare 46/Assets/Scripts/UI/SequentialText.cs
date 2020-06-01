using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Heavily taken from https://github.com/DanielMullinsGames/LD46/blob/master/Assets/Scripts/Base/Text/SequentialText.cs
public class SequentialText : MonoBehaviour
{
    public Color defaultColor = Color.black;
    public Color secondaryColor = new Color32(202, 9, 9, 255);
    public Image continueImage;

    private const float DEFAULT_FREQUENCY = 2f;

    private float _characterFrequency;
    private bool _skipToEnd;
    private Color _currentColor;
    private TextMeshProUGUI _textMesh;

    public bool IsPlayingMessage { get; private set; }

    public bool IsEndOfVisibleCharacters { get; private set; }

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _currentColor = defaultColor;
    }

    private void Update()
    {
        if (IsPlayingMessage && Input.GetMouseButtonDown(0))
        {
            SkipToEnd();
        }
    }

    public void PlayMessage(string message, bool showContinueImageWhenDone = true)
    {
        SetToDefaults();

        if (TextActive())
        {
            StartCoroutine(PlayMessageSequence(message, showContinueImageWhenDone));
        }
    }

    public void Clear()
    {
        StopAllCoroutines();
        IsPlayingMessage = false;
        continueImage.gameObject.SetActive(false);

        SetText(string.Empty);
    }

    public void SkipToEnd()
    {
        _skipToEnd = true;
    }

    public void SetText(string text)
    {
        if (_textMesh != null)
        {
            _textMesh.text = text;
        }
    }

    protected bool TextActive()
    {
        return _textMesh != null && _textMesh.gameObject.activeInHierarchy;
    }

    protected string GetText()
    {
        return _textMesh.text;
    }

    protected float GetFontSize()
    {
        return _textMesh.fontSize;
    }

    private void AppendText(string appendText)
    {
        SetText(GetText() + appendText);
    }

    private void SetToDefaults()
    {
        _currentColor = defaultColor;
        _characterFrequency = DEFAULT_FREQUENCY;
        _skipToEnd = false;
        continueImage.gameObject.SetActive(false);
    }

    private void FillTextBoxWithHiddenChars(string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            AppendText(ColorCharacter(message[i], new Color(0f, 0f, 0f, 0f)));
        }
    }

    private void ShowAllChars(string message)
    {
        Clear();

        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] >= '0' && message[i] <= '9')
            {
                AppendText(ColorCharacter(message[i], secondaryColor));
            }
            else
            {
                AppendText(ColorCharacter(message[i], _currentColor));
            }
        }
    }

    private int RemoveFirstHiddenChar()
    {
        int startIndex = GetText().IndexOf("<color=#00000000>");
        SetText(GetText().Remove(startIndex, 26));

        return startIndex;
    }

    public static string ColorString(string str, Color c)
    {
        string coloredString = "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + ">";
        coloredString += str;
        coloredString += "</color>";

        return coloredString;
    }

    public static string ColorCharacter(char character, Color c)
    {
        return ColorString(character.ToString(), c);
    }

    public static string SetStringFontSize(string str, float fontSize)
    {
        return $"<size={fontSize}>{str}</color>";
    }

    public static string SetCharacterFontSize(char character, float fontSize)
    {
        return SetStringFontSize(character.ToString(), fontSize);
    }

    private IEnumerator PlayMessageSequence(string message, bool showContinueImageWhenDone)
    {
        IsPlayingMessage = true;
        IsEndOfVisibleCharacters = false;
        string shown = string.Empty;

        SetText(shown);
        FillTextBoxWithHiddenChars(message);

        int index = 0;

        while (message.Length > index)
        {
            if (message[index].ToString() == " ")
            {
                yield return new WaitForSeconds(0.025f);
            }
            else
            {
                AudioManager.instance.Play("TextType", true);
            }

            shown += message[index];

            if (message == shown)
            {
                IsEndOfVisibleCharacters = true;
            }

            int insertIndex = RemoveFirstHiddenChar();
            string newCharacterWithColorCode;

            if (message[index] >= '0' && message[index] <= '9')
            {
                newCharacterWithColorCode = ColorCharacter(message[index], secondaryColor);
            }
            else
            {
                newCharacterWithColorCode = ColorCharacter(message[index], _currentColor);
            }

            SetText(GetText().Insert(insertIndex, newCharacterWithColorCode));
            index++;

            if (!_skipToEnd && index > 0 && (message[index - 1] == '.' || message[index - 1] == '?' || message[index - 1] == '!'))
            {
                yield return new WaitForSeconds(0.2f);
            }

            float adjustedFrequency = Mathf.Clamp(_characterFrequency * 0.01f, 0.01f, 0.2f);

            float waitTimer = 0f;

            while (!_skipToEnd && waitTimer < adjustedFrequency)
            {
                waitTimer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        IsPlayingMessage = false;

        if (showContinueImageWhenDone)
        {
            continueImage.gameObject.SetActive(true);
            StartCoroutine(RotateContinueImage());
            StartCoroutine(AnimateContinueImageCloud());
        }
    }

    private IEnumerator RotateContinueImage()
    {
        RectTransform rectTransform = continueImage.GetComponent<RectTransform>();
        Vector3 originalRotation = rectTransform.localEulerAngles;
        float speed = 4f;
        float valueForSin = 0f;

        while (continueImage.gameObject.activeSelf)
        {
            valueForSin = (valueForSin + Time.deltaTime * speed) % 360f;
            rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Sin(valueForSin) * 10);

            yield return null;
        }

        rectTransform.localEulerAngles = originalRotation;
    }

    private IEnumerator AnimateContinueImageCloud()
    {
        // Destroy any preexisting clouds
        foreach (Transform cloud in continueImage.transform)
        {
            Destroy(cloud.gameObject);
        }

        GameObject continueCloudPrefab = Resources.Load<GameObject>("Prefabs/UI/ContinueCloudImage");
        GameObject cloudInstance = Instantiate(continueCloudPrefab, continueImage.transform);
        Image cloudImage = cloudInstance.GetComponent<Image>();

        bool resetCloud = true;
        float speed = 0.75f;
        float alpha = 1f;
        float scale = 1f;

        while (continueImage.gameObject.activeSelf)
        {
            if (resetCloud)
            {
                alpha = 1f;
                cloudImage.color = new Color(cloudImage.color.r, cloudImage.color.g, cloudImage.color.b, alpha);

                scale = 1f;
                cloudInstance.transform.localScale = new Vector3(scale, scale, cloudInstance.transform.localScale.z);

                resetCloud = false;
            }

            alpha = Mathf.Clamp01(alpha - Time.deltaTime * speed);
            cloudImage.color = new Color(cloudImage.color.r, cloudImage.color.g, cloudImage.color.b, alpha);

            scale += Time.deltaTime * speed / 2.25f;
            cloudInstance.transform.localScale = new Vector3(scale, scale, cloudInstance.transform.localScale.z);

            if (alpha == 0f || scale >= 1.5f)
            {
                resetCloud = true;
            }

            yield return null;
        }

        Destroy(cloudInstance);
    }
}
