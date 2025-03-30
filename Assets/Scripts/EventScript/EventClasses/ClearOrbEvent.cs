using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearOrbEvent : EventTemplate
{
    public override void ActionEvent()
    {
        // ゲームのクリア画面に遷移する
        Debug.Log("Game Clear!");
    }
}
