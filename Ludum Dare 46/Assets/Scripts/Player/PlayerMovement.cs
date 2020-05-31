using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public DemonLifeUIHandler demonLifeUi;

    private Vector2 _direction;
    private Rigidbody2D _rb;
    private Player _player;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!PlayerStatus.IsPaused)
        {
            _direction.x = Input.GetAxisRaw("Horizontal");
            _direction.y = Input.GetAxisRaw("Vertical");

            if (!PlayerStatus.IsAlive)
            {
                // TODO: Add transition
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _direction * speed * Time.fixedDeltaTime);
        PlayerStatus.IsMoving = _direction.x != 0 || _direction.y != 0;

        // Deplete demon meter every step
        if (PlayerStatus.IsMoving)
        {
            demonLifeUi.StopAddDemonLifeEffect();
            PlayerStatus.DemonLife -= PlayerStatus.DemonLifeDepletionRate * Time.fixedDeltaTime;

            int oldValue = int.Parse(demonLifeUi.demonLifeText.text);
            demonLifeUi.UpdateDemonLife();
            int newValue = int.Parse(demonLifeUi.demonLifeText.text);

            if (newValue < oldValue)
            {
                demonLifeUi.ExecuteBloodDropletEffect();
            }
        }
    }
}
