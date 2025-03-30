using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairEvent : EventTemplate
{
    public override void ActionEvent()
    {
        GameObject createDungeonObject = GameObject.Find("CreateDungeonObject");
        CreateFloor createFloor = createDungeonObject.GetComponent<CreateFloor>();
        int currentFloor = createFloor.CurrentFloor;
        createFloor.CreateNewFloor(currentFloor + 1);
    }
}
