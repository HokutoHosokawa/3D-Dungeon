using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;


public static class FileReader
{
    public static string ReadFile(string fileName)
    {
        string text = "";

        // UnityのResourcesフォルダからファイルを読み込む
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if (textAsset != null)
        {
            text = textAsset.text;
        }
        else
        {
            Debug.LogError($"File '{fileName}' not found in Resources folder.");
        }
        return text;
    }
}
