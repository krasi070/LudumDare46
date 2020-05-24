using UnityEngine;
using TMPro;

public class Interaction : MonoBehaviour
{
    public TextMeshProUGUI speechTextMesh;
    public GameObject optionContainer;

    private int _selectedOption;
    private int _currDialogueId;

    private Player _player;
    private Dialogue _dialogue;
    private SpriteRenderer _renderer;

    private void Start()
    {
        speechTextMesh.text = string.Empty;
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_player.IsPaused)
        {
            SelectOption();
        }
    }

    public void Prepare(Sprite characterSprite, Dialogue dialogue)
    {
        _renderer.sprite = characterSprite;
        _dialogue = dialogue;
        _currDialogueId = 0;
        ShowSpeech();
        ShowOptions();
    }

    private void ShowSpeech()
    {
        speechTextMesh.text = _dialogue.dialogue[_currDialogueId].speech;
    }

    private void ShowOptions()
    {
        optionContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
            optionContainer.GetComponent<RectTransform>().sizeDelta.x, 
            _dialogue.dialogue[_currDialogueId].options.Length * 25);

        _selectedOption = 0;

        for (int i = 0; i < _dialogue.dialogue[_currDialogueId].options.Length; i++)
        {
            DialogueOption option = _dialogue.dialogue[_currDialogueId].options[i];
            TextMeshProUGUI optionTextMesh = Instantiate(Resources.Load<TextMeshProUGUI>("UI/Option"), optionContainer.transform);

            if (i == _selectedOption)
            {
                optionTextMesh.text = $"> {option.text}";
            }
            else
            {
                optionTextMesh.text = option.text;
            }
        }
    }

    private void SelectOption()
    {
        int currOption = _selectedOption;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _selectedOption =
                (_dialogue.dialogue[_currDialogueId].options.Length + (_selectedOption - 1)) %
                _dialogue.dialogue[_currDialogueId].options.Length;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _selectedOption = (_selectedOption + 1) % _dialogue.dialogue[_currDialogueId].options.Length;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            NextDialogueSection();
        }

        if (_selectedOption != currOption)
        {
            for (int i = 0; i < _dialogue.dialogue[_currDialogueId].options.Length; i++)
            {
                if (i == _selectedOption)
                {
                    optionContainer.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text =
                        $"> {_dialogue.dialogue[_currDialogueId].options[i].text}";
                }
                else
                {
                    optionContainer.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text =
                        _dialogue.dialogue[_currDialogueId].options[i].text;
                }
            }
        }
    }

    private void NextDialogueSection()
    {
        _currDialogueId = _dialogue.dialogue[_currDialogueId].options[_selectedOption].nextId;
        DestroyOptions();

        if (_currDialogueId < 0)
        {
            speechTextMesh.text = string.Empty;
            _player.IsPaused = false;

            return;
        }

        ShowSpeech();
        ShowOptions();
    }

    private void DestroyOptions()
    {
        foreach (Transform child in optionContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
