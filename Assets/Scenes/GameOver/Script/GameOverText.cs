using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverText : MonoBehaviour
{
    private readonly float _TitleMoveTime = 0.5f;
    private readonly float _TitleFinalPositionY = 360f;
    private readonly float _ButtonFadeTime = 1.0f;
    private readonly float _ButtonFadeStartTime = 0.8f;
    private readonly float _ButtonFinalScale = 1.0f;
    [SerializeField] private TextMeshProUGUI _gameOverTitleText;
    [SerializeField] private Button _goTitleButton;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _exitButton;
    private float _time = 0.0f;
    private float _titleMoveSpeed;
    private float _buttonFadeSpeed;
    private bool _isFinishScale = false;
    private bool _isFinishMove = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameOverTitleText.transform.position = new Vector3(0, Screen.height / 2.0f + _gameOverTitleText.transform.localScale.y , 0);
        _goTitleButton.transform.localScale = new Vector3(0, 0, 0);
        _retryButton.transform.localScale = new Vector3(0, 0, 0);
        _exitButton.transform.localScale = new Vector3(0, 0, 0);
        _titleMoveSpeed = (_TitleFinalPositionY - _gameOverTitleText.transform.position.y) / _TitleMoveTime;
        _buttonFadeSpeed = _ButtonFinalScale / _ButtonFadeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isFinishScale && _time >= _ButtonFadeStartTime + _ButtonFadeTime)
        {
            _isFinishScale = true;
            _goTitleButton.transform.localScale = new Vector3(_ButtonFinalScale, _ButtonFinalScale, 0);
            _retryButton.transform.localScale = new Vector3(_ButtonFinalScale, _ButtonFinalScale, 0);
            _exitButton.transform.localScale = new Vector3(_ButtonFinalScale, _ButtonFinalScale, 0);
        }
        if(_time >= _ButtonFadeStartTime + _ButtonFadeTime)
        {
            return;
        }
        _time += Time.deltaTime;
        if (_time < _TitleMoveTime)
        {
            _gameOverTitleText.transform.position += new Vector3(0, _titleMoveSpeed * Time.deltaTime, 0);
        }
        else if (!_isFinishMove)
        {
            _gameOverTitleText.transform.position = new Vector3(0, _TitleFinalPositionY, 0);
            _isFinishMove = true;
        }
        if (_time >= _ButtonFadeStartTime && _time < _ButtonFadeStartTime + _ButtonFadeTime)
        {
            _goTitleButton.transform.localScale += new Vector3(_buttonFadeSpeed * Time.deltaTime, _buttonFadeSpeed * Time.deltaTime, 0);
            _retryButton.transform.localScale += new Vector3(_buttonFadeSpeed * Time.deltaTime, _buttonFadeSpeed * Time.deltaTime, 0);
            _exitButton.transform.localScale += new Vector3(_buttonFadeSpeed * Time.deltaTime, _buttonFadeSpeed * Time.deltaTime, 0);
        }
    }
}
