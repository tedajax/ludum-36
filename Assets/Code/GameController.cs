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

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void Reset()
    {
        bool goodDeath = false;
        if (_Score - _HighScore > 0.1f) {
            _HighScore = _Score;
            goodDeath = true;
        }

        _cart.Reset();
        _avalanche.Reset();
        _generator.Reset();

        if (goodDeath) {
            _cart.PlayGoodDeath();
        }
        else {
            _cart.PlayBadDeath();
        }

        Camera.main.GetComponent<CameraController>()._ShakeModifier = 0f;
    }
}