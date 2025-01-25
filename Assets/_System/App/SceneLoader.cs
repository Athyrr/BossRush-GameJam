using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Fields

    public static SceneLoader Instance;

    [SerializeField]
    private GameObject _loadingScreen = null;

    private Coroutine _loadingCoroutine = null;

    private Animator _transitionAnimator = null;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _transitionAnimator = _loadingScreen.GetComponent<Animator>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        _loadingScreen.SetActive(false);
    }

    #endregion


    #region Public API

    public bool LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return false;

        if (_loadingCoroutine != null)
            return false;

        if (_transitionAnimator == null)
            return false;

        _loadingCoroutine = StartCoroutine(LoadSceneAsync(sceneName));
        return true;
    }

    #endregion


    #region Private API

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        _loadingScreen.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                while(!_transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("Anim_Transition_Idle")) 
                    yield return null;  

                yield return new WaitForSeconds(0.3f);
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        _transitionAnimator.SetTrigger(name: "fadeInTrigger");

        AnimatorStateInfo stateInfo = _transitionAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        _loadingScreen.SetActive(false);
        _loadingCoroutine = null;
    }

    #endregion

}
