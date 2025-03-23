using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextToIntArray
{
    public static int[] Convert(string text)
    {
        string[] nums = text.Split(',');
        int[] intArray = new int[nums.Length];
        for (int i = 0; i < nums.Length; i++)
        {
            intArray[i] = int.Parse(nums[i].Trim());
        }
        return intArray;
    }

    public static List<int[]> ConvertFromFile(string fileName)
    {
        string text = FileReader.ReadFile(fileName);
        string[] lines = text.Split('\n');
        List<int[]> intList = new List<int[]>();
        for (int i = 0; i < lines.Length; i++)
        {
            intList.Add(Convert(lines[i]));
        }
        for (int i = 1; i < intList.Count; i++)
        {
            // すべての行が同じ長さであることを確認
            int firstIntListLen = intList[0].Length;
            if(intList[i].Length != firstIntListLen)
            {
                Debug.Log("The length of the first line is different from the length of the " + i + "th line.");
                return null;
            }
        }
        return intList;
    }
}
