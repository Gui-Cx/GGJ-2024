using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This script will handle the ever going down bar
/// </summary>
public class NPCHappinessBarController : MonoBehaviour
{
    [Header("Happiness Parameters")]
    [SerializeField] private int _maxLevel = 20;
    [SerializeField] private int _happinessThreshold = 10; //at this point, the patient will start to emit sadness
    [SerializeField] private int _curLevel; //Serialized for debug purposes

    [Header("Timer Parameters")]
    [SerializeField] private float _barDownUpdateTimer=1f;
    [SerializeField] private float _happinessTimer = 5f; //the timer during which the NPC will be "happy" and its happiness bar will no longer go down

    [Header("Score Parameters")]
    [SerializeField] private int _scoreIncrease = 20;
    [SerializeField] private int _scoreDecrease = 1;

    [Header("Elements")]
    [SerializeField] private GameObject _happinessAOE;
    [SerializeField] private GameObject _laughSprite;
    [SerializeField] private GameObject _happinessBar;
    [SerializeField] private Animator _anim;

    private AOESadness _aoeSadness;
    private HappinessBarModule _barModule;

    private bool _happinessIsActive = false;
    public bool HappinessIsActive => _happinessIsActive;

    private void OnValidate()
    {
        Assert.IsNotNull(_happinessAOE);
        Assert.IsNotNull(_happinessBar);
    }

    private void Start()
    {
        _barModule = _happinessBar.GetComponent<HappinessBarModule>();
        _curLevel = _maxLevel;

        _barModule.SetMaxHappiness(_maxLevel);
        _laughSprite.SetActive(false);

        UpdateHappinessBarColor(Color.green);
        StartCoroutine(BarDownUpdate());

        _aoeSadness = GetComponentInChildren<AOESadness>();
        _aoeSadness.SetGridInfo(GameManager.Instance.TilemapGrid,GameManager.Instance.BackgroundTilemap,GameManager.Instance.GreyTilemap);
    }

    /// <summary>
    /// Function that will reduce the bar every bar down update timer
    /// </summary>
    /// <returns></returns>
    private IEnumerator BarDownUpdate()
    {
        yield return new WaitForSeconds(_barDownUpdateTimer);
        _aoeSadness.SetSadness(_curLevel, _happinessThreshold);
        if (!_happinessIsActive)
        {
            _curLevel--;
        }
        if (_curLevel < _happinessThreshold)
        {
            //Debug.Log("NPC : " + gameObject.name + " IS SAD");
            //TODO : Do something here to "emit sadness"
            GetComponent<NPCBehaviourController>().SwitchState(NPC_STATE.Sad);
            GameManager.Instance.DecreaseScore(_scoreDecrease); //we decrease (each seconds) the score by the score decrease 
            UpdateHappinessBarColor(Color.red);
        }
        if( _curLevel <= 0)
        {
            _curLevel = 0;
        }
        //Debug.Log("NPC : " + gameObject.name + " : Happiness Bar : " + _curLevel);
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
        _happinessIsActive = true;
        _curLevel = _maxLevel;        
        _anim.SetTrigger("Laugh");
        _laughSprite.SetActive(true);

        GameManager.Instance.IncreaseScore(_scoreIncrease);
        UpdateHappinessBarColor(Color.yellow);
        UpdateVisualHappinessBar();

        StartCoroutine(HappinessTimer());
    }

    private IEnumerator HappinessTimer()
    {
        GameObject aoe = Instantiate(_happinessAOE, transform);
        aoe.transform.position = gameObject.transform.position;

        yield return new WaitForSeconds(_happinessTimer);
        
        _happinessIsActive = false;
        _laughSprite.SetActive(false);
        Destroy(aoe);
        UpdateHappinessBarColor(Color.green);

        GetComponent<NPCBehaviourController>().SwitchState(NPC_STATE.Idle);
    }
}
