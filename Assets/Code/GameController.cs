using UnityEngine;

public class GameController : MonoBehaviour
{
    public CartController _cart;
    public AvalancheController _avalanche;
    public PlatformGenerator _generator;

    public static GameController _Instance { get; private set; }

    public float _HighScore { get; private set; }
    public float _Score
    {
        get
        {
            return _cart._DistanceTravelled;
        }
    }

    void Awake()
    {
        _Instance = this;
        _HighScore = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            Reset();
        }
    }

    public void Reset()
    {
        if (_Score > _HighScore) {
            _HighScore = _Score;
        }

        _cart.Reset();
        _avalanche.Reset();
        _generator.Reset();
    }
}