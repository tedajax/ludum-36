using UnityEngine;
using UnityEngine.UI;

public class SpeedMeter : MonoBehaviour
{
    public Image _image;
    public CartController _cartController;

    void Update()
    {
        float perc = _cartController._Speed / _cartController._maxSpeed;
        _image.fillAmount = perc;
    }
}
