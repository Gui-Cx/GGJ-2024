using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        _instance = this;
    }
    #endregion

    [Header("Game Timer Elements")]
    [SerializeField] private int _maxHourTimer = 18;
    [SerializeField] private int _curGameTimer=00;
    [SerializeField] private float _timerRefreshRate = 2f;

    [Header("End Menu Elements")]
    [SerializeField] private int _curScore = 0;
    [SerializeField] private int _numSatisfiedClients = 0;
    [SerializeField] private int _numDeadClients = 0;
    [SerializeField] private int _numNotAmusedClients = 0;
    [SerializeField] private int _numSadClients = 0;
    [SerializeField] private int _numTotalClients = 0;

    [Header("Tilemap elements")]
    [SerializeField] public Grid TilemapGrid;
    [SerializeField] public Tilemap BackgroundTilemap;
    [SerializeField] public Tilemap GreyTilemap;

    [SerializeField] public ItemsData itemsData;

    private int _curHour = 8;

    private void Start()
    {
        SetupTimer();
        UIController.Instance.UpdateGameTimer(_curHour, _curGameTimer);
    }

    #region TIMER FUNCTIONS
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
        AudioManager.Instance.CleanUp();
        AudioManager.Instance.PlayGameOverMusic();
        UIController.Instance.EnableEndMenu();
        DisplayFinalScore();
    }
    #endregion

    #region SCORE FUNCTIONS
    public void IncreaseScore(int value)
    {
        _curScore += value;
    }
    public void DecreaseScore(int value)
    {
        _curScore -= value;
    }
    /// <summary>
    /// Updates the number of clients that reach their happiness time.
    /// The same client can be counted multiple times
    /// </summary>
    public void UpdateNumberOfSatisfiedClients()
    {
        _numSatisfiedClients++;
    }
    public void UpdateNumberOfDeadClients()
    {
        _numDeadClients++;
        _numTotalClients--;
    }
    public void UpdateNumberOfNotAmusedClients()
    {
        _numNotAmusedClients++;
    }
    public void UpdateNumberTotalOfClients()
    {
        _numTotalClients++;
        SetMusicLevel();
    }

    public void UpdateSadNumber(bool isSad)
    {
        _ = isSad? _numSadClients++ : _numSadClients--;
        SetMusicLevel();
    }

    private void DisplayFinalScore()
    {
        UIController.Instance.UpdateEndScore(_curScore, _numSatisfiedClients, _numNotAmusedClients, _numDeadClients);
    }

    public void SetMusicLevel()
    {
        int level = (int) (4 * _numSadClients / _numTotalClients);
        AudioManager.Instance.SetMusicVersion(4-level);
    }
    #endregion
}
