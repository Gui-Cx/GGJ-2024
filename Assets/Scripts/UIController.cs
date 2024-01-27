using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Script that will handle the basic game UI
/// </summary>
public class UIController : MonoBehaviour
{
    #region SINGLETON DESIGN PATTERN
    private static UIController _instance;
    public static UIController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIController();
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
    [SerializeField] private GameObject _gameTimer;
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("End Menu Elements")]
    [SerializeField] private GameObject _endMenu;
    [SerializeField] private TextMeshProUGUI _endScoreText;

    private void Start()
    {
        _endMenu.SetActive(false);
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
        _timerText.text = textHour + ":"+ textMinute;
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
        _endScoreText.text = "Score : "+scoreValue.ToString() + '\n'+"Number of Satisfied Clients : "+numSatisfiedClients.ToString()+'\n'+"Not Amused Clients : "+numNotAmusedClients.ToString()+'\n'+"Number of Dead Clients : "+numDeadClients;
    }
}
