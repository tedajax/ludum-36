using UnityEngine;

public class AvalancheController : MonoBehaviour
{
    public float _minSpeed = 5f;
    public float _maxSpeed = 9f;
    public float _distanceFactor = 35f;
    public float _startDistance = 40f;
    public Transform _tracked;
    public CameraController _camera;

    private Vector3 _spawnPosition;
    private bool _triggered = false;
    public float _Distance { get; private set; }

    void Awake()
    {
        _spawnPosition = transform.position;
    }

    void Update()
    {
        _Distance = 0f;
        if (_tracked != null) {
            _Distance = Mathf.Abs(_tracked.position.x - transform.position.x);
        }

        if (_Distance > _startDistance) {
            _triggered = true;
        }

        if (!_triggered) {
            return;
        }

        float t = Mathf.Clamp01(_Distance / _distanceFactor);
        float speed = Mathf.Lerp(_minSpeed, _maxSpeed, t);
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (_camera != null) {
            float shake = 1f - Mathf.Clamp01(_Distance / 24f);
            _camera._ShakeModifier = shake * 0.06f;
        }
    }

    public string GetDistanceString()
    {
        if (!_triggered) {
            return "-- m";
        }
        else {
            return string.Format("{0:F1}m", _Distance);
        }
    }

    public void Reset()
    {
        transform.position = _spawnPosition;
        _triggered = false;
    }
}