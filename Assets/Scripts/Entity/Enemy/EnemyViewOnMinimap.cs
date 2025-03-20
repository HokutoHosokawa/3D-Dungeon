using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewOnMinimap : MonoBehaviour
{
    private readonly float InvisibleDistanceOnPath = 2.0f;
    private GameObject _enemyMarker;
    private GameObject _player;
    private CommonPlayerVariable _playerVariable;
    private CapsuleCollider _enemyCollider;
    bool isVisible = false;
    // Start is called before the first frame update
    void Start()
    {
        _enemyMarker = transform.Find("EnemyMarker").gameObject;
        if(_enemyMarker == null)
        {
            Debug.LogError("EnemyMarker is not found");
        }
        _enemyMarker.SetActive(false);
        _enemyCollider = GetComponent<CapsuleCollider>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerVariable = _player.GetComponent<CommonPlayerVariable>();
    }

    void Update()
    {
        EnemyIsOnVisible();
        EnemyIsOnInvisible();
    }

    private void EnemyIsOnVisible()
    {
        // 視覚に写っていないとき
        if(!isVisible)
        {
            // そもそも視線にも入っていないときは何もしない
            return;
        }
        // 視線には入っている
        Vector3 enemyPosition = transform.position;
        Vector3 enemyCenter = enemyPosition + _enemyCollider.center;
        Vector3 direction = enemyCenter - Camera.main.transform.position;
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("InvisibleLayer");
        if(Physics.Raycast(Camera.main.transform.position, direction, out hit, direction.magnitude + 1.0f, layerMask))
        {
            if(hit.collider.gameObject != gameObject && hit.collider.gameObject != _player)
            {
                _enemyMarker.SetActive(false);
                return;
            }
            Vector3 cameraToHitTarget = hit.point - Camera.main.transform.position;
            if(!_playerVariable.isPlayerInRoom && cameraToHitTarget.magnitude > InvisibleDistanceOnPath)
            {
                _enemyMarker.SetActive(false);
                return;
            }
        }
        else
        {
            Debug.Log("Invisible Because the ray does not hit anything");
            _enemyMarker.SetActive(false);
            return;
        }
        _enemyMarker.SetActive(true);
    }

    private void EnemyIsOnInvisible()
    {
        if(isVisible)
        {
            return;
        }
        _enemyMarker.SetActive(false);
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }
}
