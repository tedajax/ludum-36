using UnityEngine;

public class BarrierController : MonoBehaviour
{
    Rigidbody2D[] _rigidBodies;

    void Awake()
    {
        _rigidBodies = GetComponentsInChildren<Rigidbody2D>();
    }

    public void BlastApart(float minAngle, float maxAngle, float force)
    {
        foreach (var body in _rigidBodies) {
            float angle = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;
            body.AddForce(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * force, ForceMode2D.Impulse);
            body.gameObject.layer = LayerMask.NameToLayer("BarrierDestroyed");
            body.gameObject.AddComponent<BarrierGibController>();
            body.transform.SetParent(null);
            body.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}