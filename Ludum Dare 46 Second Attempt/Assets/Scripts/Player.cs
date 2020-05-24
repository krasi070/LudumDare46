using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public float speed;

    private Vector2 _direction;
    private Rigidbody2D _rb;

    private Canvas _hoverUI;
    private TextMeshProUGUI _interactionTextMesh;

    private MapElement _interactionWith;
    private Interaction _interaction;

    public bool IsPaused { get; set; }

    public int Pennies { get; set; }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _hoverUI = GetComponentInChildren<Canvas>();
        _interactionTextMesh = _hoverUI.GetComponentInChildren<TextMeshProUGUI>();
        _interaction = GameObject.Find("Interaction").GetComponent<Interaction>();
        ShowText(false);
    }

    private void Update()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");

        if (_interactionWith != null && !IsPaused && Input.GetKeyDown(KeyCode.E))
        {
            Camera.main.transform.position = new Vector3(-100, 0f, Camera.main.transform.position.z);
            IsPaused = true;
            _interaction.Prepare(_interactionWith.sprite, _interactionWith.dialogue);
        }
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Interactable")
        {
            _interactionWith = collision.GetComponent<MapElement>();
            _interactionTextMesh.text = _interactionWith.interactionText;
            ShowText(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ShowText(false);
        _interactionWith = null;
    }

    private void ShowText(bool show)
    {
        _interactionTextMesh.enabled = show;
    }
}
