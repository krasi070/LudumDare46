using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public float speed;

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

            if (_player.demonLife <= 0)
            {
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _direction * speed * Time.fixedDeltaTime);
        _player.isMoving = _direction.x != 0 || _direction.y != 0;

        // Deplete demon meter every step
        if (_player.isMoving)
        {
            _player.demonLife -= _player.demonMeterDepletionRate * Time.fixedDeltaTime;

            int oldValue = int.Parse(_player.demonLifeText.text);
            _player.UpdateDemonMeter();
            int newValue = int.Parse(_player.demonLifeText.text);

            if (newValue < oldValue)
            {
                _player.ExecuteBloodDropletEffect();
            }
        }
    }
}
