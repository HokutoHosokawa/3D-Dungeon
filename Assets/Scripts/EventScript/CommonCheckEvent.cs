using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonCheckEvent
{
    private static readonly float CheckDistance = 1.0f;
    private static GameObject _player = GameObject.FindGameObjectWithTag("Player");

    public static bool IsPlayerCheckObject(GameObject targetObject)
    {
        if (!IsPlayerNearObject(targetObject))
        {
            return false;
        }
        if (!InputManager.IsKeyPressed(InputManager.GetKeyCode("Action")))
        {
            return false;
        }
        return true;
    }

    public static bool IsPlayerNearObject(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return false;
        }
        if (_player == null)
        {
            return false;
        }
        Vector3 targetObjectPosition = targetObject.transform.position;
        Vector3 targetObjectPositionXZ = new Vector3(targetObjectPosition.x, 0, targetObjectPosition.z);
        Vector3 playerPosition = _player.transform.position;
        Vector3 playerPositionXZ = new Vector3(playerPosition.x, 0, playerPosition.z);
        float distance = Vector3.Distance(targetObjectPositionXZ, playerPositionXZ);
        if (distance > CheckDistance)
        {
            return false;
        }
        return true;
    }
}
