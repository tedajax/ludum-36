using UnityEngine;
using UnityEngine.UI;

public class JumpMeter : MonoBehaviour
{
    public Image _image;
    public CartController _cart;
    public float _dangerZone = 20f;

    void Update()
    {
        _image.fillAmount = Mathf.Clamp01(_cart._JumpCharge);
    }
}