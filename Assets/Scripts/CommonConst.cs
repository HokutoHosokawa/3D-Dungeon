using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonConst
{
    // 共通の定数を定義する
    public static readonly int MinPos = 0;              // 最小の位置
    public static readonly int MinScale = 0;            // 最小のスケール
    public static readonly int MinIndex = 0;            // 最小のインデックス
    public static readonly int MinFloor = 1;            // 最小の階層数
    public static readonly int MaxFloor = 2;            // 階層数
    public static readonly int MapWidth = 70;           // 盤面のサイズの横幅
    public static readonly int MapHeight = 70;          // 盤面のサイズの縦幅
    public static readonly int DungeonArea = 1;         // マップ上のダンジョンの場所
    public static readonly int EmptyArea = 0;           // マップ上の空きスペース
    public static readonly int DirectionMin = 0;        // 方向の最小値
    public static readonly int DirectionMax = 3;        // 方向の最大値
    public static readonly int HorizontalDirection = 1; // 水平方向
    public static readonly int VerticalDirection = 0;   // 垂直方向
    public static readonly int UpDirection = 0;         // 上方向
    public static readonly int RightDirection = 1;      // 右方向
    public static readonly int DownDirection = 2;       // 下方向
    public static readonly int LeftDirection = 3;       // 左方向
    public static readonly float FloorWidth = 1.5f;     // 床の横幅
    public static readonly float FloorHeight = 1.5f;    // 床の縦幅
    public static readonly float FloorThickness = 0.2f; // 床の厚み(エンティティのy座標を決定するため)
}
