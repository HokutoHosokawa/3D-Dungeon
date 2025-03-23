using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PathDirectionのペアでパスの情報を持つクラス
public class Path
{
    // パスの情報を持つクラス
    public class PathDirection
    {
        readonly Room _room; // パスが作られる部屋
        readonly int _roomIndex; // パスが作られる部屋のインデックス
        readonly int _direction; // 0:上, 1:右, 2:下, 3:左
        readonly int _pathStartPos; // パスの開始位置

        public PathDirection(Room room, int roomIndex, int direction, int pathStartPos)
        {
            if(direction < CommonConst.DirectionMin || direction > CommonConst.DirectionMax)
            {
                throw new System.ArgumentException("direction must be 0 to 3");
            }

            if(pathStartPos < CommonConst.MinPos || (pathStartPos >= room.Size.x && direction % 2 == CommonConst.VerticalDirection) || (pathStartPos >= room.Size.y && direction % 2 == CommonConst.HorizontalDirection))
            {
                throw new System.ArgumentException("pathStartPos is out of range");
            }

            if(room == null)
            {
                throw new System.ArgumentNullException("room is null");
            }

            if(roomIndex < CommonConst.MinIndex)
            {
                throw new System.ArgumentException("roomIndex must be positive");
            }
            _room = room;
            _roomIndex = roomIndex;
            _direction = direction;
            _pathStartPos = pathStartPos;
        }

        public Room TargetRoom => _room;
        public int RoomIndex => _roomIndex;
        public int Direction => _direction;
        public int PathStartPos => _pathStartPos;
    }

    readonly PathDirection[] _pathDirections = new PathDirection[2]; // パスの情報
    readonly int _pathCurvePos; // パスの曲がる位置 (上と左のように上下と左右で曲がる場合は0、それ以外(上と下など)は曲がる位置を表す。ただし、曲がる位置は_pathDirections[0]からの位置を表す)

    public Path(Room room1, int room1Index, int direction1, int pathStartPos1, Room room2, int room2Index, int direction2, int pathStartPos2, int pathCurvePos)
    {
        if(room1 == null || room2 == null)
        {
            throw new System.ArgumentNullException("room is null");
        }

        if(room1Index < CommonConst.MinIndex || room2Index < CommonConst.MinIndex)
        {
            throw new System.ArgumentException("roomIndex must be positive");
        }

        if(direction1 < CommonConst.DirectionMin || direction1 > CommonConst.DirectionMax || direction2 < CommonConst.DirectionMin || direction2 > CommonConst.DirectionMax)
        {
            throw new System.ArgumentException("direction must be 0 to 3");
        }

        if(pathCurvePos < CommonConst.MinPos)
        {
            throw new System.ArgumentException("pathCurvePos is out of range");
        }

        if(direction1 % 2 != direction2 % 2 && pathCurvePos != CommonConst.MinPos)
        {
            throw new System.ArgumentException("pathCurvePos is out of range");
        }

        _pathDirections[0] = new PathDirection(room1, room1Index, direction1, pathStartPos1);
        _pathDirections[1] = new PathDirection(room2, room2Index, direction2, pathStartPos2);
        _pathCurvePos = pathCurvePos;
    }

    public PathDirection[] PathDirections => _pathDirections;
    public int PathCurvePos => _pathCurvePos;

    public static bool operator== (Path path1, Path path2)
    {
        // パスが出る部屋とつながる部屋が同じかどうか確認する
        if((path1.PathDirections[0].TargetRoom == path2.PathDirections[0].TargetRoom && path1.PathDirections[1].TargetRoom == path2.PathDirections[1].TargetRoom) || (path1.PathDirections[0].TargetRoom == path2.PathDirections[1].TargetRoom && path1.PathDirections[1].TargetRoom == path2.PathDirections[0].TargetRoom))
        {
            return true;
        }
        return false;
    }

    public static bool operator!= (Path path1, Path path2)
    {
        // パスが出る部屋とつながる部屋が異なるかどうか確認する
        return !(path1 == path2);
    }

    public override bool Equals(object obj)
    {
        if (obj is Path other)
        {
            return ((this.PathDirections[0].TargetRoom == other.PathDirections[0].TargetRoom && this.PathDirections[1].TargetRoom == other.PathDirections[1].TargetRoom) || (this.PathDirections[0].TargetRoom == other.PathDirections[1].TargetRoom && this.PathDirections[1].TargetRoom == other.PathDirections[0].TargetRoom));
        }
        return false;
    }

    public override int GetHashCode()
    {
        // 部屋のハッシュコードを使って一意の値を作成
        int hash1 = PathDirections[0].TargetRoom.GetHashCode();
        int hash2 = PathDirections[1].TargetRoom.GetHashCode();

        // 順序が逆でも同じハッシュになるように、XOR を使用
        return hash1 ^ hash2;
    }
}
