using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will handle the ever going down bar
/// </summary>
public class NPCHappinessBarController : MonoBehaviour
{
    [SerializeField] private int _maxLevel;
    [SerializeField] private int _curLevel;

    [SerializeField] private float _barDownUpdateTimer=1f;

    private void Start()
    {
        StartCoroutine(BarDownUpdate());
    }

    /// <summary>
    /// Function that will reduce the bar every bar down update timer
    /// </summary>
    /// <returns></returns>
    private IEnumerator BarDownUpdate()
    {
        yield return new WaitForSeconds(1f);
        _curLevel--;
        if( _curLevel <= 0)
        {
            _curLevel = 0;
        }
    }
}
