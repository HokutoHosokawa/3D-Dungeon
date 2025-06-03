using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearOrbEvent : EventTemplate
{
    public override void ActionEvent()
    {
        // ゲームのクリア画面に遷移する
        SceneManager.LoadScene("GameClearScene");
    }
}
