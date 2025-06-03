using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HitPointBarController : MonoBehaviour
{
    [SerializeField] private Slider _hitPointBar;
    [SerializeField] private TextMeshProUGUI _hitPointText;
    private HitPoint _playerHitPoint;
    // Start is called before the first frame update
    void Start()
    {
        _playerHitPoint = new HitPoint(10);
    }

    public void UpdateHitPointBar(HitPoint hp)
    {
        _playerHitPoint = hp;
        _hitPointBar.maxValue = _playerHitPoint.MaxHitPoint;
        _hitPointBar.value = _playerHitPoint.CurrentHitPoint;
        _hitPointText.text = _playerHitPoint.CurrentHitPoint + " / " + _playerHitPoint.MaxHitPoint;
    }
}
