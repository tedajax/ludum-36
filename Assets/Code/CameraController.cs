using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform _tracked = null;
    public float _smoothFactor = 5f;
    public Vector3 _offset = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _shakeOffset = Vector3.zero;

    public float _ShakeModifier { get; set; }

    void Update()
    {
        float angle = Random.Range(0f, 360f);
        float x = Mathf.Cos(angle) * _ShakeModifier;
        float y = Mathf.Sin(angle) * _ShakeModifier;
        _shakeOffset = new Vector3(x, y, 0f);
    }

    void FixedUpdate()
    {
        if (_tracked == null) {
            return;
        }

        float z = transform.position.z;
        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);
        position = Vector3.SmoothDamp(position, _tracked.transform.position + _offset, ref _velocity, _smoothFactor * Time.fixedDeltaTime);
        position.y = Mathf.Clamp(position.y, 0.5f, 100f);
        position += _shakeOffset;
        transform.position = new Vector3(position.x, position.y, z);
    }
}