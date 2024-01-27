using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This script will handle the ever going down bar
/// </summary>
public class NPCHappinessBarController : MonoBehaviour
{
    [Header("Happiness Bar values")]
    [SerializeField] private int _maxLevel = 20;
    [SerializeField] private int _curLevel;
    [SerializeField] private int _happinessThreshold = 10; //at this point, the patient will start to emit sadness

    [Header("Timer values")]
    [SerializeField] private float _barDownUpdateTimer=1f;
    [SerializeField] private float _happinessTimer = 5f; //the timer during which the NPC will be "happy" and its happiness bar will no longer go down

    [Header("Happiness Bar elements")]
    [SerializeField] private GameObject _happinessBar;
    private HappinessBarModule _barModule;

    private bool _happinessIsActive = false;

    private void OnValidate()
    {
        Assert.IsNotNull(_happinessBar);
    }

    private void Start()
    {
        _barModule = _happinessBar.GetComponent<HappinessBarModule>();
        _curLevel = _maxLevel;
        _barModule.SetMaxHappiness(_maxLevel);
        UpdateHappinessBarColor(Color.green);
        StartCoroutine(BarDownUpdate());
    }

    /// <summary>
    /// Function that will reduce the bar every bar down update timer
    /// </summary>
    /// <returns></returns>
    private IEnumerator BarDownUpdate()
    {
        yield return new WaitForSeconds(1f);
        if (!_happinessIsActive)
        {
            _curLevel--;
        }
        if (_curLevel < _happinessThreshold)
        {
            Debug.Log("NPC : " + gameObject.name + " IS SAD");
            //TODO : Do something here to "emit sadness"
            UpdateHappinessBarColor(Color.red);
        }
        if( _curLevel <= 0)
        {
            _curLevel = 0;
        }
        Debug.Log("NPC : " + gameObject.name + " : Happiness Bar : " + _curLevel);
        UpdateVisualHappinessBar();
        StartCoroutine(BarDownUpdate());
    }

    private void UpdateVisualHappinessBar()
    {
        _barModule.SetHappinessValue(_curLevel);
    }

    private void UpdateHappinessBarColor(Color color)
    {
        _barModule.ChangeFillColor(color);
    }

    /// <summary>
    /// Function called when the the correct item has been applied to the NPC, triggering its "happy" phase during which:
    /// - happiness bar is maxed out
    /// - happiness bar no longer goes down for a HappinessTime duration
    /// </summary>
    public void ActivateHappinessTime()
    {
        Debug.Log("NPC IS HAPPY ! HOORAY !");
        _happinessIsActive = true;
        _curLevel = _maxLevel;
        UpdateHappinessBarColor(Color.yellow);
        UpdateVisualHappinessBar();
        StartCoroutine(HappinessTimer());
    }

    private IEnumerator HappinessTimer()
    {
        yield return new WaitForSeconds(_happinessTimer);
        _happinessIsActive = false;
        UpdateHappinessBarColor(Color.green);
        GetComponent<NPCBehaviourController>().SwitchState(NPC_STATE.Idle);
    }
}
