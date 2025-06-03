using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;

    public void UpdateLevelText(int level)
    {
        _levelText.text = "Lv." + level;
    }
}
