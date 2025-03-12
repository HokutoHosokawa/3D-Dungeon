using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 階層の情報を持つクラス
public class FloorManagement
{
    private readonly int _floor;                         // 階層数
    private readonly Material _wallFloorMaterial;        // 壁や床のマテリアル
    private readonly CreateDungeon _createDungeon;       // ダンジョン生成クラス
    private readonly int _floorClearRoomIndex;           // 階層クリアの部屋のインデックス
    private readonly Vector2Int _floorClearPosition;     // 階層クリアの部屋の中の座標
    private readonly int _stairDirection;                // 階段の方向
    private readonly bool[,] _minimapMask;               // ミニマップのマスク

    public FloorManagement(int floor, Material material)
    {
        if(floor <= 0)
        {
            throw new System.ArgumentException("Floor must be greater than 0.");
        }
        if(floor > CommonConst.MaxFloor)
        {
            throw new System.ArgumentException("Floor must be less than or equal to CommonConst.MaxFloor.");
        }
        if(material == null)
        {
            throw new System.ArgumentException("Material must not be null.");
        }
        _floor = floor;
        _wallFloorMaterial = material;
        _createDungeon = new CreateDungeon();
        _floorClearRoomIndex = Random.Range(0, _createDungeon.Rooms.Count);
        Room floorClearRoom = _createDungeon.Rooms[_floorClearRoomIndex];
        // 部屋の端2マスには階段を作成しない
        _floorClearPosition = new Vector2Int(
            Random.Range(2, floorClearRoom.Size.x - 2),
            Random.Range(2, floorClearRoom.Size.y - 2)
        );
        _stairDirection = Random.Range(0, 4);
        _minimapMask = new bool[CommonConst.MapHeight, CommonConst.MapWidth];
        for(int y = 0; y < CommonConst.MapHeight; y++)
        {
            for(int x = 0; x < CommonConst.MapWidth; x++)
            {
                _minimapMask[y, x] = false;
            }
        }
    }

    public void SetMinimapMask(int x, int y, bool value)
    {
        if(x < 0 || x >= CommonConst.MapWidth)
        {
            throw new System.ArgumentException("x must be greater than or equal to 0 and less than CommonConst.Width.");
        }
        if(y < 0 || y >= CommonConst.MapHeight)
        {
            throw new System.ArgumentException("y must be greater than or equal to 0 and less than CommonConst.Height.");
        }
        _minimapMask[y, x] = value;
    }

    public int Floor => _floor;
    public Material WallFloorMaterial => _wallFloorMaterial;
    public CreateDungeon CreateDungeon => _createDungeon;
    public int FloorClearRoomIndex => _floorClearRoomIndex;
    public Vector2Int FloorClearPosition => _floorClearPosition;
    public int StairDirection => _stairDirection;
    public bool[,] MinimapMask => _minimapMask;
}
