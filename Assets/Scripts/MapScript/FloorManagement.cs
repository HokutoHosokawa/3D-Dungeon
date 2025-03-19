using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 階層の情報を持つクラス
public class FloorManagement
{
    const int StairMinDistanceFromRoomEdge = 2;         // 階段の部屋の端からの最小距離
    private readonly int _floor;                         // 階層数
    private readonly Material _wallFloorMaterial;        // 壁や床のマテリアル
    private readonly CreateDungeon _createDungeon;       // ダンジョン生成クラス
    private readonly int _floorClearRoomIndex;           // 階層クリアの部屋のインデックス
    private readonly Vector2Int _floorClearPosition;     // 階層クリアの部屋の中の座標
    private readonly int _stairDirection;                // 階段の方向
    private readonly bool[,] _minimapMask;               // ミニマップのマスク

    public FloorManagement(int floor, Material material)
    {
        if(floor < CommonConst.MinFloor)
        {
            throw new System.ArgumentException("Floor must be greater than CommonConst.MinFloor.");
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
        _floorClearRoomIndex = Random.Range(CommonConst.MinIndex, _createDungeon.Rooms.Count);
        Room floorClearRoom = _createDungeon.Rooms[_floorClearRoomIndex];
        // 部屋の端2マスには階段を作成しない
        _floorClearPosition = new Vector2Int(
            Random.Range(StairMinDistanceFromRoomEdge, floorClearRoom.Size.x - StairMinDistanceFromRoomEdge),
            Random.Range(StairMinDistanceFromRoomEdge, floorClearRoom.Size.y - StairMinDistanceFromRoomEdge)
        );
        _stairDirection = Random.Range(CommonConst.DirectionMin, CommonConst.DirectionMax + 1);
        _minimapMask = new bool[CommonConst.MapHeight, CommonConst.MapWidth];
        for(int y = CommonConst.MinPos; y < CommonConst.MapHeight; y++)
        {
            for(int x = CommonConst.MinPos; x < CommonConst.MapWidth; x++)
            {
                _minimapMask[y, x] = false;
            }
        }
    }

    public void SetMinimapMask(int x, int y, bool value)
    {
        if(x < CommonConst.MinPos || x >= CommonConst.MapWidth)
        {
            throw new System.ArgumentException("x must be greater than or equal to 0 and less than CommonConst.Width.");
        }
        if(y < CommonConst.MinPos || y >= CommonConst.MapHeight)
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
