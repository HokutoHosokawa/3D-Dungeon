using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private Collider swordCollider;
    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponent<Collider>();
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
