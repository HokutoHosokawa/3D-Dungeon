using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-9)]
public class CreateRoom3DView : MonoBehaviour
{
    const float WallThickness = 0.2f;
    const float WallHeight = 1.0f;
    const float WallWidth = 1.0f;
    const float FloorWidth = 1.0f;
    const float FloorHeight = 1.0f;
    const float FloorThickness = 0.2f;
    [SerializeField] GameObject _floorPrefab = default;
    [SerializeField] GameObject _wallPrefab = default;
    CreateDungeon createDungeon = null;

    void Awake() {
        createDungeon = GetComponent<CreateDungeon>();
    }

    // Start is called before the first frame update
    void Start()
    {
        int[,] map = createDungeon.Map;
        for(int y = 0; y < CommonConst.MapHeight; ++y){
            for(int x = 0; x < CommonConst.MapWidth; ++x){
                if(map[y, x] == 0){
                    continue;
                }
                // 床を生成
                GameObject floor = Instantiate(_floorPrefab, new Vector3(x, 0, y), Quaternion.identity);
                GameObject floor2 = Instantiate(_floorPrefab, new Vector3(x, WallHeight * 2 + FloorThickness / 2.0f, y), Quaternion.identity);
                floor.transform.SetParent(transform);
                // 壁を生成
                Quaternion[] wallRotations = {
                    Quaternion.AngleAxis(0, Vector3.up), // z軸に水平(x軸に直交)
                    Quaternion.AngleAxis(90, Vector3.up) // z軸に垂直(x軸に直交)
                };
                if(y == 0 || map[y - 1, x] == 0){
                    GameObject wall = Instantiate(_wallPrefab, new Vector3(x, WallHeight/2.0f, y - (WallHeight + WallThickness)/2.0f), wallRotations[1], transform);
                    GameObject wall2 = Instantiate(_wallPrefab, new Vector3(x, 3 * WallHeight/2.0f, y - (WallHeight + WallThickness)/2.0f), wallRotations[1], transform);
                }
                if(y == CommonConst.MapHeight - 1 || map[y + 1, x] == 0){
                    GameObject wall = Instantiate(_wallPrefab, new Vector3(x, WallHeight/2.0f, y + (WallHeight + WallThickness)/2.0f), wallRotations[1], transform);
                    GameObject wall2 = Instantiate(_wallPrefab, new Vector3(x, 3 * WallHeight/2.0f, y + (WallHeight + WallThickness)/2.0f), wallRotations[1], transform);
                }
                if(x == 0 || map[y, x - 1] == 0){
                    GameObject wall = Instantiate(_wallPrefab, new Vector3(x - (WallHeight + WallThickness)/2.0f, WallHeight/2.0f, y), wallRotations[0], transform);
                    GameObject wall2 = Instantiate(_wallPrefab, new Vector3(x - (WallHeight + WallThickness)/2.0f, 3 * WallHeight/2.0f, y), wallRotations[0], transform);
                }
                if(x == CommonConst.MapWidth - 1 || map[y, x + 1] == 0){
                    GameObject wall = Instantiate(_wallPrefab, new Vector3(x + (WallHeight + WallThickness)/2.0f, WallHeight/2.0f, y), wallRotations[0], transform);
                    GameObject wall2 = Instantiate(_wallPrefab, new Vector3(x + (WallHeight + WallThickness)/2.0f, 3 * WallHeight/2.0f, y), wallRotations[0], transform);
                }
            }
        }
    }
}
