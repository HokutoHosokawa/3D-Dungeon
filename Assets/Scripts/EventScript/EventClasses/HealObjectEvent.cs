using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealObjectEvent : EventTemplate
{
    public override void ActionEvent()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus == null)
        {
            Debug.LogError("PlayerStatus component not found on player object.");
            return;
        }
        Player playerInfo = playerStatus.PlayerInfo;
        if (playerInfo == null)
        {
            Debug.LogError("PlayerInfo is null.");
            return;
        }
        playerInfo.Heal(playerInfo.HP.MaxHitPoint);
    }
}
