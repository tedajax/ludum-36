using UnityEngine;

public class BarrierGibController : MonoBehaviour
{
    float _timer = 0f;

    SpriteRenderer _sprite;
    Rigidbody2D _rigidBody;

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_rigidBody.velocity.magnitude < 0.1f && _timer <= 0f) {
            _timer = 2f;
        }

        if (_timer > 0f) {
            _timer -= Time.deltaTime;

            if (_timer <= 0f) {
                Destroy(gameObject);
            }
            else if (_timer < 1f) {
                _sprite.color = new Color(1f, 1f, 1f, _timer);
            }
        }
    }
}