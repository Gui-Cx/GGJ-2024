using System.Collections;
using UnityEngine;
using Color = UnityEngine.Color;

/// <summary>
/// This script will handle the ever going down bar
/// </summary>
public class NPCHappinessBarController : MonoBehaviour
{
    [SerializeField] private int _happiness; //Serialized for debug purposes

    [Header("Elements")]
    [SerializeField] private GameObject _happinessBar;
    [SerializeField] private GameObject _laughSprite;
    [SerializeField] private GameObject _AOEHappiness;

    private AOESadness _AOESadness;
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

    private void Awake()
    {
        _barModule = _happinessBar.GetComponent<HappinessBarModule>();
        _AOESadness = GetComponentInChildren<AOESadness>();

        _laughSprite.SetActive(false);
        _AOEHappiness.SetActive(false);
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

        _AOESadness.SetValues(settingsData.MinSadnessRadius, settingsData.MaxSadnessRadius, 
                              settingsData.MinEmissionRate, settingsData.MaxEmissionRate);
        
        _barModule.SetMaxHappiness(_maxHappiness);
        _barModule.ChangeFillColor(Color.green);

        _happiness = _maxHappiness;
        StartCoroutine(UpdateHappiness());
    }

    /// <summary>
    /// Function that will reduce the bar every bar down update timer
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateHappiness()
    {
        yield return new WaitForSeconds(_HappinessUpdateDelay);
        _barModule.SetHappinessValue(_happiness);

        if (!_happinessIsActive) _happiness--;
        if (_happiness < 0) _happiness = 0;

        if (_happiness < _happinessThreshold)
        {
            GetComponent<NPCBehaviourController>().SwitchState(NPC_STATE.Sad);
            GameManager.Instance.DecreaseScore(_scoreDecrease);

            _AOESadness.SetSadness(_happiness, _happinessThreshold);
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
        _happiness = _maxHappiness;        
        _animator.SetTrigger("Laugh");
        _laughSprite.SetActive(true);
        _AOESadness.RemoveSadness();

        GameManager.Instance.IncreaseScore(_scoreIncrease);

        _barModule.ChangeFillColor(Color.yellow);
        _barModule.SetHappinessValue(_happiness);

        StartCoroutine(HappinessTimer());
    }

    private IEnumerator HappinessTimer()
    {
        _AOEHappiness.SetActive(true);

        yield return new WaitForSeconds(_happyTime);
        
        _happinessIsActive = false;
        _laughSprite.SetActive(false);
        _AOEHappiness.SetActive(false);

        _barModule.ChangeFillColor(Color.green);

        GetComponent<NPCBehaviourController>().SwitchState(NPC_STATE.Idle);
    }
}
