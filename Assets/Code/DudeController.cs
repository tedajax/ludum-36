using UnityEngine;

public class DudeController : MonoBehaviour
{
    public Sprite _upSprite;
    public Sprite _downSprite;

    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Stand()
    {
        _spriteRenderer.sprite = _upSprite;
    }

    public void Duck()
    {
        _spriteRenderer.sprite = _downSprite;
    }
}