using System.Collections;
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

    [Header("Game Timer Parameters")]
    [SerializeField] private int _gameDuration = 600;
    [SerializeField] private int _timerIncrementValue = 5;
    [SerializeField] private float _timerRefreshRate = 2f;

    [Header("Tilemap elements")]
    public Grid TilemapGrid;
    public Tilemap BackgroundTilemap;
    public Tilemap GreyTilemap;

    [Header("Items")]
    public ItemsData ItemsData;

    private float _totalTimeElapsed = 0f;

    private int _curScore = 0;
    private int _numSatisfiedClients = 0;
    private int _numDeadClients = 0;
    private int _numNotAmusedClients = 0;
    private int _numSadClients = 0;
    private int _numTotalClients = 0;

    public void StartGame()
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
