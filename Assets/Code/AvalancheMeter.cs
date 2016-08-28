using UnityEngine;
using UnityEngine.UI;

public class AvalancheMeter : MonoBehaviour
{
    public Image _image;
    public AvalancheController _avalanche;
    public float _dangerZone = 20f;

    void Update()
    {
        float d = _avalanche._Distance;
        float p = 1f - Mathf.Clamp01(d / _dangerZone);
        _image.fillAmount = p;
    }
}