using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 部屋の情報を持つクラス
// 部屋の左上の座標と部屋のサイズを持つ
public class Room
{
    readonly Vector2Int _upperLeftPosition;
    readonly Vector2Int _size;

    public Room(Vector2Int upperLeftPosition, Vector2Int size)
    {
        if(size.x <= 0 || size.y <= 0)
        {
            throw new System.ArgumentException("size must be positive");
        }

        if(upperLeftPosition.x < 0 || upperLeftPosition.y < 0)
        {
            throw new System.ArgumentException("upperLeftPosition must be positive");
        }
        _upperLeftPosition = upperLeftPosition;
        _size = size;
    }

    public Vector2Int UpperLeftPosition => _upperLeftPosition;
    public Vector2Int Size => _size;
}
