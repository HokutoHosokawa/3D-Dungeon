using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewController : MonoBehaviour
{
    private Collider playerCollider;
    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<Collider>();
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.5f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            // 部屋に入った場合
            RenderSettings.fog = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            // 部屋から出た場合
            RenderSettings.fog = true;
        }
    }
}
