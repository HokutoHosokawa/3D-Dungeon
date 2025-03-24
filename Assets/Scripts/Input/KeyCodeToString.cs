using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyCodeToString
{
    public static string KeyCodeConvertToString(KeyCode key)
    {
        string keyString = key.ToString();
        if (keyString.Contains("Alpha"))
        {
            return keyString.Replace("Alpha", "");
        }
        if (keyString == KeyCode.Delete.ToString())
        {
            return "Del";
        }
        if (keyString == KeyCode.Return.ToString())
        {
            return "Enter";
        }
        if (keyString == KeyCode.Escape.ToString())
        {
            return "Esc";
        }
        if (keyString.Contains("Keypad"))
        {
            string modifiedKeyString = keyString.Replace("Keypad", "");
            if (modifiedKeyString.Length == 1)
            {
                return modifiedKeyString;
            }
            if (modifiedKeyString == "Period")
            {
                return ".";
            }
            if (modifiedKeyString == "Divide")
            {
                return "/";
            }
            if (modifiedKeyString == "Multiply")
            {
                return "*";
            }
            if (modifiedKeyString == "Minus")
            {
                return "-";
            }
            if (modifiedKeyString == "Plus")
            {
                return "+";
            }
            if (modifiedKeyString == "Enter")
            {
                return "Enter";
            }
            if (modifiedKeyString == "Equals")
            {
                return "=";
            }
        }
        if (keyString == KeyCode.UpArrow.ToString())
        {
            return "↑";
        }
        if (keyString == KeyCode.DownArrow.ToString())
        {
            return "↓";
        }
        if (keyString == KeyCode.LeftArrow.ToString())
        {
            return "←";
        }
        if (keyString == KeyCode.RightArrow.ToString())
        {
            return "→";
        }
        if (keyString == KeyCode.Insert.ToString())
        {
            return "Ins";
        }
        if (keyString == KeyCode.PageUp.ToString())
        {
            return "PgUp";
        }
        if (keyString == KeyCode.PageDown.ToString())
        {
            return "PgDn";
        }
        if (keyString ==KeyCode.Exclaim.ToString())
        {
            return "!";
        }
        if (keyString == KeyCode.DoubleQuote.ToString())
        {
            return "\"";
        }
        if (keyString == KeyCode.Hash.ToString())
        {
            return "#";
        }
        if (keyString == KeyCode.Dollar.ToString())
        {
            return "$";
        }
        if (keyString == KeyCode.Percent.ToString())
        {
            return "%";
        }
        if (keyString == KeyCode.Ampersand.ToString())
        {
            return "&";
        }
        if (keyString == KeyCode.Quote.ToString())
        {
            return "'";
        }
        if (keyString == KeyCode.LeftParen.ToString())
        {
            return "(";
        }
        if (keyString == KeyCode.RightParen.ToString())
        {
            return ")";
        }
        if (keyString == KeyCode.Asterisk.ToString())
        {
            return "*";
        }
        if (keyString == KeyCode.Plus.ToString())
        {
            return "+";
        }
        if (keyString == KeyCode.Comma.ToString())
        {
            return ",";
        }
        if (keyString == KeyCode.Minus.ToString())
        {
            return "-";
        }
        if (keyString == KeyCode.Period.ToString())
        {
            return ".";
        }
        if (keyString == KeyCode.Slash.ToString())
        {
            return "/";
        }
        if (keyString == KeyCode.Colon.ToString())
        {
            return ":";
        }
        if (keyString == KeyCode.Semicolon.ToString())
        {
            return ";";
        }
        if (keyString == KeyCode.Less.ToString())
        {
            return "<";
        }
        if (keyString == KeyCode.Equals.ToString())
        {
            return "=";
        }
        if (keyString == KeyCode.Greater.ToString())
        {
            return ">";
        }
        if (keyString == KeyCode.Question.ToString())
        {
            return "?";
        }
        if (keyString == KeyCode.At.ToString())
        {
            return "@";
        }
        if (keyString == KeyCode.LeftBracket.ToString())
        {
            return "[";
        }
        if (keyString == KeyCode.Backslash.ToString())
        {
            return "\\";
        }
        if (keyString == KeyCode.RightBracket.ToString())
        {
            return "]";
        }
        if (keyString == KeyCode.Caret.ToString())
        {
            return "^";
        }
        if (keyString == KeyCode.Underscore.ToString())
        {
            return "_";
        }
        if (keyString == KeyCode.BackQuote.ToString())
        {
            return "`";
        }
        if (keyString == KeyCode.LeftCurlyBracket.ToString())
        {
            return "{";
        }
        if (keyString == KeyCode.Pipe.ToString())
        {
            return "|";
        }
        if (keyString == KeyCode.RightCurlyBracket.ToString())
        {
            return "}";
        }
        if (keyString == KeyCode.Tilde.ToString())
        {
            return "~";
        }
        if (keyString == KeyCode.RightShift.ToString())
        {
            return "RShift";
        }
        if (keyString == KeyCode.LeftShift.ToString())
        {
            return "LShift";
        }
        if (keyString == KeyCode.RightControl.ToString())
        {
            return "RCtrl";
        }
        if (keyString == KeyCode.LeftControl.ToString())
        {
            return "LCtrl";
        }
        if (keyString == KeyCode.RightAlt.ToString())
        {
            return "RAlt";
        }
        if (keyString == KeyCode.LeftAlt.ToString())
        {
            return "LAlt";
        }
        if (keyString == KeyCode.RightCommand.ToString())
        {
            return "RCmd";
        }
        if (keyString == KeyCode.LeftCommand.ToString())
        {
            return "LCmd";
        }
        if (keyString == KeyCode.LeftWindows.ToString())
        {
            return "LWin";
        }
        if (keyString == KeyCode.RightWindows.ToString())
        {
            return "RWin";
        }
        if (keyString == KeyCode.Mouse0.ToString())
        {
            return "LClick";
        }
        if (keyString == KeyCode.Mouse1.ToString())
        {
            return "RClick";
        }
        if (keyString == KeyCode.Mouse2.ToString())
        {
            return "WheelClick";
        }
        return keyString;
    }
}
