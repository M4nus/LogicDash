using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : SingletonPersistent<SceneController>
{
    #region Variables

    public int currentScene = 0;

    #endregion

    private bool flag = false;

    #region Behaviour

    public void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void ChangeLevel(int sceneIndex)
    {
        LoadLevel(sceneIndex);
    }

    public void NextLevel()
    {
        LoadLevel(currentScene + 1);
    }

    public void PreviousLevel()
    {
        LoadLevel(currentScene - 1);
    }

    #endregion

    #region Functionality

    private void LoadLevel(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        currentScene = sceneIndex;
    }
    #endregion
}
