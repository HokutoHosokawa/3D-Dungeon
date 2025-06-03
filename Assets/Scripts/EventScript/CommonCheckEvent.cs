using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CommonCheckEvent : MonoBehaviour
{
    private EventTemplate _eventTemplate;
    private bool _isPlayerNearObject = false;
    private TextMeshPro _textMeshPro;
    private Vector3 _baseTextPosition;

    private void Start()
    {
        _textMeshPro = transform.Find("CheckText").GetComponent<TextMeshPro>();
        _textMeshPro.text = "";
        _baseTextPosition = _textMeshPro.gameObject.transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _isPlayerNearObject = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _isPlayerNearObject = false;
        }
    }

    private void Update()
    {
        if (!_isPlayerNearObject)
        {
            _textMeshPro.text = "";
            return;
        }
        // プレイヤーが近くにいる場合
        // テキストに以下の内容を表示
        // "Press (ActionKey) to interact"
        _textMeshPro.text = "Press " + KeyCodeToString.KeyCodeConvertToString(InputManager.GetKeyCode("Action")) + " to interact";
        _textMeshPro.gameObject.transform.forward = Camera.main.transform.forward;
        _textMeshPro.gameObject.transform.localPosition = _textMeshPro.gameObject.transform.forward * _baseTextPosition.z + _textMeshPro.gameObject.transform.right * _baseTextPosition.x;
        _textMeshPro.gameObject.transform.localPosition = new Vector3(_textMeshPro.gameObject.transform.localPosition.x, _baseTextPosition.y, _textMeshPro.gameObject.transform.localPosition.z);
        if (!InputManager.IsKeyPressed(InputManager.GetKeyCode("Action")))
        {
            return;
        }
        if (_eventTemplate == null)
        {
            throw new System.ArgumentException("EventTemplate is not set.");
        }
        _eventTemplate.ActionEvent();
    }

    public void SetEventTemplate(EventTemplate eventTemplate)
    {
        if (_eventTemplate != null)
        {
            throw new System.ArgumentException("EventTemplate is already set.");
        }
        _eventTemplate = eventTemplate;
    }
}
