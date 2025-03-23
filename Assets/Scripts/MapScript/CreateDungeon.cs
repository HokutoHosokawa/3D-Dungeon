using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateDungeon
{
    readonly int MinRoom = 5;                                  // 最小の部屋の個数
    readonly int MaxRoom = 15;                                 // 最大の部屋の個数
    readonly int MinRoomWidth = 5;                             // 部屋の最小の横幅
    readonly int MinRoomHeight = 5;                            // 部屋の最小の縦幅
    readonly int MaxRoomWidth = 30;                            // 部屋の最大の横幅
    readonly int MaxRoomHeight = 30;                           // 部屋の最大の縦幅
    readonly int MaxRoomArea = 300;                            // 部屋の最大の面積
    readonly int MinRoomDistance = 5;                          // 部屋同士の最小の距離
    readonly int MinCurvePos = 1;                              // パスの曲がる位置の最小値
    readonly int MapWidth = CommonConst.MapWidth;              // 盤面のサイズの横幅
    readonly int MapHeight = CommonConst.MapHeight;            // 盤面のサイズの縦幅
    readonly int UpDirection = CommonConst.UpDirection;        // 上方向
    readonly int RightDirection = CommonConst.RightDirection;  // 右方向
    readonly int DownDirection = CommonConst.DownDirection;    // 下方向
    readonly int LeftDirection = CommonConst.LeftDirection;    // 左方向


    List<Room> rooms;
    List<Path> paths;
    int[,] map;

    public List<Room> Rooms => rooms;
    public List<Path> Paths => paths;
    public int[,] Map => map;

    public CreateDungeon(){
        map  = new int[MapHeight, MapWidth];
        Create();
    }

    private void MapView(int[,] map){
        string mapString = "";
        for(int y = 0; y < MapHeight; ++y){
            for(int x = 0; x < MapWidth; ++x){
                if(map[y, x] == 1){
                    mapString += "■";
                }else{
                    mapString += "□";
                }
            }
            mapString += "\n";
        }
        Debug.Log(mapString);
    }

    // DFSをする関数
    private List<bool> DFS(List<List<int>> graph, List<bool> visited, int node){
        visited[node] = true;
        foreach(int nextNode in graph[node]){
            if(!visited[nextNode]){
                DFS(graph, visited, nextNode);
            }
        }
        return visited;
    }

    // すべての部屋が繋がっているかどうかを確認する関数
    private List<bool> IsConnectedAllRooms(IReadOnlyList<Room> rooms, IReadOnlyList<Path> paths){
        // rooms : 部屋のリスト
        // paths : パスのリスト
        List<List<int>> graph = new List<List<int>>();
        for(int i = CommonConst.MinIndex; i < rooms.Count; ++i){
            graph.Add(new List<int>());
        }
        foreach(Path path in paths){
            graph[path.PathDirections[0].RoomIndex].Add(path.PathDirections[1].RoomIndex);
            graph[path.PathDirections[1].RoomIndex].Add(path.PathDirections[0].RoomIndex);
        }
        List<bool> visited = new List<bool>();
        for(int i = CommonConst.MinIndex; i < rooms.Count; ++i){
            visited.Add(false);
        }
        // 0番目の部屋からDFSをする
        visited = DFS(graph, visited, CommonConst.MinIndex);
        return visited;
    }


    // ダンジョンを作成するアルゴリズム
    private void Create()
    {
        rooms = FindRoomsPosition(); // 部屋の個数を気にせずに作成する
        // 部屋の個数がMinRoom以下の場合は再生成
        while(rooms.Count < MinRoom){
            rooms = FindRoomsPosition();
        }
        // 部屋の個数がMaxRoomを超える場合は超えた分をランダムに削除
        while(rooms.Count > MaxRoom){
            int removeIndex = Random.Range(CommonConst.MinIndex, rooms.Count);
            rooms.RemoveAt(removeIndex);
        }
        var tuppleData = CreatePath(rooms);     // 部屋同士のパスを作成する
        paths = tuppleData.Item1;
        List<List<RoomDistanceWithIndex>> roomDistancesWithIndex = tuppleData.Item2; // 部屋同士の距離を持つリスト
        // ここでは作成されたパスがすべてのノードを繋いでいるかどうかをDFSで確認する
        List<bool> isConnectedAllRooms = IsConnectedAllRooms(rooms, paths);
        while(isConnectedAllRooms.Contains(false)){
            // すべての部屋が繋がっていない場合は繋がるまで新たなパスを追加する
            // 新たなパスはつながっている部屋とつながっていない部屋の組み合わせの内最小の距離の組み合わせを選ぶ
            int minDistance = int.MaxValue;
            int room1Index = -1;
            int room2Index = -1;
            List<int> connectedRoomIndexes = new List<int>();
            List<int> notConnectedRoomIndexes = new List<int>();
            for(int i = CommonConst.MinIndex; i < isConnectedAllRooms.Count; ++i){
                if(isConnectedAllRooms[i]){
                    connectedRoomIndexes.Add(i);
                }else{
                    notConnectedRoomIndexes.Add(i);
                }
            }
            foreach(int connectedRoomIndex in connectedRoomIndexes){
                foreach(int notConnectedRoomIndex in notConnectedRoomIndexes){
                    if(roomDistancesWithIndex[connectedRoomIndex][notConnectedRoomIndex].Distance < minDistance){
                        minDistance = roomDistancesWithIndex[connectedRoomIndex][notConnectedRoomIndex].Distance;
                        room1Index = connectedRoomIndex;
                        room2Index = notConnectedRoomIndex;
                    }
                }
            }
            paths.Add(MakePath(rooms, room1Index, room2Index));
            isConnectedAllRooms = IsConnectedAllRooms(rooms, paths);
        }


        for(int y = CommonConst.MinPos; y < MapHeight; ++y){
            for(int x = CommonConst.MinPos; x < MapWidth; ++x){
                map[y, x] = CommonConst.EmptyArea;
            }
        }
        foreach(Room room in rooms){
            for(int y = room.UpperLeftPosition.y; y < room.UpperLeftPosition.y + room.Size.y; ++y){
                for(int x = room.UpperLeftPosition.x; x < room.UpperLeftPosition.x + room.Size.x; ++x){
                    map[y, x] = CommonConst.DungeonArea;
                }
            }
        }
        foreach(Path path in paths){
            int room1PathStartPos = path.PathDirections[0].PathStartPos;
            int room2PathStartPos = path.PathDirections[1].PathStartPos;
            int room1Direction = path.PathDirections[0].Direction;
            int room2Direction = path.PathDirections[1].Direction;
            int room1X = path.PathDirections[0].TargetRoom.UpperLeftPosition.x;
            int room1Y = path.PathDirections[0].TargetRoom.UpperLeftPosition.y;
            int room2X = path.PathDirections[1].TargetRoom.UpperLeftPosition.x;
            int room2Y = path.PathDirections[1].TargetRoom.UpperLeftPosition.y;
            int room1Width = path.PathDirections[0].TargetRoom.Size.x;
            int room1Height = path.PathDirections[0].TargetRoom.Size.y;
            int room2Width = path.PathDirections[1].TargetRoom.Size.x;
            int room2Height = path.PathDirections[1].TargetRoom.Size.y;
            if(path.PathCurvePos == CommonConst.MinPos){
                // 上下と左右でパスが繋がっている場合
                if(room1Direction == UpDirection){
                    int x = room1X + room1PathStartPos;
                    int y = room2Y + room2PathStartPos;
                    if(room2Direction == RightDirection){
                        for(int i = room2X + room2Width; i < x; ++i){
                            map[y, i] = CommonConst.DungeonArea;
                        }
                        for(int i = y; i < room1Y; ++i){
                            map[i, x] = CommonConst.DungeonArea;
                        }
                    }else if(room2Direction == LeftDirection){
                        for(int i = x; i < room2X; ++i){
                            map[y, i] = CommonConst.DungeonArea;
                        }
                        for(int i = y; i < room1Y; ++i){
                            map[i, x] = CommonConst.DungeonArea;
                        }
                    }
                }else if(room1Direction == DownDirection){
                    int x = room1X + room1PathStartPos;
                    int y = room2Y + room2PathStartPos;
                    if(room2Direction == RightDirection){
                        for(int i = room2X + room2Width; i < x; ++i){
                            map[y, i] = CommonConst.DungeonArea;
                        }
                        for(int i = room1Y + room1Height; i <= y; ++i){
                            map[i, x] = CommonConst.DungeonArea;
                        }
                    }else if(room2Direction == LeftDirection){
                        for(int i = x; i < room2X; ++i){
                            map[y, i] = CommonConst.DungeonArea;
                        }
                        for(int i = room1Y + room1Height; i < y; ++i){
                            map[i, x] = CommonConst.DungeonArea;
                        }
                    }
                }else if(room1Direction == RightDirection){
                    int x = room2X + room2PathStartPos;
                    int y = room1Y + room1PathStartPos;
                    if(room2Direction == UpDirection){
                        for(int i = y; i < room2Y; ++i){
                            map[i, x] = CommonConst.DungeonArea;
                        }
                        for(int i = room1X + room1Width; i < x; ++i){
                            map[y, i] = CommonConst.DungeonArea;
                        }
                    }else if(room2Direction == DownDirection){
                        for(int i = room2Y + room2Height; i <= y; ++i){
                            map[i, x] = CommonConst.DungeonArea;
                        }
                        for(int i = room1X + room1Width; i < x; ++i){
                            map[y, i] = CommonConst.DungeonArea;
                        }
                    }
                }else if(room1Direction == LeftDirection){
                    int x = room2X + room2PathStartPos;
                    int y = room1Y + room1PathStartPos;
                    if(room2Direction == UpDirection){
                        for(int i = y; i < room2Y; ++i){
                            map[i, x] = CommonConst.DungeonArea;
                        }
                        for(int i = x; i < room1X; ++i){
                            map[y, i] = CommonConst.DungeonArea;
                        }
                    }else if(room2Direction == DownDirection){
                        for(int i = room2Y + room2Height; i < y; ++i){
                            map[i, x] = CommonConst.DungeonArea;
                        }
                        for(int i = x; i < room1X; ++i){
                            map[y, i] = CommonConst.DungeonArea;
                        }
                    }
                }
            } else {
                // 上と下、下と上、右と左、左と右のいずれかでパスが繋がっている場合
                // PathCurvePosの位置で曲がる
                if(room1Direction == UpDirection || room1Direction == DownDirection){
                    // 上下方向でつながる場合
                    // room1が上にある場合
                    int UpperRoomDirection = room1Direction;
                    int LowerRoomDirection = room2Direction;
                    int UpperRoomX = room1X;
                    int UpperRoomY = room1Y;
                    int LowerRoomX = room2X;
                    int LowerRoomY = room2Y;
                    int UpperRoomWidth = room1Width;
                    int UpperRoomHeight = room1Height;
                    int LowerRoomWidth = room2Width;
                    int LowerRoomHeight = room2Height;
                    int UpperRoomPathStartPos = room1PathStartPos;
                    int LowerRoomPathStartPos = room2PathStartPos;
                    int CurvePosFromUpperRoom = path.PathCurvePos;

                    if(room1Direction == UpDirection){
                        // room1が下にある場合
                        UpperRoomDirection = room2Direction;
                        LowerRoomDirection = room1Direction;
                        UpperRoomX = room2X;
                        UpperRoomY = room2Y;
                        LowerRoomX = room1X;
                        LowerRoomY = room1Y;
                        UpperRoomWidth = room2Width;
                        UpperRoomHeight = room2Height;
                        LowerRoomWidth = room1Width;
                        LowerRoomHeight = room1Height;
                        UpperRoomPathStartPos = room2PathStartPos;
                        LowerRoomPathStartPos = room1PathStartPos;
                        CurvePosFromUpperRoom = LowerRoomY - (UpperRoomY + UpperRoomHeight) - CurvePosFromUpperRoom - 1;
                    }
                    int UpperXPos = UpperRoomX + UpperRoomPathStartPos;
                    int LowerXPos = LowerRoomX + LowerRoomPathStartPos;
                    if(UpperXPos < LowerXPos){
                        int i;
                        for(i = UpperRoomY + UpperRoomHeight; i < UpperRoomY + UpperRoomHeight + CurvePosFromUpperRoom; ++i){
                            map[i, UpperXPos] = CommonConst.DungeonArea;
                        }
                        for(int j = UpperXPos; j < LowerXPos; ++j){
                            map[i, j] = CommonConst.DungeonArea;
                        }
                        for(int j = i; j < LowerRoomY; ++j){
                            map[j, LowerXPos] = CommonConst.DungeonArea;
                        }
                    }else{
                        int i;
                        for(i = UpperRoomY + UpperRoomHeight; i < UpperRoomY + UpperRoomHeight + CurvePosFromUpperRoom; ++i){
                            map[i, UpperXPos] = CommonConst.DungeonArea;
                        }
                        for(int j = UpperXPos; j > LowerXPos; --j){
                            map[i, j] = CommonConst.DungeonArea;
                        }
                        for(int j = i; j < LowerRoomY; ++j){
                            map[j, LowerXPos] = CommonConst.DungeonArea;
                        }
                    }
                }else if(room1Direction == RightDirection || room1Direction == LeftDirection){
                    // 左右方向でつながる場合
                    // room1が左にある場合
                    int LeftRoomDirection = room1Direction;
                    int RightRoomDirection = room2Direction;
                    int LeftRoomX = room1X;
                    int LeftRoomY = room1Y;
                    int RightRoomX = room2X;
                    int RightRoomY = room2Y;
                    int LeftRoomWidth = room1Width;
                    int LeftRoomHeight = room1Height;
                    int RightRoomWidth = room2Width;
                    int RightRoomHeight = room2Height;
                    int LeftRoomPathStartPos = room1PathStartPos;
                    int RightRoomPathStartPos = room2PathStartPos;
                    int CurvePosFromLeftRoom = path.PathCurvePos;
                    if(room1Direction == LeftDirection){
                        // room1が右にある場合
                        LeftRoomDirection = room2Direction;
                        RightRoomDirection = room1Direction;
                        LeftRoomX = room2X;
                        LeftRoomY = room2Y;
                        RightRoomX = room1X;
                        RightRoomY = room1Y;
                        LeftRoomWidth = room2Width;
                        LeftRoomHeight = room2Height;
                        RightRoomWidth = room1Width;
                        RightRoomHeight = room1Height;
                        LeftRoomPathStartPos = room2PathStartPos;
                        RightRoomPathStartPos = room1PathStartPos;
                        CurvePosFromLeftRoom = RightRoomX - (LeftRoomX + LeftRoomWidth) - CurvePosFromLeftRoom - 1;
                    }
                    int LeftYPos = LeftRoomY + LeftRoomPathStartPos;
                    int RightYPos = RightRoomY + RightRoomPathStartPos;
                    if(LeftYPos < RightYPos){
                        int i;
                        for(i = LeftRoomX + LeftRoomWidth; i < LeftRoomX + LeftRoomWidth + CurvePosFromLeftRoom; ++i){
                            map[LeftYPos, i] = CommonConst.DungeonArea;
                        }
                        for(int j = LeftYPos; j < RightYPos; ++j){
                            map[j, i] = CommonConst.DungeonArea;
                        }
                        for(int j = i; j < RightRoomX; ++j){
                            map[RightYPos, j] = CommonConst.DungeonArea;
                        }
                    }else{
                        int i;
                        for(i = LeftRoomX + LeftRoomWidth; i < LeftRoomX + LeftRoomWidth + CurvePosFromLeftRoom; ++i){
                            map[LeftYPos, i] = CommonConst.DungeonArea;
                        }
                        for(int j = LeftYPos; j > RightYPos; --j){
                            map[j, i] = CommonConst.DungeonArea;
                        }
                        for(int j = i; j < RightRoomX; ++j){
                            map[RightYPos, j] = CommonConst.DungeonArea;
                        }
                    }
                }
            }
        }
        MapView(map);
    }

    // FindRoomsPosition関数で用いる既存の部屋との距離を計算し、有効かどうか判定する関数
    private bool IsValidRoom(Room newRoom, IEnumerable<Room> rooms){
        // newRoom : 新しく生成する部屋
        // rooms : 既存の部屋のリスト
        foreach(Room room in rooms){
            int dx = Mathf.Max(new []{newRoom.UpperLeftPosition.x - (room.UpperLeftPosition.x + room.Size.x), room.UpperLeftPosition.x - (newRoom.UpperLeftPosition.x + newRoom.Size.x), 0});
            int dy = Mathf.Max(new []{newRoom.UpperLeftPosition.y - (room.UpperLeftPosition.y + room.Size.y), room.UpperLeftPosition.y - (newRoom.UpperLeftPosition.y + newRoom.Size.y), 0});
            if(dx == 0 && dy == 0){
                // 重なっている部屋がある場合
                return false;
            }
            if(dx + dy < MinRoomDistance){
                // 部屋同士の距離(マンハッタン距離)が最小距離より小さい場合
                return false;
            }
        }
        return true;
    }

    // Poisson Disk Samplingを応用する
    // 部屋の左上の座標と部屋のサイズから部屋同士の距離を計算
    // これで条件を満たす部屋を生成する
    private List<Room> FindRoomsPosition(int k = 30){
        // r : 部屋同士の最小距離
        // k : 最初に生成する部屋の候補点の個数

        int firstRoomPositionX = Random.Range(CommonConst.MinPos, MapWidth - MinRoomWidth);
        int firstRoomPositionY = Random.Range(CommonConst.MinPos, MapHeight - MinRoomHeight);
        int firstRoomWidth = Random.Range(MinRoomWidth, MaxRoomWidth);
        int firstRoomHeight = Random.Range(MinRoomHeight, MaxRoomHeight);
        while(firstRoomPositionX + firstRoomWidth >= MapWidth || firstRoomPositionY + firstRoomHeight >= MapHeight || firstRoomWidth * firstRoomHeight >= MaxRoomArea){
            // 部屋が盤面の外に出る場合は再生成
            firstRoomPositionX = Random.Range(CommonConst.MinPos, MapWidth - MinRoomWidth);
            firstRoomPositionY = Random.Range(CommonConst.MinPos, MapHeight - MinRoomHeight);
            firstRoomWidth = Random.Range(MinRoomWidth, MaxRoomWidth);
            firstRoomHeight = Random.Range(MinRoomHeight, MaxRoomHeight);
        }

        List<Room> active = new List<Room>();
        List<Room> points = new List<Room>();
        Room firstRoom = new Room(new Vector2Int(firstRoomPositionX, firstRoomPositionY), new Vector2Int(firstRoomWidth, firstRoomHeight));
        active.Add(firstRoom);
        points.Add(firstRoom);
        // 部屋の候補点を生成
        while(active.Count > 0){
            int centerIndex = Random.Range(CommonConst.MinIndex, active.Count);
            Room centerRoom = active[centerIndex];
            Vector2 centerRoomCenter = new Vector2(centerRoom.UpperLeftPosition.x + centerRoom.Size.x / 2.0f, centerRoom.UpperLeftPosition.y + centerRoom.Size.y / 2.0f);
            bool isFind = false;
            for(int i = CommonConst.MinIndex; i < k; ++i){
                float angleToNewPoint = Random.Range(0, 2.0f * Mathf.PI);
                float distanceToNewPoint = Random.Range(MinRoomDistance, 2 * MinRoomDistance);

                Vector2Int newRoomSize = new Vector2Int(Random.Range(MinRoomWidth, MaxRoomWidth), Random.Range(MinRoomHeight, MaxRoomHeight));
                while(newRoomSize.x * newRoomSize.y >= MaxRoomArea){
                    newRoomSize = new Vector2Int(Random.Range(MinRoomWidth, MaxRoomWidth), Random.Range(MinRoomHeight, MaxRoomHeight));
                }
                // 中心となる矩形の中心座標と角度から中心となる矩形のその角度の上にある点Aを求める
                // 点Aから新しい部屋の最も近い辺までの長さ(中心と点Aを結ぶ直線と新しい矩形の交点の内、最も近い点までの距離)をdistanceToNewPointとする

                // 矩形の左右の辺と交わる点と上下の辺と交わる点までの長さを求める
                // その4点(2個)のうち距離が近い方を選ぶ
                float sideLength = Mathf.Abs((centerRoom.Size.x/2.0f) / Mathf.Cos(angleToNewPoint));    // 中心の矩形から左右の辺と交わる点までの長さ
                float upDownLength = Mathf.Abs((centerRoom.Size.y/2.0f) / Mathf.Sin(angleToNewPoint));  // 中心の矩形から上下の辺と交わる点までの長さ
                int isRight;    // 右の辺と交わるか(角度が第1象限or第4象限の場合は1, 第2象限or第3象限の場合は-1)
                int isDown;     // 下の辺と交わるか(角度が第1象限or第2象限の場合は1, 第3象限or第4象限の場合は-1)

                Vector2 newRoomCenter;

                if(Mathf.Cos(angleToNewPoint) > 0){
                    isRight = 1;
                }else{
                    isRight = -1;
                }
                if(Mathf.Sin(angleToNewPoint) > 0){
                    isDown = 1;
                }else{
                    isDown = -1;
                }
                if(sideLength < upDownLength){
                    if(Mathf.Abs(Mathf.Tan(angleToNewPoint)) < ((newRoomSize.y * 1.0) / (newRoomSize.x * 1.0))){
                        newRoomCenter = new Vector2(centerRoomCenter.x + sideLength * Mathf.Cos(angleToNewPoint) + distanceToNewPoint * Mathf.Cos(angleToNewPoint) + isRight * (newRoomSize.x / 2.0f), centerRoomCenter.y + sideLength * Mathf.Sin(angleToNewPoint) + distanceToNewPoint * Mathf.Sin(angleToNewPoint) + isDown * newRoomSize.x * Mathf.Abs(Mathf.Tan(angleToNewPoint)) / 2.0f);
                    }else{
                        newRoomCenter = new Vector2(centerRoomCenter.x + sideLength * Mathf.Cos(angleToNewPoint) + distanceToNewPoint * Mathf.Cos(angleToNewPoint) + isRight * (newRoomSize.y / Mathf.Abs((2.0f * Mathf.Tan(angleToNewPoint)))), centerRoomCenter.y + sideLength * Mathf.Sin(angleToNewPoint) + distanceToNewPoint * Mathf.Sin(angleToNewPoint) + isDown * (newRoomSize.y / 2.0f));
                    }
                }else{
                    if(Mathf.Abs(Mathf.Tan(angleToNewPoint)) < ((newRoomSize.y * 1.0) / (newRoomSize.x * 1.0))){
                        newRoomCenter = new Vector2(centerRoomCenter.x + upDownLength * Mathf.Cos(angleToNewPoint) + distanceToNewPoint * Mathf.Cos(angleToNewPoint) + isRight * (newRoomSize.x / 2.0f), centerRoomCenter.y + upDownLength * Mathf.Sin(angleToNewPoint) + distanceToNewPoint * Mathf.Sin(angleToNewPoint) + isDown * newRoomSize.x * Mathf.Abs(Mathf.Tan(angleToNewPoint)) / 2.0f);
                    }else{
                        newRoomCenter = new Vector2(centerRoomCenter.x + upDownLength * Mathf.Cos(angleToNewPoint) + distanceToNewPoint * Mathf.Cos(angleToNewPoint) + isRight * (newRoomSize.y / Mathf.Abs((2.0f * Mathf.Tan(angleToNewPoint)))), centerRoomCenter.y + upDownLength * Mathf.Sin(angleToNewPoint) + distanceToNewPoint * Mathf.Sin(angleToNewPoint) + isDown * (newRoomSize.y / 2.0f));
                    }
                }

                if(newRoomCenter.x - newRoomSize.x / 2.0f < 0 || newRoomCenter.x + newRoomSize.x / 2.0f >= MapWidth || newRoomCenter.y - newRoomSize.y / 2.0f < 0 || newRoomCenter.y + newRoomSize.y / 2.0f >= MapHeight){
                    continue;
                }

                Room newRoom = new Room(new Vector2Int((int)(newRoomCenter.x - newRoomSize.x / 2.0f), (int)(newRoomCenter.y - newRoomSize.y / 2.0f)), newRoomSize);
                if(!IsValidRoom(newRoom, points)){
                    continue;
                }

                active.Add(newRoom);
                points.Add(newRoom);
                isFind = true;
                break;
            }
            if(!isFind){
                active.RemoveAt(centerIndex);
            }
        }
    return points;
    }


    private class RoomDistance{
        // 部屋同士の方向と距離を表すクラス
        private int _roomDirections; // もう一つの部屋が -1:左or上, 0:その軸で重なっている, 1:右or下 にある
        private int _distance;       // 部屋までのその軸方向の距離
        public RoomDistance(int roomDirections, int distance){
            _roomDirections = roomDirections;
            _distance = distance;
        }

        public int RoomDirections => _roomDirections;
        public int Distance => _distance;
    }

    private class RoomDistanceWithIndex{
        private int _index;
        private int _distance;

        public RoomDistanceWithIndex(int index, int distance){
            _index = index;
            _distance = distance;
        }

        public int Index => _index;
        public int Distance => _distance;
    }

    // x方向の部屋同士の距離を計算する関数
    private RoomDistance CalculateXRoomDistance(Room room1, Room room2){
        // room1 : 1番目の部屋
        // room2 : 2番目の部屋
        int xDistance = Mathf.Max(new []{room1.UpperLeftPosition.x - (room2.UpperLeftPosition.x + room2.Size.x), room2.UpperLeftPosition.x - (room1.UpperLeftPosition.x + room1.Size.x), 0});
        if(xDistance == 0){
            return new RoomDistance(0, 0);
        }else if(room1.UpperLeftPosition.x < room2.UpperLeftPosition.x){
            return new RoomDistance(1, xDistance);
        }else{
            return new RoomDistance(-1, xDistance);
        }
    }

    // y方向の部屋同士の距離を計算する関数
    private RoomDistance CalculateYRoomDistance(Room room1, Room room2){
        // room1 : 1番目の部屋
        // room2 : 2番目の部屋
        int yDistance = Mathf.Max(new []{room1.UpperLeftPosition.y - (room2.UpperLeftPosition.y + room2.Size.y), room2.UpperLeftPosition.y - (room1.UpperLeftPosition.y + room1.Size.y), 0});
        if(yDistance == 0){
            return new RoomDistance(0, 0);
        }else if(room1.UpperLeftPosition.y < room2.UpperLeftPosition.y){
            return new RoomDistance(1, yDistance);
        }else{
            return new RoomDistance(-1, yDistance);
        }
    }

    // 部屋同士のパスを生成する関数
    private Path MakePath(IReadOnlyList<Room> rooms, int room1Index, int room2Index){
        // rooms : 部屋のリスト
        // room1Index : 1番目の部屋のインデックス
        // room2Index : 2番目の部屋のインデックス
        // xRoomDistance : x方向の部屋同士の距離と方向
        // yRoomDistance : y方向の部屋同士の距離と方向
        // xRoomDirection : x方向の部屋同士の関係 (0:重なっている, 1:右にある, -1:左にある)
        // yRoomDirection : y方向の部屋同士の関係 (0:重なっている, 1:下にある, -1:上にある)
        RoomDistance xRoomDistance = CalculateXRoomDistance(rooms[room1Index], rooms[room2Index]);
        RoomDistance yRoomDistance = CalculateYRoomDistance(rooms[room1Index], rooms[room2Index]);

        int xRoomDirection = xRoomDistance.RoomDirections;
        int yRoomDirection = yRoomDistance.RoomDirections;
        int room1Direction, room2Direction;
        int room1PathStartPos, room2PathStartPos;
        int curvePos;
        if(xRoomDirection == 0){
            // x方向が重なっている場合
            if(yRoomDirection == 0){
                // 1番目に近い部屋との距離が0の場合
                throw new System.Exception("1番目に近い部屋との距離が0です");
            }
            if(yRoomDirection == 1){
                // y方向でもう一つの部屋が下にある場合
                room1Direction = DownDirection;
                room2Direction = UpDirection;
                room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                curvePos = Random.Range(MinCurvePos, yRoomDistance.Distance - MinCurvePos);
            }else{
                // y方向でもう一つの部屋が上にある場合
                room1Direction = UpDirection;
                room2Direction = DownDirection;
                room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                curvePos = Random.Range(MinCurvePos, yRoomDistance.Distance - MinCurvePos);
            }
        }else if(xRoomDirection == 1){
            // x方向でもう一つの部屋が右にある場合
            if(yRoomDirection == 0){
                // y方向が重なっている場合
                room1Direction = RightDirection;
                room2Direction = LeftDirection;
                room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                curvePos = Random.Range(MinCurvePos, xRoomDistance.Distance - MinCurvePos);
            }else if(yRoomDirection == 1){
                // y方向でもう一つの部屋が下にある場合
                // 方向は右か下
                int tempRandomDirection = Random.Range(0, 4);
                switch(tempRandomDirection){
                    case 0:
                        room1Direction = RightDirection;
                        room2Direction = LeftDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                        curvePos = Random.Range(MinCurvePos, xRoomDistance.Distance - MinCurvePos);
                        if(xRoomDistance.Distance >= 3){
                            // 3以上の場合は曲がり、そうでない場合はcase 1のパターンにする
                            break;
                        }
                        goto case 1;
                    case 1:
                        room1Direction = RightDirection;
                        room2Direction = UpDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                        curvePos = CommonConst.MinPos;
                        break;
                    case 2:
                        room1Direction = DownDirection;
                        room2Direction = UpDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                        curvePos = Random.Range(MinCurvePos, yRoomDistance.Distance - MinCurvePos);
                        if(yRoomDistance.Distance >= 3){
                            // 3以上の場合は曲がり、そうでない場合はcase 3のパターンにする
                            break;
                        }
                        goto case 3;
                    case 3:
                        room1Direction = DownDirection;
                        room2Direction = LeftDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                        curvePos = CommonConst.MinPos;
                        break;
                    default:
                        throw new System.Exception("方向が不正です");
                }
            }else{
                // y方向でもう一つの部屋が上にある場合
                int tempRandomDirection = Random.Range(0, 4);
                switch(tempRandomDirection){
                    case 0:
                        room1Direction = RightDirection;
                        room2Direction = LeftDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                        curvePos = Random.Range(MinCurvePos, xRoomDistance.Distance - MinCurvePos);
                        if(xRoomDistance.Distance >= 3){
                            // 3以上の場合は曲がり、そうでない場合はcase 1のパターンにする
                            break;
                        }
                        goto case 1;
                    case 1:
                        room1Direction = RightDirection;
                        room2Direction = DownDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                        curvePos = CommonConst.MinPos;
                        break;
                    case 2:
                        room1Direction = UpDirection;
                        room2Direction = DownDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                        curvePos = Random.Range(MinCurvePos, yRoomDistance.Distance - MinCurvePos);
                        if(yRoomDistance.Distance >= 3){
                            // 3以上の場合は曲がり、そうでない場合はcase 3のパターンにする
                            break;
                        }
                        goto case 3;
                    case 3:
                        room1Direction = UpDirection;
                        room2Direction = LeftDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                        curvePos = CommonConst.MinPos;
                        break;
                    default:
                        throw new System.Exception("方向が不正です");
                }
            }
        }else{
            // x方向でもう一つの部屋が左にある場合
            if(yRoomDirection == 0){
                // y方向が重なっている場合
                room1Direction = LeftDirection;
                room2Direction = RightDirection;
                room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                curvePos = Random.Range(MinCurvePos, xRoomDistance.Distance - MinCurvePos);
            }else if(yRoomDirection == 1){
                // y方向でもう一つの部屋が下にある場合
                int tempRandomDirection = Random.Range(0, 4);
                switch(tempRandomDirection){
                    case 0:
                        room1Direction = LeftDirection;
                        room2Direction = RightDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                        curvePos = Random.Range(MinCurvePos, xRoomDistance.Distance - MinCurvePos);
                        if(xRoomDistance.Distance >= 3){
                            // 3以上の場合は曲がり、そうでない場合はcase 1のパターンにする
                            break;
                        }
                        goto case 1;
                    case 1:
                        room1Direction = LeftDirection;
                        room2Direction = UpDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                        curvePos = CommonConst.MinPos;
                        break;
                    case 2:
                        room1Direction = DownDirection;
                        room2Direction = UpDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                        curvePos = Random.Range(MinCurvePos, yRoomDistance.Distance - MinCurvePos);
                        if(yRoomDistance.Distance >= 3){
                            // 3以上の場合は曲がり、そうでない場合はcase 3のパターンにする
                            break;
                        }
                        goto case 3;
                    case 3:
                        room1Direction = DownDirection;
                        room2Direction = RightDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                        curvePos = CommonConst.MinPos;
                        break;
                    default:
                        throw new System.Exception("方向が不正です");
                }
            }else{
                // y方向でもう一つの部屋が上にある場合
                int tempRandomDirection = Random.Range(0, 4);
                switch(tempRandomDirection){
                    case 0:
                        room1Direction = LeftDirection;
                        room2Direction = RightDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                        curvePos = Random.Range(MinCurvePos, xRoomDistance.Distance - MinCurvePos);
                        if(xRoomDistance.Distance >= 3){
                            // 3以上の場合は曲がり、そうでない場合はcase 1のパターンにする
                            break;
                        }
                        goto case 1;
                    case 1:
                        room1Direction = LeftDirection;
                        room2Direction = DownDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.y / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                        curvePos = CommonConst.MinPos;
                        break;
                    case 2:
                        room1Direction = UpDirection;
                        room2Direction = DownDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.x / 2) * 2;
                        curvePos = Random.Range(MinCurvePos, yRoomDistance.Distance - MinCurvePos);
                        if(yRoomDistance.Distance >= 3){
                            // 3以上の場合は曲がり、そうでない場合はcase 3のパターンにする
                            break;
                        }
                        goto case 3;
                    case 3:
                        room1Direction = UpDirection;
                        room2Direction = RightDirection;
                        room1PathStartPos = Random.Range(CommonConst.MinPos, rooms[room1Index].Size.x / 2) * 2;
                        room2PathStartPos = Random.Range(CommonConst.MinPos, rooms[room2Index].Size.y / 2) * 2;
                        curvePos = CommonConst.MinPos;
                        break;
                    default:
                        throw new System.Exception("方向が不正です");
                }
            }
        }
        return new Path(rooms[room1Index], room1Index, room1Direction, room1PathStartPos, rooms[room2Index], room2Index, room2Direction, room2PathStartPos, curvePos);
    }

    // 部屋同士のパスを生成する関数
    private (List<Path>, List<List<RoomDistanceWithIndex>>) CreatePath(IReadOnlyList<Room> rooms){
        //rooms : 部屋のリスト

        // 部屋同士の距離を計算
        List<List<RoomDistance>> xRoomDistances = new List<List<RoomDistance>>();
        List<List<RoomDistance>> yRoomDistances = new List<List<RoomDistance>>();
        List<List<RoomDistanceWithIndex>> roomDistancesWithIndex = new List<List<RoomDistanceWithIndex>>();
        foreach(Room room in rooms){
            List<RoomDistance> xRoomDistance = new List<RoomDistance>();
            List<RoomDistance> yRoomDistance = new List<RoomDistance>();
            List<RoomDistanceWithIndex> roomDistanceWithIndex = new List<RoomDistanceWithIndex>();
            int roomIndex = CommonConst.MinIndex;
            foreach(Room otherRoom in rooms){
                if(room == otherRoom){
                    xRoomDistance.Add(new RoomDistance(0, 0));
                    yRoomDistance.Add(new RoomDistance(0, 0));
                    roomDistanceWithIndex.Add(new RoomDistanceWithIndex(roomIndex, 0));
                    roomIndex++;
                    continue;
                }
                RoomDistance xDistance = CalculateXRoomDistance(room, otherRoom);
                RoomDistance yDistance = CalculateYRoomDistance(room, otherRoom);
                xRoomDistance.Add(xDistance);
                yRoomDistance.Add(yDistance);
                roomDistanceWithIndex.Add(new RoomDistanceWithIndex(roomIndex, xDistance.Distance + yDistance.Distance));
                roomIndex++;
            }
            xRoomDistances.Add(xRoomDistance);
            yRoomDistances.Add(yRoomDistance);
            roomDistancesWithIndex.Add(roomDistanceWithIndex);
        }

        // 1番目に近い部屋と2番目に近い部屋は必ずパスができる。

        // 1番目に近い部屋とのパスを作成
        List<Path> paths = new List<Path>();
        // roomDistancesWithIndexの各行を距離の昇順にソート
        // 部屋の数はせいぜいMaxRoomなのでバブルソートで十分
        for(int line = CommonConst.MinIndex; line < roomDistancesWithIndex.Count; ++line){
            for(int i = CommonConst.MinIndex; i < roomDistancesWithIndex[line].Count; ++i){
                for(int j = roomDistancesWithIndex[line].Count - 1; j > i; --j){
                    if(roomDistancesWithIndex[line][j - 1].Distance > roomDistancesWithIndex[line][j].Distance){
                        RoomDistanceWithIndex temp = roomDistancesWithIndex[line][j - 1];
                        roomDistancesWithIndex[line][j - 1] = roomDistancesWithIndex[line][j];
                        roomDistancesWithIndex[line][j] = temp;
                    }
                }
            }
        }
        // それぞれの部屋と1番目に近い部屋(自身は除く)とのパスを作成
        for(int j = CommonConst.MinIndex; j < 2; ++j){
            for(int i = CommonConst.MinIndex; i < rooms.Count; ++i){
                // その組の部屋のパスがすでに存在しているかどうか求める
                Room room1, room2;
                int room2Index = roomDistancesWithIndex[i][j + 1].Index;
                room1 = rooms[i];
                room2 = rooms[room2Index];
                Path tempPath = new Path(room1, i, 0, 0, room2, room2Index, 0, 0, 0);
                bool isExist = false;
                foreach(Path existPath in paths){
                    if(tempPath == existPath){
                        isExist = true;
                        break;
                    }
                }
                if(isExist){
                    // すでにパスが存在している場合次に進む
                    continue;
                }

                // パスを作成
                Path path = MakePath(rooms, i, room2Index);
                paths.Add(path);
            }
        }
        return (paths, roomDistancesWithIndex);
    }
}
