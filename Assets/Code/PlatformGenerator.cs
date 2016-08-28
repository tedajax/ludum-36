using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    public Transform _tracked;

    public GameObject _noRailPrefab;
    public GameObject _railPrefab;
    public GameObject _barrierPrefab;

    public int _platformLength = 30;
    public int _minRailLength = 3;
    public float _dirtChance = 0.3f;
    public float _barrierChance = 0.1f;
    public float _changeHeightChance = 0.05f;
    public int _minBlocksBeforeHeightChange = 10;
    public float _spacing = 1f;

    public float _destroyDistance = 10f;
    public float _createDistance;

    private List<GameObject> _gameObjects;

    private Transform _currentSpawnPoint;
    private int _blocksSinceHeightChange = 0;

    void Awake()
    {
        _gameObjects = new List<GameObject>();

        var spawnGameObject = new GameObject();
        _currentSpawnPoint = spawnGameObject.transform;
        _currentSpawnPoint.SetParent(transform, false);

        for (int i = 0; i < _platformLength; ++i) {
            AddBlock(false, false, false);
        }
    }

    void Update()
    {
        if (_tracked == null) {
            return;
        }

        for (int i = _gameObjects.Count - 1; i >= 0; --i) {
            var obj = _gameObjects[i];
            if ((obj.transform.position - _tracked.position).x < -_destroyDistance) {
                _gameObjects.RemoveAt(i);
                Destroy(obj);
            }
        }

        Debug.Log((_currentSpawnPoint.position - _tracked.position).x);
        if ((_currentSpawnPoint.position - _tracked.position).x < _createDistance) {
            GenerateBlock();
        }
    }

    void GenerateBlock()
    {
        if (Roll(_dirtChance)) {
            int size = Random.Range(1, 4);
            for (int i = 0; i < size; ++i) {
                AddBlock(true, false, true);
            }
        }
        else {
            AddBlock(false, Roll(_barrierChance), true);
        }
    }

    bool Roll(float chance)
    {
        return (Random.Range(0f, 1f) < chance);
    }

    void AddBlock(bool noRail, bool addBarrier, bool allowHeightChange)
    {
        if (Roll(_changeHeightChance) && _blocksSinceHeightChange >= _minBlocksBeforeHeightChange && allowHeightChange) {
            float direction = 1f;
            if (Random.Range(0, 2) == 1) {
                direction = -1f;
            }
            _currentSpawnPoint.position += transform.up * _spacing * direction;
            _blocksSinceHeightChange = 0;
        }

        var prefab = _railPrefab;
        if (noRail) {
            prefab = _noRailPrefab;
        }

        var position = _currentSpawnPoint.position;
        var groundObj = Instantiate(prefab, position, transform.rotation) as GameObject;
        _gameObjects.Add(groundObj);

        if (addBarrier) {
            var barrierPos = position + transform.up * -.2f;
            var barrierObj = Instantiate(_barrierPrefab, barrierPos, transform.rotation) as GameObject;
            _gameObjects.Add(barrierObj);
        }

        ++_blocksSinceHeightChange;
        _currentSpawnPoint.position += transform.right * _spacing;
    }
}