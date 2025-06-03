using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonPlayerVariable : MonoBehaviour
{
    public bool isPlayerInRoom = false;
    public Collider currentRoomCollider = null;


    private Collider playerCollider;

    void Start()
    {
        playerCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            // 部屋に入った場合
            isPlayerInRoom = true;
            currentRoomCollider = other;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            // 部屋から出た場合
            isPlayerInRoom = false;
            currentRoomCollider = null;
        }
    }
}
