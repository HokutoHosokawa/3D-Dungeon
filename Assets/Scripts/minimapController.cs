using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimapController
{
    // minimapのマスクを設定するクラス
    public static void SetMinimapMaskInPath(FloorManagement floorManagement, float playerX, float playerY)
    {
        // 通路上にプレイヤーがいる場合、前後計5マスを星型に描画するようにする
        // ただし、プレイヤーのいる位置から壁一枚隔てている場所は描画しない
        int playerXIndex = Mathf.FloorToInt(playerX + 0.5f);
        int playerYIndex = Mathf.FloorToInt(playerY + 0.5f);
        for(int deltaY = -2; deltaY <= 2; deltaY++)
        {
            for(int tempX = 0; tempX < 5 - 2 * Mathf.Abs(deltaY); tempX++)
            {
                int deltaX = tempX - 2 + Mathf.Abs(deltaY);
                int x = playerXIndex + deltaX;
                int y = playerYIndex + deltaY;
                if(x < 0 || x >= CommonConst.MapWidth || y < 0 || y >= CommonConst.MapHeight)
                {
                    continue;
                }
                if((deltaX == -2 && floorManagement.CreateDungeon.Map[y, x + 1] == CommonConst.EmptyArea) ||
                    (deltaX == 2 && floorManagement.CreateDungeon.Map[y, x - 1] == CommonConst.EmptyArea) ||
                    (deltaY == -2 && floorManagement.CreateDungeon.Map[y + 1, x] == CommonConst.EmptyArea) ||
                    (deltaY == 2 && floorManagement.CreateDungeon.Map[y - 1, x] == CommonConst.EmptyArea))
                {
                    // 壁が一枚隔てている場合は描画しない
                    continue;
                }
                floorManagement.SetMinimapMask(x, y, true);
            }
        }
    }

    public static void SetMinimapMaskInRoom(FloorManagement floorManagement, Room room)
    {
        // プレイヤーが部屋にいる場合、部屋の中とその周り1マスを描画するようにする
        for(int y = room.UpperLeftPosition.y - 1; y <= room.UpperLeftPosition.y + room.Size.y; y++)
        {
            for(int x = room.UpperLeftPosition.x - 1; x <= room.UpperLeftPosition.x + room.Size.x; x++)
            {
                if(x < 0 || x >= CommonConst.MapWidth || y < 0 || y >= CommonConst.MapHeight)
                {
                    continue;
                }
                floorManagement.SetMinimapMask(x, y, true);
            }
        }
    }
}
