using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that will handle the basic game UI
/// </summary>
public class UIController : MonoBehaviour
{
    #region SINGLETON DESIGN PATTERN
    public static UIController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        DontDestroyOnLoad(this);
    }
    #endregion

    [Header("Game Timer Elements")]
    [SerializeField] private GameObject _gameTimer;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _sunDialArrow;
    [SerializeField] private float _arrowStartAngle;
    [SerializeField] private float _arrowEndAngle;

    [Header("End Menu Elements")]
    [SerializeField] private GameObject _endMenu;
    [SerializeField] private TextMeshProUGUI _endScoreText;

    [Header("Start Menu Elements")]
    [SerializeField] private GameObject _startMenu;

    private void Start()
    {
        _endMenu.SetActive(false);
        _gameTimer.SetActive(false);
        _startMenu.SetActive(true);
    }

    public void UpdateGameTimer(float hourValue, float minuteValue)
    {
        string textHour = hourValue.ToString();
        string textMinute = minuteValue.ToString();
        if (hourValue < 10)
        {
            textHour = "0" + textHour;
        }
        if (minuteValue < 10)
        {
            textMinute = "0" + textMinute;
        }
        _timerText.text = textHour + "H" + textMinute;
    }

    public void UpdateArrow(float elapsedTime, float totalTime)
    {
        float angle = _arrowStartAngle + elapsedTime / totalTime * (_arrowEndAngle - _arrowStartAngle);
        _sunDialArrow.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void EnableEndMenu()
    {
        _endMenu.SetActive(true);
    }

    /// <summary>
    /// Function that will update the score on the end menu.
    /// </summary>
    /// <param name="scoreValue">Score of the player</param>
    /// <param name="numSatisfiedClients">Number of clients that reached their "happiness" time</param>
    /// <param name="numNotAmusedClients">Number of clients that were "not amused" (wrong item used)</param>
    /// <param name="numDeadClients">Number of Dead clients (self-explanatory)</param>
    public void UpdateEndScore(int scoreValue, int numSatisfiedClients, int numNotAmusedClients, int numDeadClients)
    {
        _endScoreText.text = "Score : "+scoreValue.ToString() + '\n' + '\n' + "Number of Satisfied Clients : "+numSatisfiedClients.ToString() + '\n' + '\n' + "Not Amused Clients : "+numNotAmusedClients.ToString() + '\n' + '\n' + "Number of Dead Clients : "+numDeadClients;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Debug.Log("restarting game");
        _endMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame(int buildIndex)
    {
        //SceneManager.LoadScene(buildIndex);
        _startMenu.SetActive(false);
        _gameTimer.SetActive(true);
    }
}
