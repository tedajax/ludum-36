using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform _tracked = null;
    public float _smoothFactor = 5f;
    public Vector3 _offset = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;

    void FixedUpdate()
    {
        if (_tracked == null) {
            return;
        }

        float z = transform.position.z;
        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);
        position = Vector3.SmoothDamp(position, _tracked.transform.position + _offset, ref _velocity, _smoothFactor * Time.fixedDeltaTime);
        position.y = Mathf.Clamp(position.y, 0.5f, 100f);
        transform.position = new Vector3(position.x, position.y, z);
    }
}