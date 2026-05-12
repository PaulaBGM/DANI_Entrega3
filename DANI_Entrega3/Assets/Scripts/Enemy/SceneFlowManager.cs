using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance;

    [SerializeField] private SceneManagerSo _sceneManager;
    [SerializeField] private float delayBeforeLoad = 2f; // para usar tras ganar o perder

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

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(_sceneManager.MainMenuSceneName);
    }

    public void LoadInGameScene()
    {
        PlaySFXButton();
        SceneManager.LoadScene(_sceneManager.InGameSceneName);
    }

    public void QuitGame()
    {
        PlaySFXButton();
        Application.Quit();
    }

    public void LoadSceneAfterDelay(string sceneName)
    {
        StartCoroutine(LoadSceneDelayed(sceneName));
    }

    private IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(sceneName);
    }

    private void PlaySFXButton()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.audioLibrary.buttonSfx);
    }
}
