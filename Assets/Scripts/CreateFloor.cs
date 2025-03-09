using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 階層の作成、描画を行うクラス
public class CreateFloor : MonoBehaviour
{
    [SerializeField] GameObject _player;
    private int _currentFloor = 1;
    private FloorManagement[] floorManagements = new FloorManagement[CommonConst.MaxFloor];
    // Start is called before the first frame update
    void Start()
    {
        CreateNewFloor(_currentFloor);
    }

    public void CreateNewFloor(int currentFloor)
    {
        if(currentFloor <= 0)
        {
            throw new System.ArgumentException("Floor must be greater than 0.");
        }
        if(currentFloor > CommonConst.MaxFloor)
        {
            throw new System.ArgumentException("Floor must be less than or equal to CommonConst.MaxFloor.");
        }
        if(floorManagements[currentFloor - 1] != null)
        {
            // 既に作成済みの階層は作成しない
            return;
        }
        // 最終階層以外は階段を作成
        Material mat = Resources.Load<Material>("Materials/StoneWall");
        floorManagements[currentFloor - 1] = new FloorManagement(currentFloor, mat);
        CreateRoom3DView.ViewStart(floorManagements[currentFloor - 1]);
        // プレイヤーの初期位置を設定
        int playerStartRoomIndex = Random.Range(0, floorManagements[currentFloor - 1].CreateDungeon.Rooms.Count - 1);
        if(playerStartRoomIndex >= floorManagements[currentFloor - 1].FloorClearRoomIndex)
        {
            // クリアルームと同じ部屋にプレイヤーがいないようにする
            playerStartRoomIndex++;
        }
        Room playerStartRoom = floorManagements[currentFloor - 1].CreateDungeon.Rooms[playerStartRoomIndex];
        _player.transform.position = new Vector3(playerStartRoom.UpperLeftPosition.x + playerStartRoom.Size.x / 2.0f, 0.1f, playerStartRoom.UpperLeftPosition.y + playerStartRoom.Size.y / 2.0f);
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
