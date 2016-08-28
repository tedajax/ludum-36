using UnityEngine;
using UnityEngine.UI;

public class Bouncer : MonoBehaviour
{
    RectTransform _rectTransform;
    float _baseHeight;

    public float _amount = 2f;
    public float _duration = 3f;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _baseHeight = _rectTransform.position.y;
    }

    void Update()
    {
        float a = Mathf.Sin((Time.time * Mathf.PI * 2f) / _duration) * _amount;
        var pos = _rectTransform.position;
        pos.y = _baseHeight + a;
        _rectTransform.position = pos;
    }
}