using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HealObjectMove : MonoBehaviour
{
    private readonly float UpAndDownAmp = 0.3f;
    private readonly float UpAndDownCycleTime = 2.0f;
    private readonly float RotateSpeed = 90.0f;
    private float upAndDownTime = 0.0f;
    private float rotateTime = 0.0f;
    private float baseY = 0.0f;
    private void Start()
    {
        baseY = transform.localPosition.y;
    }

    void Update()
    {
        upAndDownTime += Time.deltaTime;
        rotateTime += Time.deltaTime;
        if (upAndDownTime > UpAndDownCycleTime)
        {
            upAndDownTime -= UpAndDownCycleTime;
        }
        float rotateCycleTime = 360.0f / RotateSpeed;
        if (rotateTime > rotateCycleTime)
        {
            rotateTime -= rotateCycleTime;
        }
        Vector3 position = transform.localPosition;
        position.y = baseY + GetUpAndDownY();
        transform.localPosition = position;
        transform.localRotation = Quaternion.Euler(0.0f, GetRotateY(), 0.0f);
    }

    private float GetUpAndDownY()
    {
        return (Mathf.Sin(upAndDownTime / UpAndDownCycleTime * Mathf.PI * 2.0f) * UpAndDownAmp + UpAndDownAmp) / 2.0f;
    }

    private float GetRotateY()
    {
        return rotateTime * RotateSpeed;
    }
}
