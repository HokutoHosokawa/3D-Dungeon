using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

// 階層の作成、描画を行うクラス
public class CreateFloor : MonoBehaviour
{
    const float FloorThickness = 0.2f;
    [SerializeField] GameObject _player;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    private int _currentFloor = 1;
    private FloorManagement[] floorManagements = new FloorManagement[CommonConst.MaxFloor];
    private CommonPlayerVariable _commonPlayerVariable;
    private MapObjects mapObjects;

    public int CurrentFloor => _currentFloor;

    // Start is called before the first frame update
    void Start()
    {
        _commonPlayerVariable = _player.GetComponent<CommonPlayerVariable>();
        CreateNewFloor(_currentFloor);
    }

    // Update is called once per frame
    void Update()
    {
        UploadMinimapInfo();
        UploadMinimap();
    }

    public void CreateNewFloor(int currentFloor)
    {
        if(currentFloor < CommonConst.MinFloor)
        {
            throw new System.ArgumentException("Floor must be greater than CommonConst.MinFloor.");
        }
        if(currentFloor > CommonConst.MaxFloor)
        {
            throw new System.ArgumentException("Floor must be less than or equal to CommonConst.MaxFloor.");
        }
        _currentFloor = currentFloor;
        if(mapObjects != null)
        {
            mapObjects.Destroy();
            _player.transform.position = new Vector3(-1, 0, -1);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyMoveController>().DestroyTargetGameObject();
                Destroy(enemy);
            }
        }
        // 最終階層以外は階段を作成
        Material mat = Resources.Load<Material>("Materials/StoneWall");
        if(floorManagements[currentFloor - 1] == null)
        {
            floorManagements[currentFloor - 1] = new FloorManagement(currentFloor, mat);
        }
        mapObjects = CreateRoom3DView.ViewStart(floorManagements[currentFloor - 1]);
        // プレイヤーの初期位置を設定
        int playerStartRoomIndex = Random.Range(CommonConst.MinIndex, floorManagements[currentFloor - 1].CreateDungeon.Rooms.Count - 1);
        if(playerStartRoomIndex >= floorManagements[currentFloor - 1].FloorClearRoomIndex)
        {
            // クリアルームと同じ部屋にプレイヤーがいないようにする
            playerStartRoomIndex++;
        }
        Room playerStartRoom = floorManagements[currentFloor - 1].CreateDungeon.Rooms[playerStartRoomIndex];
        _player.transform.position = new Vector3((playerStartRoom.UpperLeftPosition.x + playerStartRoom.Size.x / 2.0f) * CommonConst.FloorWidth, FloorThickness / 2.0f, (playerStartRoom.UpperLeftPosition.y + playerStartRoom.Size.y / 2.0f) * CommonConst.FloorHeight);
        _navMeshSurface.BuildNavMesh();
    }

    public void UploadMinimapInfo(){
        if(floorManagements[_currentFloor - 1] == null || _player == null || _commonPlayerVariable == null || mapObjects == null)
        {
            throw new System.ArgumentException("FloorManagement, Player, CommonPlayerVariable, MapObjects must not be null.");
        }
        if(!_commonPlayerVariable.isPlayerInRoom)
        {
            // 通路上にプレイヤーがいる場合、周囲5×5マスを描画するようにする
            minimapController.SetMinimapMaskInPath(floorManagements[_currentFloor - 1], _player.transform.position.x, _player.transform.position.z);
            return;
        }
        // 部屋に入った場合、部屋の中を描画するようにする
        // 最初に現在いる部屋の情報を取得する
        foreach(Room room in mapObjects.RoomColliderObjectList.Keys)
        {
            if(mapObjects.RoomColliderObjectList[room].GetInstanceID() == _commonPlayerVariable.currentRoomCollider.gameObject.GetInstanceID())
            {
                minimapController.SetMinimapMaskInRoom(floorManagements[_currentFloor - 1], room);
                return;
            }
        }
    }

    public void UploadMinimap(){
        if(floorManagements[_currentFloor - 1] == null)
        {
            throw new System.ArgumentException("FloorManagement must not be null.");
        }
        for(int y = 0; y < CommonConst.MapHeight; y++)
        {
            for(int x = 0; x < CommonConst.MapWidth; x++)
            {
                if(floorManagements[_currentFloor - 1].CreateDungeon.Map[y, x] == CommonConst.DungeonArea && floorManagements[_currentFloor - 1].MinimapMask[y, x])
                {
                    mapObjects.ChangeMinimapObjectActive(x, y, true);
                }
            }
        }
    }

    public FloorManagement GetCurrentFloorManagement()
    {
        return floorManagements[_currentFloor - 1];
    }
}
