using UnityEngine;

public class RockController : MonoBehaviour
{
    private Rigidbody2D _rigidBody;

    public float _startingAngularVelocity;
    public float _torque;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.angularVelocity = -_startingAngularVelocity;
    }

    void FixedUpdate()
    {
        _rigidBody.AddTorque(-_torque);
    }
}