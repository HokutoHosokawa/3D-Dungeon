using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 部屋を描画するクラス
public static class CreateRoom3DView
{
    const float WallThickness = 0.2f;
    const float WallHeight = 1.0f;
    const float WallWidth = 1.0f;
    const float InvisibleWallThickness = 0.23f;
    const float FloorWidth = 1.0f;
    const float FloorHeight = 1.0f;
    const float FloorThickness = 0.2f;

    public static MapObjects ViewStart(FloorManagement floorManagement)
    {
        GameObject floorPrefab = Resources.Load<GameObject>("Prefabs/PlaneFloor");
        GameObject wallPrefab = Resources.Load<GameObject>("Prefabs/PlaneWall");
        GameObject invisibleWallPrefab = Resources.Load<GameObject>("Prefabs/InvisibleWall");
        GameObject invisibleWallCornerPrefab = Resources.Load<GameObject>("Prefabs/InvisibleWallCorner");
        GameObject stairPrefab = Resources.Load<GameObject>("Prefabs/Stairs");
        GameObject clearOrbPrefab = Resources.Load<GameObject>("Prefabs/MagicOrb");
        Material minimapMaterial = Resources.Load<Material>("Materials/Minimap");
        Material minimapStairsMaterial = Resources.Load<Material>("Materials/MinimapStairsMaterials");

        CreateDungeon createDungeon = floorManagement.CreateDungeon;
        List<GameObject> mapObjects = new List<GameObject>();
        Dictionary<Room, GameObject> roomColliderObjects = new Dictionary<Room, GameObject>();
        Dictionary<Vector2Int, GameObject> minimapObjects = new Dictionary<Vector2Int, GameObject>();
        List<Room> rooms = createDungeon.Rooms;
        int[,] map = createDungeon.Map;
        Room floorClearRoom = createDungeon.Rooms[floorManagement.FloorClearRoomIndex];
        Vector2Int floorClearPosition = floorManagement.FloorClearPosition;
        int stairDirection = floorManagement.StairDirection;

        GameObject mapObjectParent = GameObject.Find("Map");
        GameObject minimapObjectParent = GameObject.Find("MinimapObject");
        GameObject roomColliderParent = GameObject.Find("RoomCollider");
        for(int y = -1; y <= CommonConst.MapHeight; ++y){
            for(int x = -1; x <= CommonConst.MapWidth; ++x){
                // 天井を生成
                // このようにしないとなぜか角から光が入ってくる
                GameObject ceil = GameObject.Instantiate(floorPrefab, new Vector3(x, WallHeight * 2 + FloorThickness / 2.0f, y), Quaternion.identity);
                ceil.transform.parent = mapObjectParent.transform;
                ceil.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                mapObjects.Add(ceil);
                // map[y, x] == CommonConst.EmptyArea なら壁を生成しない
                if(y < 0 || y >= CommonConst.MapHeight || x < 0 || x >= CommonConst.MapWidth || map[y, x] == CommonConst.EmptyArea){
                    continue;
                }
                GameObject minimapObject = GameObject.Instantiate(floorPrefab, new Vector3(x, 10, y), Quaternion.identity);
                minimapObject.GetComponent<Renderer>().material = minimapMaterial;
                minimapObject.layer = LayerMask.NameToLayer("Minimap");
                minimapObject.transform.parent = minimapObjectParent.transform;
                minimapObject.SetActive(false);
                minimapObjects.Add(new Vector2Int(x, y), minimapObject);
                // 壁を生成
                Quaternion[] wallRotations = {
                    Quaternion.AngleAxis(0, Vector3.up), // z軸に水平(x軸に直交)
                    Quaternion.AngleAxis(90, Vector3.up) // z軸に垂直(x軸に直交)
                };
                if(y > 0 && x > 0 && map[y - 1, x] == CommonConst.DungeonArea && map[y, x - 1] == CommonConst.DungeonArea && map[y - 1, x - 1] == CommonConst.EmptyArea){
                    // 右下が角
                    GameObject wallCorner = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x - (FloorWidth)/2.0f + (InvisibleWallThickness)/2.0f, WallHeight/2.0f, y - (FloorHeight)/2.0f + (InvisibleWallThickness)/2.0f), Quaternion.identity);
                    GameObject wallCorner2 = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x - (FloorWidth)/2.0f + (InvisibleWallThickness)/2.0f, 3 * WallHeight/2.0f, y - (FloorHeight)/2.0f + (InvisibleWallThickness)/2.0f), Quaternion.identity);
                    wallCorner.transform.parent = mapObjectParent.transform;
                    wallCorner2.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(wallCorner);
                    mapObjects.Add(wallCorner2);
                }
                if(y > 0 && x < CommonConst.MapWidth - 1 && map[y - 1, x] == CommonConst.DungeonArea && map[y, x + 1] == CommonConst.DungeonArea && map[y - 1, x + 1] == CommonConst.EmptyArea){
                    // 左下が角
                    GameObject wallCorner = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x + (FloorWidth)/2.0f - (InvisibleWallThickness)/2.0f, WallHeight/2.0f, y - (FloorHeight)/2.0f + (InvisibleWallThickness)/2.0f), Quaternion.identity);
                    GameObject wallCorner2 = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x + (FloorWidth)/2.0f - (InvisibleWallThickness)/2.0f, 3 * WallHeight/2.0f, y - (FloorHeight)/2.0f + (InvisibleWallThickness)/2.0f), Quaternion.identity);
                    wallCorner.transform.parent = mapObjectParent.transform;
                    wallCorner2.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(wallCorner);
                    mapObjects.Add(wallCorner2);
                }
                if(y < CommonConst.MapHeight - 1 && x > 0 && map[y + 1, x] == CommonConst.DungeonArea && map[y, x - 1] == CommonConst.DungeonArea && map[y + 1, x - 1] == CommonConst.EmptyArea){
                    // 右上が角
                    GameObject wallCorner = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x - (FloorWidth)/2.0f + (InvisibleWallThickness)/2.0f, WallHeight/2.0f, y + (FloorHeight)/2.0f - (InvisibleWallThickness)/2.0f), Quaternion.identity);
                    GameObject wallCorner2 = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x - (FloorWidth)/2.0f + (InvisibleWallThickness)/2.0f, 3 * WallHeight/2.0f, y + (FloorHeight)/2.0f - (InvisibleWallThickness)/2.0f), Quaternion.identity);
                    wallCorner.transform.parent = mapObjectParent.transform;
                    wallCorner2.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(wallCorner);
                    mapObjects.Add(wallCorner2);
                }
                if(y < CommonConst.MapHeight - 1 && x < CommonConst.MapWidth - 1 && map[y + 1, x] == CommonConst.DungeonArea && map[y, x + 1] == CommonConst.DungeonArea && map[y + 1, x + 1] == CommonConst.EmptyArea){
                    // 左上が角
                    GameObject wallCorner = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x + (FloorWidth)/2.0f - (InvisibleWallThickness)/2.0f, WallHeight/2.0f, y + (FloorHeight)/2.0f - (InvisibleWallThickness)/2.0f), Quaternion.identity);
                    GameObject wallCorner2 = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x + (FloorWidth)/2.0f - (InvisibleWallThickness)/2.0f, 3 * WallHeight/2.0f, y + (FloorHeight)/2.0f - (InvisibleWallThickness)/2.0f), Quaternion.identity);
                    wallCorner.transform.parent = mapObjectParent.transform;
                    wallCorner2.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(wallCorner);
                    mapObjects.Add(wallCorner2);
                }
                if(y == 0 || map[y - 1, x] == CommonConst.EmptyArea){
                    GameObject wall = GameObject.Instantiate(wallPrefab, new Vector3(x, WallHeight/2.0f, y - (FloorHeight + WallThickness)/2.0f), wallRotations[1]);
                    GameObject wall2 = GameObject.Instantiate(wallPrefab, new Vector3(x, 3 * WallHeight/2.0f, y - (FloorHeight + WallThickness)/2.0f), wallRotations[1]);
                    GameObject invisibleWall = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x, WallHeight/2.0f, y - (FloorHeight)/2.0f + (InvisibleWallThickness)/2.0f), wallRotations[1]);
                    GameObject invisibleWall2 = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x, 3 * WallHeight/2.0f, y - (FloorHeight)/2.0f + (InvisibleWallThickness)/2.0f), wallRotations[1]);
                    wall.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                    wall2.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                    wall.transform.parent = mapObjectParent.transform;
                    wall2.transform.parent = mapObjectParent.transform;
                    invisibleWall.transform.parent = mapObjectParent.transform;
                    invisibleWall2.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(wall);
                    mapObjects.Add(wall2);
                    mapObjects.Add(invisibleWall);
                    mapObjects.Add(invisibleWall2);
                }
                if(y == CommonConst.MapHeight - 1 || map[y + 1, x] == CommonConst.EmptyArea){
                    GameObject wall = GameObject.Instantiate(wallPrefab, new Vector3(x, WallHeight/2.0f, y + (FloorHeight + WallThickness)/2.0f), wallRotations[1]);
                    GameObject wall2 = GameObject.Instantiate(wallPrefab, new Vector3(x, 3 * WallHeight/2.0f, y + (FloorHeight + WallThickness)/2.0f), wallRotations[1]);
                    GameObject invisibleWall = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x, WallHeight/2.0f, y + (FloorHeight)/2.0f - (InvisibleWallThickness)/2.0f), wallRotations[1]);
                    GameObject invisibleWall2 = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x, 3 * WallHeight/2.0f, y + (FloorHeight)/2.0f - (InvisibleWallThickness)/2.0f), wallRotations[1]);
                    wall.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                    wall2.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                    wall.transform.parent = mapObjectParent.transform;
                    wall2.transform.parent = mapObjectParent.transform;
                    invisibleWall.transform.parent = mapObjectParent.transform;
                    invisibleWall2.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(wall);
                    mapObjects.Add(wall2);
                    mapObjects.Add(invisibleWall);
                    mapObjects.Add(invisibleWall2);
                }
                if(x == 0 || map[y, x - 1] == CommonConst.EmptyArea){
                    GameObject wall = GameObject.Instantiate(wallPrefab, new Vector3(x - (FloorWidth + WallThickness)/2.0f, WallHeight/2.0f, y), wallRotations[0]);
                    GameObject wall2 = GameObject.Instantiate(wallPrefab, new Vector3(x - (FloorWidth + WallThickness)/2.0f, 3 * WallHeight/2.0f, y), wallRotations[0]);
                    GameObject invisibleWall = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x - (FloorWidth)/2.0f + (InvisibleWallThickness)/2.0f, WallHeight/2.0f, y), wallRotations[0]);
                    GameObject invisibleWall2 = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x - (FloorWidth)/2.0f + (InvisibleWallThickness)/2.0f, 3 * WallHeight/2.0f, y), wallRotations[0]);
                    wall.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                    wall2.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                    wall.transform.parent = mapObjectParent.transform;
                    wall2.transform.parent = mapObjectParent.transform;
                    invisibleWall.transform.parent = mapObjectParent.transform;
                    invisibleWall2.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(wall);
                    mapObjects.Add(wall2);
                    mapObjects.Add(invisibleWall);
                    mapObjects.Add(invisibleWall2);
                }
                if(x == CommonConst.MapWidth - 1 || map[y, x + 1] == CommonConst.EmptyArea){
                    GameObject wall = GameObject.Instantiate(wallPrefab, new Vector3(x + (FloorWidth + WallThickness)/2.0f, WallHeight/2.0f, y), wallRotations[0]);
                    GameObject wall2 = GameObject.Instantiate(wallPrefab, new Vector3(x + (FloorWidth + WallThickness)/2.0f, 3 * WallHeight/2.0f, y), wallRotations[0]);
                    GameObject invisibleWall = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x + (FloorWidth)/2.0f - (InvisibleWallThickness)/2.0f, WallHeight/2.0f, y), wallRotations[0]);
                    GameObject invisibleWall2 = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x + (FloorWidth)/2.0f - (InvisibleWallThickness)/2.0f, 3 * WallHeight/2.0f, y), wallRotations[0]);
                    wall.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                    wall2.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                    wall.transform.parent = mapObjectParent.transform;
                    wall2.transform.parent = mapObjectParent.transform;
                    invisibleWall.transform.parent = mapObjectParent.transform;
                    invisibleWall2.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(wall);
                    mapObjects.Add(wall2);
                    mapObjects.Add(invisibleWall);
                    mapObjects.Add(invisibleWall2);
                }
                if(floorManagement.Floor != CommonConst.MaxFloor && ((x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y) || (stairDirection == CommonConst.UpDirection && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y - 1) || (stairDirection == CommonConst.RightDirection && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x + 1 && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y) || (stairDirection == CommonConst.DownDirection && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y + 1) || (stairDirection == CommonConst.LeftDirection && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x - 1 && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y))){
                    if(x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y){
                        GameObject stairs;
                        if(stairDirection == CommonConst.UpDirection){
                            stairs = GameObject.Instantiate(stairPrefab, new Vector3(x, - 4 * WallHeight / 2.0f, y - FloorHeight), Quaternion.AngleAxis(180, Vector3.up));
                        }else if(stairDirection == CommonConst.RightDirection){
                            stairs = GameObject.Instantiate(stairPrefab, new Vector3(x + FloorWidth, - 4 * WallHeight / 2.0f, y), Quaternion.AngleAxis(90, Vector3.up));
                        }else if(stairDirection == CommonConst.DownDirection){
                            stairs = GameObject.Instantiate(stairPrefab, new Vector3(x, - 4 * WallHeight / 2.0f, y + FloorHeight), Quaternion.AngleAxis(0, Vector3.up));
                        }else{
                            stairs = GameObject.Instantiate(stairPrefab, new Vector3(x - FloorWidth, - 4 * WallHeight / 2.0f, y), Quaternion.AngleAxis(270, Vector3.up));
                        }
                        MeshRenderer[] meshRenderers = stairs.GetComponentsInChildren<MeshRenderer>();
                        foreach(MeshRenderer meshRenderer in meshRenderers){
                            meshRenderer.material = floorManagement.WallFloorMaterial;
                        }
                        stairs.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(stairs);
                    }
                    minimapObject.GetComponent<Renderer>().material = minimapStairsMaterial;
                    minimapObjects[new Vector2Int(x, y)] = minimapObject;
                    continue;
                }
                if(floorManagement.Floor == CommonConst.MaxFloor && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y){
                    GameObject clearOrb = GameObject.Instantiate(clearOrbPrefab, new Vector3(x, WallHeight / 2.0f, y), Quaternion.identity);
                    clearOrb.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(clearOrb);
                }
                // 床を生成
                GameObject floor = GameObject.Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity);
                floor.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                floor.transform.parent = mapObjectParent.transform;
                mapObjects.Add(floor);
            }
        }
        foreach(Room room in rooms){
            // 部屋にいるかどうかを判定するために部屋のサイズと一致するBoxColliderを持った空オブジェクトを作成
            GameObject roomObject = new GameObject("Room");
            roomObject.tag = "Room";
            BoxCollider boxCollider = roomObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            roomObject.transform.position = new Vector3(room.UpperLeftPosition.x + (room.Size.x - 1) / 2.0f, WallHeight, room.UpperLeftPosition.y + (room.Size.y - 1) / 2.0f);
            boxCollider.size = new Vector3(room.Size.x, 2 * WallHeight, room.Size.y);
            roomObject.transform.parent = roomColliderParent.transform;
            roomColliderObjects.Add(room, roomObject);
        }
        return new MapObjects(mapObjects, roomColliderObjects, minimapObjects);
    }
}
