using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    private static Dictionary<string, KeyCode> _keyCodeDict = new Dictionary<string, KeyCode>()
    {
        {"Action", KeyCode.F},
        {"Forward", KeyCode.W},
        {"Backward", KeyCode.S},
        {"Left", KeyCode.A},
        {"Right", KeyCode.D},
        {"Jump", KeyCode.Space},
        {"Attack", KeyCode.Mouse0}
    };
    public static Dictionary<string, KeyCode> KeyCodeDict => _keyCodeDict;
    public static bool IsKeyPressed(KeyCode keyCode)
    {
        return Input.GetKey(keyCode);
    }

    // 操作方法などを変えたいときに使う
    public static KeyCode GetPressedKeyCode()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (IsKeyPressed(keyCode))
            {
                return keyCode;
            }
        }
        return KeyCode.None;
    }

    public static KeyCode GetKeyCode(string keyName)
    {
        if (!_keyCodeDict.ContainsKey(keyName))
        {
            return KeyCode.None;
        }
        return _keyCodeDict[keyName];
    }

    public static void ChangeKeyCode(string keyName, KeyCode keyCode)
    {
        if (!_keyCodeDict.ContainsKey(keyName))
        {
            return;
        }
        _keyCodeDict[keyName] = keyCode;
    }
}
