using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : Singleton<SceneController>
{
    #region Variables

    public int currentScene = 0;

    #endregion

    #region Behaviour

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
        UnloadLevel();
        if(sceneIndex <= 0)
        {
            sceneIndex = 1;
        }
        if(sceneIndex > SceneManager.sceneCountInBuildSettings - 1)
        {
            sceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        }

        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        currentScene = sceneIndex;
    }

    private void UnloadLevel()
    {
        if(currentScene <= 0)
        {
            currentScene = 0;
            return;
        }

        SceneManager.UnloadSceneAsync(currentScene);
        currentScene = 0;
    }
    #endregion
}
