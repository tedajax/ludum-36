using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    public Transform _tracked;

    public GameObject _noRailPrefab;
    public GameObject _railPrefab;
    public GameObject _centerPrefab;
    public GameObject _barrierPrefab;

    public int _platformLength = 30;
    public int _groundDepth = 4;
    public int _minRailLength = 3;
    public float _dirtChance = 0.3f;
    public float _barrierChance = 0.1f;
    public float _changeHeightChance = 0.05f;
    public float _gapChance = 0f;
    public int _maxGapSize = 5;
    public int _minBlocksBeforeGap = 6;
    public int _minBlocksBeforeHeightChange = 10;
    public float _spacing = 1f;
    public float _verticalSpacing = 0.33f;

    public float _destroyDistance = 10f;
    public float _createDistance;

    private List<GameObject> _gameObjects;

    private Transform _currentSpawnPoint;
    private int _blocksSinceHeightChange = 0;
    private int _blocksSinceGap = 0;

    void Awake()
    {
        _gameObjects = new List<GameObject>();

        var spawnGameObject = new GameObject();
        _currentSpawnPoint = spawnGameObject.transform;
        _currentSpawnPoint.SetParent(transform, false);

        for (int i = 0; i < _platformLength; ++i) {
            AddBlock(false, false, false, false);
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

        if ((_currentSpawnPoint.position - _tracked.position).x < _createDistance) {
            GenerateBlock();
        }
    }

    void GenerateBlock()
    {
        if (Roll(_dirtChance)) {
            int size = Random.Range(1, 4);
            for (int i = 0; i < size; ++i) {
                AddBlock(true, false, true, true);
            }
        }
        else {
            AddBlock(false, Roll(_barrierChance), true, true);
        }
    }

    bool Roll(float chance)
    {
        return (Random.Range(0f, 1f) < chance);
    }

    void AddBlock(bool noRail, bool addBarrier, bool allowHeightChange, bool allowGaps)
    {
        if (allowGaps && Roll(_gapChance) && _blocksSinceGap >= _minBlocksBeforeGap) {
            AddGap(Random.Range(1, _maxGapSize + 1));
            _blocksSinceGap = 0;

            if (Roll(_changeHeightChance) && _blocksSinceHeightChange >= _minBlocksBeforeHeightChange && allowHeightChange) {
                float direction = 1f;
                if (Random.Range(0, 2) == 1) {
                    direction = -1f;
                }
                _currentSpawnPoint.position += transform.up * _verticalSpacing * direction;
                _blocksSinceHeightChange = 0;
            }

            return;
        }

        

        var prefab = _railPrefab;
        if (noRail) {
            prefab = _noRailPrefab;
        }

        var position = _currentSpawnPoint.position;
        var groundObj = Instantiate(prefab, position, transform.rotation) as GameObject;
        _gameObjects.Add(groundObj);

        var underPosition = position + -transform.up * _spacing;
        for (int i = 0; i < _groundDepth; ++i) {
            var underGroundObj = Instantiate(_centerPrefab, underPosition, transform.rotation) as GameObject;
            _gameObjects.Add(underGroundObj);
            underPosition += -transform.up * _spacing;
        }

        if (addBarrier) {
            var barrierPos = position + transform.up * -.2f;
            var barrierObj = Instantiate(_barrierPrefab, barrierPos, transform.rotation) as GameObject;
            _gameObjects.Add(barrierObj);
        }

        ++_blocksSinceHeightChange;
        ++_blocksSinceGap;
        _currentSpawnPoint.position += transform.right * _spacing;
    }

    void AddGap(int blocks)
    {
        _currentSpawnPoint.position += transform.right * _spacing * blocks;
    }
}