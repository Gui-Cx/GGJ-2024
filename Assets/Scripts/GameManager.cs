using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script of the GameManager object that will handle the game's objective and overall architecture
/// </summary>
public class GameManager : MonoBehaviour
{

    #region SINGLETON DESIGN PATTERN
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;
    }
    #endregion

    [Header("Game Timer Elements")]
    [SerializeField] private int _maxHourTimer = 18;
    [SerializeField,InspectorName("DO NOT EDIT")] private int _curGameTimer=00;
    [SerializeField] private float _timerRefreshRate = 2f;
    private int _curHour = 8;

    private void Start()
    {
        SetupTimer();
        UIController.Instance.UpdateGameTimer(_curHour, _curGameTimer);
    }

    /// <summary>
    /// Function that will setup the timer and start the counter
    /// </summary>
    private void SetupTimer()
    {
        _curGameTimer = 0;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        yield return new WaitForSeconds(_timerRefreshRate);
        _curGameTimer+=5;
        if (_curGameTimer == 60)
        {
            _curHour++;
            _curGameTimer=0;
        }
        UIController.Instance.UpdateGameTimer(_curHour,_curGameTimer);
        if (_curHour >= _maxHourTimer)
        {
            TriggerEndOfGame();
        }
        else
        {
            StartCoroutine(UpdateTimer());
        }
    }

    /// <summary>
    /// Function called internally that will trigger the end of the game and all its associated methods.
    /// </summary>
    private void TriggerEndOfGame()
    {
        Debug.Log("GAME IS OVER");
    }

}
