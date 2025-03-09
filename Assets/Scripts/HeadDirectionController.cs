using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadDirectionController : MonoBehaviour
{
    [SerializeField] private Transform head;    // プレイヤーの頭部
    [SerializeField] private GameObject player; // プレイヤー
    public float sensitivity = 50f;  // マウス感度
    public float maxYAngle = 50f;   // 上下回転の制限

    private float yaw = 0f;   // 左右の回転
    private float pitch = 0f; // 上下の回転

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * 100f * Time.deltaTime;

        // 左右の回転（首ごと動かす）
        yaw += mouseX;
        player.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // 上下の回転（頭だけ動かす）
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxYAngle, maxYAngle); // 上下の回転範囲を制限
    }

    private void LateUpdate() {
        head.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
