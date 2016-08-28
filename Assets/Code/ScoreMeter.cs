using UnityEngine;
using UnityEngine.UI;

public class ScoreMeter : MonoBehaviour
{
    public bool _isHighScore = false;
    public Text _text;

    void Update()
    {
        if (_isHighScore) {
            _text.text = string.Format("High: {0:F1}m", GameController._Instance._HighScore);
        }
        else {
            _text.text = string.Format("Score: {0:F1}m", GameController._Instance._Score);
        }
    }
}