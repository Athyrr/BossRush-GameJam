using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    public enum AppState
    {
        None,
        MainMenu,
        Game,
        Credits
    }

    #region Fields

    public static AppManager Instance;

    private const string _gameSceneName = "SC_Game";
    private const string _menuSceneName = "SC_Menu";
    private const string _creditsSceneName = "SC_Credits";

    private AppState _currentAppState = AppState.None;


    #endregion

    #region Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _currentAppState = AppState.MainMenu;
        //Play();
    }

    #endregion

    #region Public API

    public bool Play()
    {
        if (_currentAppState != AppState.MainMenu)
            return false;

        _currentAppState = AppState.Game;

        SceneLoader.Instance.LoadScene(_gameSceneName);
        return true;
    }

    public bool Menu()
    {
        //if (_currentAppState != AppState.Game)
        //    return false;

        _currentAppState = AppState.MainMenu;

        SceneLoader.Instance.LoadScene(_menuSceneName);
        return true;
    }
    
    public bool Credits()
    {
        if (_currentAppState != AppState.MainMenu)
            return false;

        _currentAppState = AppState.Credits;

        SceneLoader.Instance.LoadScene(_creditsSceneName);
        return true;
    }

    public bool Quit()
    {
        if (_currentAppState != AppState.MainMenu)
            return false;

        Application.Quit();
        return true;
    }
    #endregion

}
