using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneController : SingletonPersistent<SceneController>
{
    #region Variables

    public CanvasGroup transitionPanel;
    public int currentScene = 1;

    #endregion

    private bool flag = false;

    #region Behaviour

    public IEnumerator Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        transitionPanel = GameObject.FindGameObjectWithTag("Panel").GetComponent<CanvasGroup>();
        SceneManager.activeSceneChanged += OnSceneChanged;
        transitionPanel.GetComponent<CanvasGroup>().alpha = 1f;
        yield return new WaitForSeconds(0.5f);
        yield return transitionPanel.DOFade(0f, 3f);
    }

    public void OnSceneChanged(Scene sceneOld, Scene sceneNew)
    {
        transitionPanel = GameObject.FindGameObjectWithTag("Panel").GetComponent<CanvasGroup>();
        transitionPanel.DOFade(0f, 1f);
    }

    public void ChangeLevel(int sceneIndex)
    {
        StartCoroutine(LoadLevel(sceneIndex));
    }

    public void NextLevel()
    {
        StartCoroutine(LoadLevel(currentScene + 1));
    }

    public void PreviousLevel()
    {
        StartCoroutine(LoadLevel(currentScene - 1));
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(currentScene);
    }

    #endregion

    #region Functionality

    private IEnumerator LoadLevel(int sceneIndex)
    {
        yield return transitionPanel.DOFade(1f, 1f).OnComplete(() => SceneManager.LoadScene(sceneIndex));
        currentScene = sceneIndex;
        yield return new WaitForSeconds(1f);
    }
    #endregion
}
