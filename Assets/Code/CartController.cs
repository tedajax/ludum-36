using UnityEngine;

public class CartController : MonoBehaviour
{
    public float _crankForce = 5f;
    public float _crankMaxAngle = 30f;
    public float _crankMinTurnRate = 90f;
    public float _crankMaxTurnRate = 540f;
    public float _crankDownForce = 2f;
    public float _downForce = 200f;
    public float _minJumpForce = 10f;
    public float _maxJumpForce = 200f;
    public float _maxJumpChargeTime = 4f;
    public float _airControlForce = 5f;

    public float _maxSpeed = 20f;

    public float _killY = -5f;

    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Rigidbody2D _leftWheel;
    [SerializeField] private Rigidbody2D _rightWheel;

    [SerializeField] private DudeController _leftDude;
    [SerializeField] private DudeController _rightDude;

    [SerializeField] private Transform _crankTransform;

    [SerializeField] private AudioSource _audioSource;
    public AudioClip _crankClip;
    public AudioClip _jumpClip;
    public AudioClip _deathClip;

    private float _speedRatio = 0f;

    private float _crankAngle = 0f;
    private float _prevCrankAngle = 0f;

    private float _jumpForceRequested;
    private float _jumpCharge;
    private bool _canJump = true;
    private float _jumpCooldown = 0f;
    private float _crankForceRequested;
    private float _horizontalAxis;
    private float _verticalAxis;

    private Vector3 _spawnPosition;

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

    public float _JumpCharge
    {
        get
        {
            return Mathf.Clamp01(_jumpCharge / _maxJumpChargeTime);
        }
    }

    public float _DistanceTravelled
    {
        get
        {
            return Mathf.Abs(transform.position.x - _spawnPosition.x);
        }
    }

    void Awake()
    {
        _spawnPosition = transform.position;
    }

    void Update()
    {
        _horizontalAxis = Input.GetAxis("Horizontal");
        _verticalAxis = Input.GetAxis("Vertical");

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

        if (_jumpCooldown > 0f) {
            _jumpCooldown -= Time.deltaTime;
        }

        if (Input.GetButton("Jump")) {
            _jumpCharge += Time.deltaTime;
        }
        else if (Input.GetButtonUp("Jump")) {
            float t = Mathf.Clamp01(_jumpCharge / _maxJumpChargeTime);
            _jumpForceRequested = Mathf.Lerp(_minJumpForce, _maxJumpForce, t);
        }

        if (transform.position.y < _killY) {
            GameController._Instance.Reset();
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

        if (_jumpCooldown <= 0f && onGround) {
            _canJump = true;
        }

        if (!Mathf.Approximately(_horizontalAxis, 0f) && !onGround) {
            float force = -_horizontalAxis * _airControlForce;
            _rigidBody.AddTorque(force);
        }

        if (_verticalAxis < 0f && !onGround) {
            _rigidBody.AddForce(Vector2.down * _downForce, ForceMode2D.Force);
        }

        if (_jumpForceRequested > 0f) {
            if (_canJump) {
                _rigidBody.AddForce(transform.up * _jumpForceRequested, ForceMode2D.Impulse);
                _canJump = false;
                _jumpCooldown = 0.1f;
            }
            _jumpForceRequested = 0f;
            _jumpCharge = 0f;
        }
    }

    bool OnGround()
    {
        bool leftOnGround = _leftWheel.IsTouchingLayers(1 << LayerMask.NameToLayer("Environment"));
        bool rightOnGround = _rightWheel.IsTouchingLayers(1 << LayerMask.NameToLayer("Environment"));
        return leftOnGround && rightOnGround;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Avalanche") {
            GameController._Instance.Reset();
        }
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

    public void Reset()
    {
        RigidBodyOff(_rigidBody);
        RigidBodyOff(_leftWheel);
        RigidBodyOff(_rightWheel);

        transform.position = _spawnPosition;
        transform.rotation = Quaternion.identity;
        _rigidBody.position = transform.position;
        _rigidBody.rotation = 0f;

        gameObject.SetActive(false);
        gameObject.SetActive(true);

        RigidBodyOn(_rigidBody);
        RigidBodyOn(_leftWheel);
        RigidBodyOn(_rightWheel);
    }

    void RigidBodyOff(Rigidbody2D body)
    {
        body.velocity = Vector2.zero;
        body.angularVelocity = 0f;
        body.isKinematic = true;
        body.gravityScale = 0f;
    }

    void RigidBodyOn(Rigidbody2D body)
    {
        body.isKinematic = false;
        body.gravityScale = 1f;
    }
}
