using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        else Destroy(gameObject);

        DontDestroyOnLoad(this);
    }
    #endregion

    [Header("Button UI Elements")]
    [SerializeField] private Image _interactButton;
    [SerializeField] private TextMeshProUGUI _interactText;
    [SerializeField] private Image _itemButton;
    [SerializeField] private TextMeshProUGUI _itemText;
    [Space(10)]
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _disabledColor;

    [Header("Game Timer Elements")]
    [SerializeField] private GameObject _sunDialArrow;
    [SerializeField] private float _arrowStartAngle;
    [SerializeField] private float _arrowEndAngle;

    [Header("Start Menu Elements")]
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _startMenuQuitButton;

    [Header("End Menu Elements")]
    [SerializeField] private GameObject _endMenu;
    [SerializeField] private TextMeshProUGUI _endScoreText;
    [SerializeField] private GameObject _endMenuQuitButton;

    [Header("Loading Screen Elements")]
    [SerializeField] private GameObject _loadingMenu;

    private void Start()
    {
        _endMenu.SetActive(false);
        EnableStartMenu();
    }

    #region IN-GAME UI
    public void SetInteractButton(bool active)
    {
        _interactButton.color = active ? _activeColor : _disabledColor;
        _interactText.color = active ? _activeColor : _disabledColor;
    }

    public void SetItemButton(bool active)
    {
        _itemButton.color = active ? _activeColor : _disabledColor;
        _itemText.color = active ? _activeColor : _disabledColor;
    }

    public void UpdateArrow(float elapsedTime, float totalTime)
    {
        float angle = _arrowStartAngle + elapsedTime / totalTime * (_arrowEndAngle - _arrowStartAngle);
        _sunDialArrow.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    #endregion

    #region START MENU UI
    private void EnableStartMenu()
    {
        _startMenu.SetActive(true);
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _startMenuQuitButton.SetActive(false);
        }
    }
    #endregion

    #region END-MENU UI
    public void EnableEndMenu()
    {
        _endMenu.SetActive(true);
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _endMenuQuitButton.SetActive(false);
        }
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
    #endregion

    #region BUTTONS
    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Debug.Log("restarting game");
        _endMenu.SetActive(false);
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame(int buildIndex)
    {
        LoadLevel(buildIndex);
    }
    #endregion

    #region SCENE MANAGEMENT
    public void LoadLevel(int buildIndex)
    {
        StartCoroutine(LoadSceneAsynchronously(buildIndex));
    }

    private IEnumerator LoadSceneAsynchronously(int buildIndex)
    {
        AsyncOperation loadSceneAsyncOperation = SceneManager.LoadSceneAsync(buildIndex);
        _loadingMenu.SetActive(true);
        _startMenu.SetActive(false);
        while (!loadSceneAsyncOperation.isDone)
        {
            float loadingProgress = Mathf.Clamp01(loadSceneAsyncOperation.progress / 0.9f);
            Debug.Log("Loading progress : "+loadingProgress);
            yield return null;
        }
        _loadingMenu.SetActive(false);
        //_gameTimer.SetActive(true);
    }
    #endregion

}
