using System.Collections;
using UnityEngine;

/// <summary>
/// Script of the GameManager object that will handle the game's objective and overall architecture
/// </summary>
public class GameManager : MonoBehaviour
{

    #region SINGLETON DESIGN PATTERN
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }
    #endregion

    [Header("Game Timer Parameters")]
    [SerializeField] private int _gameDuration = 600;
    [SerializeField] private float _timerRefreshRate = 1f;

    [Header("Items")]
    public ItemsData ItemsData;

    private float _totalTimeElapsed = 0f;

    private int _curScore = 0;
    private int _numSatisfiedClients = 0;
    private int _numDeadClients = 0;
    private int _numNotAmusedClients = 0;
    private int _numSadClients = 0;
    private int _numTotalClients = 0;

    public void Start()
    {
        StartCoroutine(UpdateTimer());
    }

    #region TIMER FUNCTIONS
    private IEnumerator UpdateTimer()
    {        
        UIController.Instance.UpdateArrow(_totalTimeElapsed, _gameDuration);
        _totalTimeElapsed += _timerRefreshRate;

        if (_totalTimeElapsed >= _gameDuration)
        {
            TriggerEndOfGame();
        }
        else
        {
            yield return new WaitForSeconds(_timerRefreshRate);
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
        UIController.Instance.UpdateScore(_curScore);
    }
    public void DecreaseScore(int value)
    {
        _curScore -= value;
        UIController.Instance.UpdateScore(_curScore);
    }
    /// <summary>
    /// Updates the number of clients that reach their happiness time.
    /// The same client can be counted multiple times
    /// </summary>
    public void UpdateSatisfiedClientsCount()
    {
        _numSatisfiedClients++;
    }
    public void UpdateDeadClientsCount()
    {
        _numDeadClients++;
        _numTotalClients--;
    }
    public void UpdateNotAmusedClientsCount()
    {
        _numNotAmusedClients++;
    }
    public void UpdateTotalClientsCount()
    {
        _numTotalClients++;
        SetMusicLevel();
    }

    public void UpdateSadCount(bool isSad)
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
