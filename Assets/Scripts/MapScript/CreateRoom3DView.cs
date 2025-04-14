using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 部屋を描画するクラス
public static class CreateRoom3DView
{
    private static readonly int WallAmount = 3;
    private static readonly float WallThickness = 0.2f;
    private static readonly float WallHeight = 1.0f;
    // private static readonly float WallWidth = 1.0f;
    private static readonly float InvisibleWallThickness = 0.23f;
    private static readonly float PatitionDistance = 2.0f;
    private static readonly float FloorWidth = CommonConst.FloorWidth;
    private static readonly float FloorHeight = CommonConst.FloorHeight;
    private static readonly float FloorThickness = CommonConst.FloorThickness;

    public static MapObjects ViewStart(FloorManagement floorManagement)
    {
        GameObject floorPrefab = Resources.Load<GameObject>("Prefabs/PlaneFloor");
        GameObject wallPrefab = Resources.Load<GameObject>("Prefabs/PlaneWall");
        GameObject invisibleWallPrefab = Resources.Load<GameObject>("Prefabs/InvisibleWall");
        GameObject invisibleWallCornerPrefab = Resources.Load<GameObject>("Prefabs/InvisibleWallCorner");
        GameObject healObject = Resources.Load<GameObject>("Prefabs/HealObject");
        GameObject stairPrefab = Resources.Load<GameObject>("Prefabs/Stairs");
        GameObject clearOrbPrefab = Resources.Load<GameObject>("Prefabs/MagicOrb");
        // GameObject roomPatitionPrefab = Resources.Load<GameObject>("Prefabs/RoomPatition");
        GameObject roomPatitionPrefab = Resources.Load<GameObject>("Prefabs/FogParticle");
        Material minimapMaterial = Resources.Load<Material>("Materials/Minimap");
        Material minimapStairsMaterial = Resources.Load<Material>("Materials/MinimapStairsMaterials");
        Material minimapHealObjectMaterial = Resources.Load<Material>("Materials/HealObjectMaterial");

        CreateDungeon createDungeon = floorManagement.CreateDungeon;
        List<GameObject> mapObjects = new List<GameObject>();
        Dictionary<Room, GameObject> roomColliderObjects = new Dictionary<Room, GameObject>();
        Dictionary<Vector2Int, GameObject> minimapObjects = new Dictionary<Vector2Int, GameObject>();
        List<Room> rooms = createDungeon.Rooms;
        int[,] map = createDungeon.Map;
        Room floorClearRoom = createDungeon.Rooms[floorManagement.FloorClearRoomIndex];
        Vector2Int floorClearPosition = floorManagement.FloorClearPosition;
        int stairDirection = floorManagement.StairDirection;
        Room healObjectRoom = createDungeon.Rooms[floorManagement.HealObjectRoomIndex];
        Vector2Int healObjectPosition = floorManagement.HealObjectPosition;

        GameObject mapObjectParent = GameObject.Find("Map");
        GameObject minimapObjectParent = GameObject.Find("MinimapObject");
        GameObject roomColliderParent = GameObject.Find("RoomCollider");
        for(int y = -1; y <= CommonConst.MapHeight; ++y){
            for(int x = -1; x <= CommonConst.MapWidth; ++x){
                // 天井を生成
                // このようにしないとなぜか角から光が入ってくる
                GameObject ceil = GameObject.Instantiate(floorPrefab, new Vector3(x * FloorWidth, WallHeight * WallAmount + FloorThickness / 2.0f, y * FloorHeight), Quaternion.identity);
                ceil.transform.parent = mapObjectParent.transform;
                ceil.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                mapObjects.Add(ceil);
                // map[y, x] == CommonConst.EmptyArea なら壁を生成しない
                if(y < CommonConst.MinPos || y >= CommonConst.MapHeight || x < CommonConst.MinPos || x >= CommonConst.MapWidth || map[y, x] == CommonConst.EmptyArea){
                    continue;
                }
                GameObject minimapObject = GameObject.Instantiate(floorPrefab, new Vector3(x * FloorWidth, 10, y * FloorHeight), Quaternion.identity);
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
                if(y > CommonConst.MinPos && x > CommonConst.MinPos && map[y - 1, x] == CommonConst.DungeonArea && map[y, x - 1] == CommonConst.DungeonArea && map[y - 1, x - 1] == CommonConst.EmptyArea){
                    // 右下が角
                    for(int i = 0; i < WallAmount; ++i){
                        GameObject wallCorner = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x * FloorWidth - (FloorWidth)/2.0f + (InvisibleWallThickness)/2.0f, WallHeight/2.0f + i * WallHeight, y * FloorHeight - (FloorHeight)/2.0f + (InvisibleWallThickness)/2.0f), Quaternion.identity);
                        wallCorner.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(wallCorner);
                    }
                }
                if(y > CommonConst.MinPos && x < CommonConst.MapWidth - 1 && map[y - 1, x] == CommonConst.DungeonArea && map[y, x + 1] == CommonConst.DungeonArea && map[y - 1, x + 1] == CommonConst.EmptyArea){
                    // 左下が角
                    for(int i = 0; i < WallAmount; ++i){
                        GameObject wallCorner = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x * FloorWidth + (FloorWidth)/2.0f - (InvisibleWallThickness)/2.0f, WallHeight/2.0f + i * WallHeight, y * FloorHeight - (FloorHeight)/2.0f + (InvisibleWallThickness)/2.0f), Quaternion.identity);
                        wallCorner.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(wallCorner);
                    }
                }
                if(y < CommonConst.MapHeight - 1 && x > CommonConst.MinPos && map[y + 1, x] == CommonConst.DungeonArea && map[y, x - 1] == CommonConst.DungeonArea && map[y + 1, x - 1] == CommonConst.EmptyArea){
                    // 右上が角
                    for(int i = 0; i < WallAmount; ++i){
                        GameObject wallCorner = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x * FloorWidth - (FloorWidth)/2.0f + (InvisibleWallThickness)/2.0f, WallHeight/2.0f + i * WallHeight, y * FloorHeight + (FloorHeight)/2.0f - (InvisibleWallThickness)/2.0f), Quaternion.identity);
                        wallCorner.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(wallCorner);
                    }
                }
                if(y < CommonConst.MapHeight - 1 && x < CommonConst.MapWidth - 1 && map[y + 1, x] == CommonConst.DungeonArea && map[y, x + 1] == CommonConst.DungeonArea && map[y + 1, x + 1] == CommonConst.EmptyArea){
                    // 左上が角
                    for(int i = 0; i < WallAmount; ++i){
                        GameObject wallCorner = GameObject.Instantiate(invisibleWallCornerPrefab, new Vector3(x * FloorWidth + (FloorWidth)/2.0f - (InvisibleWallThickness)/2.0f, WallHeight/2.0f + i * WallHeight, y * FloorHeight + (FloorHeight)/2.0f - (InvisibleWallThickness)/2.0f), Quaternion.identity);
                        wallCorner.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(wallCorner);
                    }
                }
                if(y == CommonConst.MinPos || map[y - 1, x] == CommonConst.EmptyArea){
                    for(int i = 0; i < WallAmount; ++i){
                        GameObject wall = GameObject.Instantiate(wallPrefab, new Vector3(x * FloorWidth, WallHeight/2.0f + i * WallHeight, y * FloorHeight - (FloorHeight + WallThickness)/2.0f), wallRotations[1]);
                        GameObject invisibleWall = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x * FloorWidth, WallHeight/2.0f + i * WallHeight, y * FloorHeight - (FloorHeight)/2.0f + (InvisibleWallThickness)/2.0f), wallRotations[1]);
                        wall.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                        wall.transform.parent = mapObjectParent.transform;
                        invisibleWall.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(wall);
                        mapObjects.Add(invisibleWall);
                    }
                }
                if(y == CommonConst.MapHeight - 1 || map[y + 1, x] == CommonConst.EmptyArea){
                    for(int i = 0; i < WallAmount; ++i){
                        GameObject wall = GameObject.Instantiate(wallPrefab, new Vector3(x * FloorWidth, WallHeight/2.0f + i * WallHeight, y * FloorHeight + (FloorHeight + WallThickness)/2.0f), wallRotations[1]);
                        GameObject invisibleWall = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x * FloorWidth, WallHeight/2.0f + i * WallHeight, y * FloorHeight + (FloorHeight)/2.0f - (InvisibleWallThickness)/2.0f), wallRotations[1]);
                        wall.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                        wall.transform.parent = mapObjectParent.transform;
                        invisibleWall.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(wall);
                        mapObjects.Add(invisibleWall);
                    }
                }
                if(x == CommonConst.MinPos || map[y, x - 1] == CommonConst.EmptyArea){
                    for(int i = 0; i < WallAmount; ++i){
                        GameObject wall = GameObject.Instantiate(wallPrefab, new Vector3(x * FloorWidth - (FloorWidth + WallThickness)/2.0f, WallHeight/2.0f + i * WallHeight, y * FloorHeight), wallRotations[0]);
                        GameObject invisibleWall = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x * FloorWidth - (FloorWidth)/2.0f + (InvisibleWallThickness)/2.0f, WallHeight/2.0f + i * WallHeight, y * FloorHeight), wallRotations[0]);
                        wall.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                        wall.transform.parent = mapObjectParent.transform;
                        invisibleWall.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(wall);
                        mapObjects.Add(invisibleWall);
                    }
                }
                if(x == CommonConst.MapWidth - 1 || map[y, x + 1] == CommonConst.EmptyArea){
                    for(int i = 0; i < WallAmount; ++i){
                        GameObject wall = GameObject.Instantiate(wallPrefab, new Vector3(x * FloorWidth + (FloorWidth + WallThickness)/2.0f, WallHeight/2.0f + i * WallHeight, y * FloorHeight), wallRotations[0]);
                        GameObject invisibleWall = GameObject.Instantiate(invisibleWallPrefab, new Vector3(x * FloorWidth + (FloorWidth)/2.0f - (InvisibleWallThickness)/2.0f, WallHeight/2.0f + i * WallHeight, y * FloorHeight), wallRotations[0]);
                        wall.GetComponent<Renderer>().material = floorManagement.WallFloorMaterial;
                        wall.transform.parent = mapObjectParent.transform;
                        invisibleWall.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(wall);
                        mapObjects.Add(invisibleWall);
                    }
                }
                if(floorManagement.Floor != CommonConst.MaxFloor && ((x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y) || (stairDirection == CommonConst.UpDirection && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y - 1) || (stairDirection == CommonConst.RightDirection && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x + 1 && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y) || (stairDirection == CommonConst.DownDirection && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y + 1) || (stairDirection == CommonConst.LeftDirection && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x - 1 && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y))){
                    if(x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y){
                        GameObject stairs;
                        if(stairDirection == CommonConst.UpDirection){
                            stairs = GameObject.Instantiate(stairPrefab, new Vector3(x * FloorWidth, - 2 * WallHeight, y * FloorHeight - FloorHeight), Quaternion.AngleAxis(180, Vector3.up));
                        }else if(stairDirection == CommonConst.RightDirection){
                            stairs = GameObject.Instantiate(stairPrefab, new Vector3(x * FloorWidth + FloorWidth, - 2 * WallHeight, y * FloorHeight), Quaternion.AngleAxis(90, Vector3.up));
                        }else if(stairDirection == CommonConst.DownDirection){
                            stairs = GameObject.Instantiate(stairPrefab, new Vector3(x * FloorWidth, - 2 * WallHeight, y * FloorHeight + FloorHeight), Quaternion.AngleAxis(0, Vector3.up));
                        }else{
                            stairs = GameObject.Instantiate(stairPrefab, new Vector3(x * FloorWidth - FloorWidth, - 2 * WallHeight, y * FloorHeight), Quaternion.AngleAxis(270, Vector3.up));
                        }
                        MeshRenderer[] meshRenderers = stairs.GetComponentsInChildren<MeshRenderer>();
                        foreach(MeshRenderer meshRenderer in meshRenderers){
                            if(meshRenderer.gameObject.name == "CheckText"){
                                continue;
                            }
                            meshRenderer.material = floorManagement.WallFloorMaterial;
                        }
                        stairs.transform.Find("StairCollider").gameObject.GetComponent<CommonCheckEvent>().SetEventTemplate(new StairEvent());
                        stairs.transform.parent = mapObjectParent.transform;
                        mapObjects.Add(stairs);
                    }
                    minimapObject.GetComponent<Renderer>().material = minimapStairsMaterial;
                    minimapObjects[new Vector2Int(x, y)] = minimapObject;
                    continue;
                }
                if(floorManagement.Floor == CommonConst.MaxFloor && x == floorClearRoom.UpperLeftPosition.x + floorClearPosition.x && y == floorClearRoom.UpperLeftPosition.y + floorClearPosition.y){
                    GameObject clearOrb = GameObject.Instantiate(clearOrbPrefab, new Vector3(x * FloorWidth, WallHeight / 2.0f, y * FloorHeight), Quaternion.identity);
                    clearOrb.transform.Find("MagicOrbCollider").gameObject.GetComponent<CommonCheckEvent>().SetEventTemplate(new ClearOrbEvent());
                    clearOrb.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(clearOrb);
                    minimapObject.GetComponent<Renderer>().material = minimapStairsMaterial;
                    minimapObjects[new Vector2Int(x, y)] = minimapObject;
                }
                if(x == healObjectRoom.UpperLeftPosition.x + healObjectPosition.x && y == healObjectRoom.UpperLeftPosition.y + healObjectPosition.y){
                    GameObject healObjectInstance = GameObject.Instantiate(healObject, new Vector3(x * FloorWidth, FloorThickness / 2.0f, y * FloorHeight), Quaternion.identity);
                    healObjectInstance.transform.Find("HealObjectCollider").gameObject.GetComponent<CommonCheckEvent>().SetEventTemplate(new HealObjectEvent());
                    healObjectInstance.transform.parent = mapObjectParent.transform;
                    mapObjects.Add(healObjectInstance);
                    minimapObject.GetComponent<Renderer>().material = minimapHealObjectMaterial;
                    minimapObjects[new Vector2Int(x, y)] = minimapObject;
                }
                // 床を生成
                GameObject floor = GameObject.Instantiate(floorPrefab, new Vector3(x * FloorWidth, 0, y * FloorHeight), Quaternion.identity);
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
            roomObject.transform.position = new Vector3((room.UpperLeftPosition.x + (room.Size.x - 1) / 2.0f) * FloorWidth, WallAmount * WallHeight / 2.0f, (room.UpperLeftPosition.y + (room.Size.y - 1) / 2.0f) * FloorHeight);
            // 通路の位置を把握し、その位置にQuad(roomPatitionPrefab)を生成する
            for(int i = 0; i < room.Size.x; ++i){
                if(room.UpperLeftPosition.y != 0)
                {
                    if(map[room.UpperLeftPosition.y - 1, room.UpperLeftPosition.x + i] == CommonConst.DungeonArea)
                    {
                        // 通路が上にある場合
                        GameObject roomPatition = GameObject.Instantiate(roomPatitionPrefab, new Vector3((room.UpperLeftPosition.x + i) * FloorWidth, roomObject.transform.position.y, (room.UpperLeftPosition.y - PatitionDistance) * FloorHeight), Quaternion.Euler(new Vector3(0, 180, 0)), roomObject.transform);
                        // roomPatition.transform.localScale = new Vector3(FloorWidth, WallAmount * WallHeight, 1f);
                    }
                }
                if(room.UpperLeftPosition.y + room.Size.y != CommonConst.MapHeight - 1)
                {
                    if(map[room.UpperLeftPosition.y + room.Size.y, room.UpperLeftPosition.x + i] == CommonConst.DungeonArea)
                    {
                        // 通路が下にある場合
                        GameObject roomPatition = GameObject.Instantiate(roomPatitionPrefab, new Vector3((room.UpperLeftPosition.x + i) * FloorWidth, roomObject.transform.position.y, (room.UpperLeftPosition.y + room.Size.y - 1.0f + PatitionDistance) * FloorHeight), Quaternion.Euler(new Vector3(0, 0, 0)), roomObject.transform);
                        // roomPatition.transform.localScale = new Vector3(FloorWidth, WallAmount * WallHeight, 1f);
                    }
                }
            }
            for(int i = 0; i < room.Size.y; ++i){
                if(room.UpperLeftPosition.x != 0)
                {
                    if(map[room.UpperLeftPosition.y + i, room.UpperLeftPosition.x - 1] == CommonConst.DungeonArea)
                    {
                        // 通路が左にある場合
                        GameObject roomPatition = GameObject.Instantiate(roomPatitionPrefab, new Vector3((room.UpperLeftPosition.x - PatitionDistance) * FloorWidth, roomObject.transform.position.y, (room.UpperLeftPosition.y + i) * FloorHeight), Quaternion.Euler(new Vector3(0, 270, 0)), roomObject.transform);
                        // roomPatition.transform.localScale = new Vector3(FloorHeight, WallAmount * WallHeight, 1f);
                    }
                }
                if(room.UpperLeftPosition.x + room.Size.x != CommonConst.MapWidth - 1)
                {
                    if(map[room.UpperLeftPosition.y + i, room.UpperLeftPosition.x + room.Size.x] == CommonConst.DungeonArea)
                    {
                        // 通路が右にある場合
                        GameObject roomPatition = GameObject.Instantiate(roomPatitionPrefab, new Vector3((room.UpperLeftPosition.x + room.Size.x - 1.0f + PatitionDistance) * FloorWidth, roomObject.transform.position.y, (room.UpperLeftPosition.y + i) * FloorHeight), Quaternion.Euler(new Vector3(0, 90, 0)), roomObject.transform);
                        // roomPatition.transform.localScale = new Vector3(FloorHeight, WallAmount * WallHeight, 1f);
                    }
                }
            }
            // Vector3 roomPatitionPos1 = new Vector3(roomObject.transform.position.x, roomObject.transform.position.y, (room.UpperLeftPosition.y - 1.5f) * FloorHeight);
            // GameObject roomPatition1 = GameObject.Instantiate(roomPatitionPrefab, roomPatitionPos1, Quaternion.Euler(new Vector3(0, 180, 0)), roomObject.transform);
            // roomPatition1.transform.localScale = new Vector3((room.Size.x + 2) * FloorWidth, WallAmount * WallHeight, 1f);
            // Vector3 roomPatitionPos2 = new Vector3((room.UpperLeftPosition.x + room.Size.x + 0.5f) * FloorWidth, roomObject.transform.position.y, roomObject.transform.position.z);
            // GameObject roomPatition2 = GameObject.Instantiate(roomPatitionPrefab, roomPatitionPos2, Quaternion.Euler(new Vector3(0, 90, 0)), roomObject.transform);
            // roomPatition2.transform.localScale = new Vector3((room.Size.y + 2) * FloorHeight, WallAmount * WallHeight, 1f);
            // Vector3 roomPatitionPos3 = new Vector3(roomObject.transform.position.x, roomObject.transform.position.y, (room.UpperLeftPosition.y + room.Size.y + 0.5f) * FloorHeight);
            // GameObject roomPatition3 = GameObject.Instantiate(roomPatitionPrefab, roomPatitionPos3, Quaternion.Euler(new Vector3(0, 0, 0)), roomObject.transform);
            // roomPatition3.transform.localScale = new Vector3((room.Size.x + 2) * FloorWidth, WallAmount * WallHeight, 1f);
            // Vector3 roomPatitionPos4 = new Vector3((room.UpperLeftPosition.x - 1.5f) * FloorWidth, roomObject.transform.position.y, roomObject.transform.position.z);
            // GameObject roomPatition4 = GameObject.Instantiate(roomPatitionPrefab, roomPatitionPos4, Quaternion.Euler(new Vector3(0, 270, 0)), roomObject.transform);
            // roomPatition4.transform.localScale = new Vector3((room.Size.y + 2) * FloorHeight, WallAmount * WallHeight, 1f);
            boxCollider.size = new Vector3(room.Size.x * FloorWidth, WallAmount * WallHeight, room.Size.y * FloorHeight);
            roomObject.layer = LayerMask.NameToLayer("InvisibleLayer");
            roomObject.transform.parent = roomColliderParent.transform;
            roomColliderObjects.Add(room, roomObject);
        }
        return new MapObjects(mapObjects, roomColliderObjects, minimapObjects);
    }
}
