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
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");

        if (_player.demonMeter <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _direction * speed * Time.fixedDeltaTime);

        // Deplete demon meter every step
        if (_direction.x != 0 || _direction.y != 0)
        {
            _player.demonMeter -= _player.demonMeterDepletionRate * Time.fixedDeltaTime;
            _player.UpdateDemonMeter();
        }
    }
}
