using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerViewController : MonoBehaviour
{
    private Collider playerCollider;
    private Dictionary<Room, GameObject> roomColliderObjects = new Dictionary<Room, GameObject>();
    private Dictionary<Room, List<GameObject>> roomPatitionObjects = new Dictionary<Room, List<GameObject>>();
    private List<Room> roomList = new List<Room>();
    // private Material inRoomMaterial;
    // private Material outRoomMaterial;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<Collider>();
        InitRoomObjectState();
        // inRoomMaterial = Resources.Load<Material>("Materials/BlackMaterial");
        // outRoomMaterial = Resources.Load<Material>("Materials/InvisibleMaterial");
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.5f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            // 部屋に入った場合
            RenderSettings.fog = false;
            List<GameObject> patitions = roomPatitionObjects[SearchRoom(other)];
            foreach(GameObject patition in patitions)
            {
                patition.SetActive(true);
            }
            // foreach(Transform child in other.transform)
            // {
            //     GameObject patition = child.gameObject;
            //     MeshRenderer meshRenderer = patition.GetComponent<MeshRenderer>();
            //     meshRenderer.material = inRoomMaterial;
            //     if(child.rotation == Quaternion.Euler(0,0,0))
            //     {
            //         // 下の壁
            //         meshRenderer.material.SetVector("_RoomNormal", new Vector3(0, 0, 1));
            //         meshRenderer.material.SetVector("_ExitPosition", new Vector3(child.position.x, child.position.y, child.position.z + 1.5f * CommonConst.FloorHeight));
            //     }else if(child.rotation == Quaternion.Euler(0, 90, 0))
            //     {
            //         // 右の壁
            //         meshRenderer.material.SetVector("_RoomNormal", new Vector3(-1, 0, 0));
            //         meshRenderer.material.SetVector("_ExitPosition", new Vector3(child.position.x - 1.5f * CommonConst.FloorWidth, child.position.y, child.position.z));
            //     }else if(child.rotation == Quaternion.Euler(0, 180, 0))
            //     {
            //         // 上の壁
            //         meshRenderer.material.SetVector("_RoomNormal", new Vector3(0, 0, -1));
            //         meshRenderer.material.SetVector("_ExitPosition", new Vector3(child.position.x, child.position.y, child.position.z - 1.5f * CommonConst.FloorHeight));
            //     }else if(child.rotation == Quaternion.Euler(0, 270, 0))
            //     {
            //         // 左の壁
            //         meshRenderer.material.SetVector("_RoomNormal", new Vector3(1, 0, 0));
            //         meshRenderer.material.SetVector("_ExitPosition", new Vector3(child.position.x + 1.5f * CommonConst.FloorWidth, child.position.y, child.position.z));
            //     }
            // }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Room"))
        {
            // 部屋から出た場合
            RenderSettings.fog = true;
            List<GameObject> patitions = roomPatitionObjects[SearchRoom(other)];
            foreach(GameObject patition in patitions)
            {
                patition.SetActive(false);
            }
            // foreach(Transform child in other.transform)
            // {
            //     GameObject patition = child.gameObject;
            //     MeshRenderer meshRenderer = patition.GetComponent<MeshRenderer>();
            //     meshRenderer.material = outRoomMaterial;
            // }
        }
    }

    private Room SearchRoom(Collider roomCollider)
    {
        if(roomCollider.GetType() != typeof(BoxCollider))
        {
            throw new System.ArgumentException("roomCollider must be BoxCollider.");
        }
        BoxCollider boxCollider = (BoxCollider)roomCollider;
        foreach(Room room in roomList)
        {
            float roomColliderCenterX = boxCollider.transform.position.x / CommonConst.FloorWidth;
            float roomColliderCenterY = boxCollider.transform.position.z / CommonConst.FloorHeight;
            int roomColliderSizeX = (int)(boxCollider.size.x / CommonConst.FloorWidth);
            int roomColliderSizeY = (int)(boxCollider.size.z / CommonConst.FloorHeight);
            int roomUpperLeftX = (int)(roomColliderCenterX - (roomColliderSizeX - 1) / 2.0f + 0.1f);
            int roomUpperLeftY = (int)(roomColliderCenterY - (roomColliderSizeY - 1) / 2.0f + 0.1f);
            if(room.UpperLeftPosition.x == roomUpperLeftX && room.UpperLeftPosition.y == roomUpperLeftY && room.Size.x == roomColliderSizeX && room.Size.y == roomColliderSizeY)
            {
                return room;
            }
        }
        throw new System.ArgumentException("roomCollider is not in roomList.");
    }

    public void InitRoomObjectState()
    {
        roomColliderObjects = GameObject.Find("CreateDungeonObject").GetComponent<CreateFloor>().GetMapObjects().RoomColliderObjectList;
        //roomList = roomColliderObjects.Keys.ToList();
        roomList = new List<Room>(roomColliderObjects.Keys);
        foreach(var roomColliderObject in roomColliderObjects)
        {
            foreach(Transform child in roomColliderObject.Value.transform)
            {
                GameObject patition = child.gameObject;
                if(!roomPatitionObjects.ContainsKey(roomColliderObject.Key))
                {
                    roomPatitionObjects[roomColliderObject.Key] = new List<GameObject>();
                }
                roomPatitionObjects[roomColliderObject.Key].Add(patition);
                patition.SetActive(false);
            }
        }
    }
}
