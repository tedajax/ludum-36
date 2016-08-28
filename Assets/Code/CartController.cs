using UnityEngine;

public class CartController : MonoBehaviour
{
    public float _crankForce = 5f;
    public float _crankMaxAngle = 30f;
    public float _crankMinTurnRate = 90f;
    public float _crankMaxTurnRate = 540f;
    public float _crankDownForce = 2f;
    public float _jumpForce = 10f;
    public float _airControlForce = 5f;

    public float _maxSpeed = 20f;

    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Rigidbody2D _leftWheel;
    [SerializeField] private Rigidbody2D _rightWheel;

    [SerializeField] private DudeController _leftDude;
    [SerializeField] private DudeController _rightDude;

    [SerializeField] private Transform _crankTransform;

    private float _speedRatio = 0f;

    private float _crankAngle = 0f;
    private float _prevCrankAngle = 0f;

    private bool _jumpRquested;
    private float _crankForceRequested;
    private float _horizontalAxis;

    private float _CrankAngle
    {
        get
        {
            return _crankTransform.localRotation.eulerAngles.z;
        }
    }

    public float _Speed
    {
        get
        {
            return Mathf.Abs(_leftWheel.angularVelocity) * Mathf.Deg2Rad * _WheelRadius;
        }
    }

    private float _WheelRadius
    {
        get
        {
            return _leftWheel.GetComponent<CircleCollider2D>().radius;
        }
    }

    void Awake()
    {
    }

    void Update()
    {
        _horizontalAxis = Input.GetAxis("Horizontal");

        _prevCrankAngle = _crankAngle;

        float crankTurnRate = Mathf.Lerp(_crankMinTurnRate, _crankMaxTurnRate, _speedRatio);

        _crankAngle += crankTurnRate * -_horizontalAxis * Time.deltaTime;
        _crankAngle = Mathf.Clamp(_crankAngle, -_crankMaxAngle, _crankMaxAngle);
        _crankTransform.localRotation = Quaternion.AngleAxis(_crankAngle, Vector3.forward);

        if (_crankAngle > 0f) {
            _leftDude.Duck();
            _rightDude.Stand();
        }
        else if (_crankAngle < 0f) {
            _leftDude.Stand();
            _rightDude.Duck();
        }

        float crankDelta = _crankAngle - _prevCrankAngle;
        _crankForceRequested += crankDelta / _crankMaxAngle;

        if (Input.GetButtonDown("Jump")) {
            _jumpRquested = true;
        }
    }

    void FixedUpdate()
    {
        _speedRatio = _Speed / _maxSpeed;
        float forceRatio = 1f - _speedRatio;

        if (!Mathf.Approximately(_crankForceRequested, 0f)) {
            float force = Mathf.Abs(_crankForceRequested) * forceRatio;
            float torque = force * -_crankForce;
            _leftWheel.AddTorque(torque, ForceMode2D.Force);
            _rightWheel.AddTorque(torque, ForceMode2D.Force);

            Vector3 downPosition = _rightWheel.position;
            if (_crankForceRequested > 0f) {
                downPosition = _leftWheel.position;
            }
            _rigidBody.AddForceAtPosition(-transform.up * _crankDownForce * force, downPosition, ForceMode2D.Force);

            _crankForceRequested = 0f;
        }


        bool onGround = OnGround();

        if (!Mathf.Approximately(_horizontalAxis, 0f) && !onGround) {
            float force = -_horizontalAxis * _airControlForce;
            _rigidBody.AddTorque(force);
        }

        if (_jumpRquested && onGround) {
            _rigidBody.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
            _jumpRquested = false;
        }
    }

    bool OnGround()
    {
        bool leftOnGround = _leftWheel.IsTouchingLayers(1 << LayerMask.NameToLayer("Environment"));
        bool rightOnGround = _rightWheel.IsTouchingLayers(1 << LayerMask.NameToLayer("Environment"));
        return leftOnGround && rightOnGround;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Barrier") {
            var barrierController = collision.transform.GetComponentInParent<BarrierController>();
            if (barrierController != null) {
                float angle = transform.rotation.eulerAngles.z;
                barrierController.BlastApart(angle, angle + 45, _Speed);
            }
        }
    }
}
