using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEnemyVariable : MonoBehaviour
{
    public bool isEnemyInRoom = false;
    public Collider currentRoomCollider = null;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            // 部屋に入った場合
            isEnemyInRoom = true;
            currentRoomCollider = other;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            // 部屋から出た場合
            isEnemyInRoom = false;
            currentRoomCollider = null;
        }
    }
}
