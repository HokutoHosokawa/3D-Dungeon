using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "SampleScene")
            CursorLock();
        else
            CursorUnlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "SampleScene")
        {
            CursorUnlock();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !Cursor.visible)
        {
            CursorUnlock();
        }
        if (Input.GetMouseButtonDown(0) && Cursor.visible)
        {
            CursorLock();
        }
    }

    private void CursorLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void CursorUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
