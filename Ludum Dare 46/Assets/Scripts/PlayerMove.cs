using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed;

    private Vector2 _direction;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _direction * speed * Time.fixedDeltaTime);
    }
}
