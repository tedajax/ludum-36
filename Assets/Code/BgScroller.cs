using UnityEngine;

public class BgScroller : MonoBehaviour
{
    public Camera _camera;
    public float _scrollRate;

    Material _material;

    void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        var position = transform.position;
        position.x = _camera.transform.position.x;
        transform.position = position;

        _material.SetTextureOffset("_MainTex", new Vector2(_camera.transform.position.x * _scrollRate, -0.01f));
    }
}