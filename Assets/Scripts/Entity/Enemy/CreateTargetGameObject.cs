using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class CreateTargetGameObject
{
    readonly static float MaxTargetDistanceForAxis = 5.0f;
    private static int[,] _map = GameObject.Find("CreateDungeonObject").GetComponent<CreateFloor>().GetCurrentFloorManagement().CreateDungeon.Map;

    public static GameObject CreateTarget(GameObject mainGameObject){
        CommonEnemyVariable commonEnemyVariable = mainGameObject.GetComponent<CommonEnemyVariable>();
        if(commonEnemyVariable == null){
            Debug.LogError("CommonEnemyVariableがアタッチされていません");
            return null;
        }
        if(commonEnemyVariable.isEnemyInRoom){
            // 部屋にいる場合
            return CreateTargetGameObjectInRoom(mainGameObject);
        }
        // 通路にいる場合
        return CreateTargetGameObjectInPath(mainGameObject);
    }

    private static GameObject CreateTargetGameObjectInRoom(GameObject mainGameObject)
    {
        CommonEnemyVariable commonEnemyVariable = mainGameObject.GetComponent<CommonEnemyVariable>();
        if(commonEnemyVariable == null){
            Debug.LogError("CommonEnemyVariableがアタッチされていません");
            return null;
        }
        if(!commonEnemyVariable.isEnemyInRoom){
            Debug.LogError("引数のゲームオブジェクトが部屋にいません");
            return null;
        }
        // 最初に自分を中心とした(2MaxTargetDistanceForAxis+1)×(2MaxTargetDistanceForAxis+1)の範囲内の移動可能な座標を求め、その中からランダムに選択する
        Vector2Int mainGameObjectPosition = new Vector2Int(Mathf.FloorToInt((mainGameObject.transform.position.x + CommonConst.FloorWidth / 2f) / CommonConst.FloorWidth), Mathf.FloorToInt((mainGameObject.transform.position.z + CommonConst.FloorHeight / 2f) / CommonConst.FloorHeight));
        if(mainGameObjectPosition.x < 0 || mainGameObjectPosition.x >= CommonConst.MapWidth || mainGameObjectPosition.y < 0 || mainGameObjectPosition.y >= CommonConst.MapWidth){
            Debug.LogError("引数のゲームオブジェクトがマップ外にいます");
            return null;
        }
        List<Vector2Int> targetPositions = new List<Vector2Int>();
        for(int y = mainGameObjectPosition.y - (int)MaxTargetDistanceForAxis; y <= mainGameObjectPosition.y + (int)MaxTargetDistanceForAxis; ++y){
            for(int x = mainGameObjectPosition.x - (int)MaxTargetDistanceForAxis; x <= mainGameObjectPosition.x + (int)MaxTargetDistanceForAxis; ++x){
                if(x >= 0 && x < CommonConst.MapWidth && y >= 0 && y < CommonConst.MapHeight && _map[y, x] == CommonConst.DungeonArea){
                    targetPositions.Add(new Vector2Int(x, y));
                }
            }
        }
        if(targetPositions.Count == 0){
            Debug.LogError("引数のゲームオブジェクトの周囲に移動可能な座標がありません");
            return null;
        }
        Vector2Int targetPositionCoordinate = targetPositions[Random.Range(0, targetPositions.Count)];
        Vector3 targetPosition = new Vector3(targetPositionCoordinate.x * CommonConst.FloorWidth, mainGameObject.transform.position.y, targetPositionCoordinate.y * CommonConst.FloorHeight);

        GameObject targetGameObject = new GameObject("Target");
        targetGameObject.transform.position = targetPosition;
        return targetGameObject;
    }

    private static GameObject CreateTargetGameObjectInPath(GameObject mainGameObject)
    {
        CommonEnemyVariable commonEnemyVariable = mainGameObject.GetComponent<CommonEnemyVariable>();
        if(commonEnemyVariable == null){
            Debug.LogError("CommonEnemyVariableがアタッチされていません");
            return null;
        }
        if(commonEnemyVariable.isEnemyInRoom){
            Debug.LogError("引数のゲームオブジェクトが通路にいません");
            return null;
        }
        // 最初に方向を決定し、その後に移動距離を決定する
        bool[] continuousPathDirection = new bool[CommonConst.DirectionMax + 1];
        for(int i = 0; i < CommonConst.DirectionMax; i++){
            continuousPathDirection[i] = false;
        }
        Vector2Int mainGameObjectPosition = new Vector2Int(Mathf.FloorToInt((mainGameObject.transform.position.x + CommonConst.FloorWidth / 2f) / CommonConst.FloorWidth), Mathf.FloorToInt((mainGameObject.transform.position.z + CommonConst.FloorHeight / 2f) / CommonConst.FloorHeight));
        if(mainGameObjectPosition.x < 0 || mainGameObjectPosition.x >= CommonConst.MapWidth || mainGameObjectPosition.y < 0 || mainGameObjectPosition.y >= CommonConst.MapWidth){
            Debug.LogError("引数のゲームオブジェクトがマップ外にいます");
            return null;
        }
        int continuousPathDirectionCount = 0;
        if(mainGameObjectPosition.x - 1 >= 0 && _map[mainGameObjectPosition.y, mainGameObjectPosition.x - 1] == CommonConst.DungeonArea){
            continuousPathDirection[CommonConst.LeftDirection] = true;
            ++continuousPathDirectionCount;
        }
        if(mainGameObjectPosition.x + 1 < _map.GetLength(0) && _map[mainGameObjectPosition.y, mainGameObjectPosition.x + 1] == CommonConst.DungeonArea){
            continuousPathDirection[CommonConst.RightDirection] = true;
            ++continuousPathDirectionCount;
        }
        if(mainGameObjectPosition.y - 1 >= 0 && _map[mainGameObjectPosition.y - 1, mainGameObjectPosition.x] == CommonConst.DungeonArea){
            continuousPathDirection[CommonConst.UpDirection] = true;
            ++continuousPathDirectionCount;
        }
        if(mainGameObjectPosition.y + 1 < _map.GetLength(1) && _map[mainGameObjectPosition.y + 1, mainGameObjectPosition.x] == CommonConst.DungeonArea){
            continuousPathDirection[CommonConst.DownDirection] = true;
            ++continuousPathDirectionCount;
        }
        if(continuousPathDirectionCount == 0 || continuousPathDirectionCount == 1){
            Debug.LogError("引数のゲームオブジェクトが通路にいません");
            return null;
        }
        int direction = Random.Range(0, continuousPathDirectionCount);
        int continuousPathDirectionIndex = 0;
        int continuousPathDirectionCountIndex = 0;
        for(int i = 0; i < CommonConst.DirectionMax; ++i){
            if(continuousPathDirection[i]){
                if(direction == continuousPathDirectionCountIndex){
                    continuousPathDirectionIndex = i;
                    break;
                }
                ++continuousPathDirectionCountIndex;
            }
        }
        int continuousPathValidLength = 0;
        Vector3 targetPosition;
        if(continuousPathDirectionIndex == CommonConst.LeftDirection){
            for(int i = mainGameObjectPosition.x - 1; i > mainGameObjectPosition.x - (int)MaxTargetDistanceForAxis; --i){
                if(i > 0 && _map[mainGameObjectPosition.y, i] == CommonConst.DungeonArea && _map[mainGameObjectPosition.y, i + 1] == CommonConst.DungeonArea){
                    ++continuousPathValidLength;
                }
                else{
                    break;
                }
            }
            float distance = Random.Range(1.0f, (float)continuousPathValidLength);
            targetPosition = new Vector3(mainGameObject.transform.position.x - distance * CommonConst.FloorWidth + CommonConst.FloorWidth/2.0f, mainGameObject.transform.position.y, mainGameObject.transform.position.z);
        } else if(continuousPathDirectionIndex == CommonConst.RightDirection){
            for(int i = mainGameObjectPosition.x + 1; i < mainGameObjectPosition.x + (int)MaxTargetDistanceForAxis; ++i){
                if(i < CommonConst.MapWidth && _map[mainGameObjectPosition.y, i] == CommonConst.DungeonArea && _map[mainGameObjectPosition.y, i - 1] == CommonConst.DungeonArea){
                    ++continuousPathValidLength;
                }
                else{
                    break;
                }
            }
            float distance = Random.Range(1.0f, (float)continuousPathValidLength);
            targetPosition = new Vector3(mainGameObject.transform.position.x + distance * CommonConst.FloorWidth - CommonConst.FloorWidth/2.0f, mainGameObject.transform.position.y, mainGameObject.transform.position.z);
        } else if(continuousPathDirectionIndex == CommonConst.UpDirection){
            for(int i = mainGameObjectPosition.y - 1; i > mainGameObjectPosition.y - (int)MaxTargetDistanceForAxis; --i){
                if(i > 0 && _map[i, mainGameObjectPosition.x] == CommonConst.DungeonArea && _map[i + 1, mainGameObjectPosition.x] == CommonConst.DungeonArea){
                    ++continuousPathValidLength;
                }
                else{
                    break;
                }
            }
            float distance = Random.Range(1.0f, (float)continuousPathValidLength);
            targetPosition = new Vector3(mainGameObject.transform.position.x, mainGameObject.transform.position.y, mainGameObject.transform.position.z - distance * CommonConst.FloorHeight + CommonConst.FloorHeight/2.0f);
        } else if(continuousPathDirectionIndex == CommonConst.DownDirection){
            for(int i = mainGameObjectPosition.y + 1; i < mainGameObjectPosition.y + (int)MaxTargetDistanceForAxis; ++i){
                if(i < CommonConst.MapHeight && _map[i, mainGameObjectPosition.x] == CommonConst.DungeonArea && _map[i - 1, mainGameObjectPosition.x] == CommonConst.DungeonArea){
                    ++continuousPathValidLength;
                }
                else{
                    break;
                }
            }
            float distance = Random.Range(1.0f, (float)continuousPathValidLength);
            targetPosition = new Vector3(mainGameObject.transform.position.x, mainGameObject.transform.position.y, mainGameObject.transform.position.z + distance * CommonConst.FloorHeight - CommonConst.FloorHeight/2.0f);
        } else {
            // ここには来ないはず
            targetPosition = mainGameObject.transform.position;
        }

        GameObject targetGameObject = new GameObject("Target");
        targetGameObject.transform.position = targetPosition;
        return targetGameObject;
    }
}
