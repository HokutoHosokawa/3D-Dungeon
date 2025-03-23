using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCanvasController : MonoBehaviour
{
    private Enemy _enemyStatus;
    [SerializeField] private TextMeshProUGUI _enemyNameText;

    // Start is called before the first frame update
    void Start()
    {
        _enemyStatus = GetComponent<EnemyStatus>().EnemyInfo;
        _enemyNameText.text = "Lv." + _enemyStatus.Level + ": " + gameObject.name.Replace("(Clone)", "");
    }

    void Update()
    {
        transform.Find("EnemyCanvas").LookAt(Camera.main.transform);
    }

}
