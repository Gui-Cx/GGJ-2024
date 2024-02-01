using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Color = UnityEngine.Color;

/// <summary>
/// This script will handle the ever going down bar
/// </summary>
public class NPCHappinessBarController : MonoBehaviour
{
    [SerializeField, InspectorName("Happiness Lvl (debug)")] private int _curLevel; //Serialized for debug purposes

    [Header("Elements")]
    [SerializeField] private GameObject _happinessBar;
    [SerializeField] private GameObject _laughSprite;
    [SerializeField] private GameObject _happinessAOE;
    [SerializeField] private GameObject _sadnessAOE;

    private AOESadness _aoeSadness;
    private HappinessBarModule _barModule;
    private Animator _animator;

    private int _maxHappiness;
    private int _happinessThreshold;
    private float _HappinessUpdateDelay;
    private float _happyTime;

    private int _scoreIncrease;
    private int _scoreDecrease;

    private bool _happinessIsActive = false;
    public bool HappinessIsActive => _happinessIsActive;

    private void OnValidate()
    {
        Assert.IsNotNull(_happinessAOE);
        Assert.IsNotNull(_happinessBar);
    }

    private void Awake()
    {
        _barModule = _happinessBar.GetComponent<HappinessBarModule>();
        _aoeSadness = GetComponentInChildren<AOESadness>();

        _laughSprite.SetActive(false);
    }

    public void SetValues(NPCSettingsData settingsData, Animator animator)
    {
        _animator = animator;
        _maxHappiness = settingsData.MaxHappiness;
        _happinessThreshold = settingsData.HappinessThreshold;
        _HappinessUpdateDelay = settingsData.HappinessUpdateDelay;
        _happyTime = settingsData.HappyTime;
        _scoreIncrease = settingsData.ScoreIncrease;
        _scoreDecrease = settingsData.ScoreDecrease;

        _aoeSadness.SetValues(settingsData.MinSadnessRadius, settingsData.MaxSadnessRadius, 
                              settingsData.MinEmissionRate, settingsData.MaxEmissionRate);

        _barModule.SetMaxHappiness(_maxHappiness);
        _barModule.ChangeFillColor(Color.green);

        StartCoroutine(UpdateHappiness());
    }

    /// <summary>
    /// Function that will reduce the bar every bar down update timer
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateHappiness()
    {
        yield return new WaitForSeconds(_HappinessUpdateDelay);
        _barModule.SetHappinessValue(_curLevel);

        if (!_happinessIsActive) _curLevel--;
        if (_curLevel < 0) _curLevel = 0;

        if (_curLevel < _happinessThreshold)
        {
            GetComponent<NPCBehaviourController>().SwitchState(NPC_STATE.Sad);
            GameManager.Instance.DecreaseScore(_scoreDecrease);

            _aoeSadness.SetSadness(_curLevel, _happinessThreshold);
            _barModule.ChangeFillColor(Color.red);
        }

        StartCoroutine(UpdateHappiness());
    }

    /// <summary>
    /// Function called when the the correct item has been applied to the NPC, 
    /// triggering its "happy" phase during which:
    /// <br>- happiness bar is maxed out</br>
    /// <br>- happiness bar no longer goes down for a HappinessTime duration</br>
    /// </summary>
    public void ActivateHappinessTime()
    {
        _happinessIsActive = true;
        _curLevel = _maxHappiness;        
        _animator.SetTrigger("Laugh");
        _laughSprite.SetActive(true);
        _aoeSadness.RemoveSadness();

        GameManager.Instance.IncreaseScore(_scoreIncrease);

        _barModule.ChangeFillColor(Color.yellow);
        _barModule.SetHappinessValue(_curLevel);

        StartCoroutine(HappinessTimer());
    }

    private IEnumerator HappinessTimer()
    {
        _happinessAOE.SetActive(true);

        yield return new WaitForSeconds(_happyTime);
        
        _happinessIsActive = false;
        _laughSprite.SetActive(false);
        _happinessAOE.SetActive(false);

        _barModule.ChangeFillColor(Color.green);

        GetComponent<NPCBehaviourController>().SwitchState(NPC_STATE.Idle);
    }
}
