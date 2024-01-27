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

    [Header("Game Timer elements")]
    [SerializeField] private GameObject _gameTimer;
    [SerializeField] private TextMeshProUGUI _timerText;

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
}
