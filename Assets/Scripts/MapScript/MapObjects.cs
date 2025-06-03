using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjects
{

    private List<GameObject> _mapObjects;
    private Dictionary<Room, GameObject> _roomColliderObjects;
    private Dictionary<Vector2Int, GameObject> _minimapObjects;

    public MapObjects(List<GameObject> mapObjects, Dictionary<Room, GameObject> roomColliderObjects, Dictionary<Vector2Int, GameObject> minimapObjects)
    {
        if(mapObjects == null || roomColliderObjects == null){
            throw new System.ArgumentNullException("mapObjects or roomColliderObjects is null");
        }
        _mapObjects = mapObjects;
        _roomColliderObjects = roomColliderObjects;
        _minimapObjects = minimapObjects;
    }

    public void Destroy()
    {
        foreach(GameObject mapObject in _mapObjects){
            GameObject.Destroy(mapObject);
        }
        foreach(GameObject roomColliderObject in _roomColliderObjects.Values){
            GameObject.Destroy(roomColliderObject);
        }
        foreach(GameObject minimapObject in _minimapObjects.Values){
            GameObject.Destroy(minimapObject);
        }
    }

    public void ChangeMinimapObjectActive(int x, int y, bool active)
    {
        Vector2Int position = new Vector2Int(x, y);
        if(!_minimapObjects.ContainsKey(position)){
            throw new System.ArgumentException("Minimap object does not exist.");
        }
        _minimapObjects[position].SetActive(active);
    }

    public List<GameObject> MapObjectList => _mapObjects;
    public Dictionary<Room, GameObject> RoomColliderObjectList => _roomColliderObjects;
    public Dictionary<Vector2Int, GameObject> MinimapObjectList => _minimapObjects;
}
