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
        FileInfo file = new FileInfo(Application.dataPath + "/" + fileName);
        try
        {
            using (StreamReader sr = new StreamReader(file.OpenRead(), Encoding.UTF8))
            {
                text = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);
        }
        return text;
    }
}
