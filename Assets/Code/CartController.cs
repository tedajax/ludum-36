using UnityEngine;

public class CartController : MonoBehaviour
{
    public float _crankForce = 5f;
    public float _crankMaxAngle = 30f;
    public float _crankTiltTime = 0.3f;
    public float _jumpForce = 10f;

    enum CrankState
    {
        Left,
        Right
    }

    [SerializeField] private Rigidbody2D _rigidBody;

    private CrankState _crankState = CrankState.Left;
    private CrankState _prevCrankState = CrankState.Left;
    [SerializeField] private Transform _crankTransform;
    private float _crankVelocity;

    void Awake()
    {
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal < 0f) {
            _crankState = CrankState.Left;
        }
        else if (horizontal > 0f) {
            _crankState = CrankState.Right;
        }

        bool requestJump = false;
        if (Input.GetButtonDown("Fire1")) {
            requestJump = true;
        }

        if (_prevCrankState != _crankState) {
            _prevCrankState = _crankState;
            _rigidBody.AddForce(Vector2.right * _crankForce, ForceMode2D.Impulse);
        }

        float _targetCrankAngle = 0f;
        if (_crankState == CrankState.Left) {
            _targetCrankAngle = _crankMaxAngle;
        }
        else {
            _targetCrankAngle = -_crankMaxAngle;
        }
        float crankAngle = _crankTransform.localRotation.eulerAngles.z;
        crankAngle = Mathf.SmoothDampAngle(crankAngle, _targetCrankAngle, ref _crankVelocity, _crankTiltTime * Time.deltaTime);
        _crankTransform.localRotation = Quaternion.AngleAxis(crankAngle, Vector3.forward);

        if (requestJump) {
            _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }
}
