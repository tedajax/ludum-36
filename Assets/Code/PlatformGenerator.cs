using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject _noRailPrefab;
    public GameObject _railPrefab;

    public int _platformLength = 30;
    public int _minRailLength = 3;
    public float _dirtChance = 0.3f;
    public float _spacing = 1f;

    void Awake()
    {
        for (int i = 0; i < _platformLength; ++i) {
            GameObject prefab = _railPrefab;
            if (Random.Range(0f, 1f) < _dirtChance) {
                prefab = _noRailPrefab;
            }
            Instantiate(prefab, transform.position + Vector3.right * _spacing * i, Quaternion.identity);
        }
    }
}