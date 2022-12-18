using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Ending : MonoBehaviour
{
    IEnumerator Start()
    {
        SceneController.Instance.transitionPanel.GetComponent<CanvasGroup>().alpha = 1f;
        yield return new WaitForSeconds(4f);
        yield return SceneController.Instance.transitionPanel.DOFade(0, 3f);
        yield return StartCoroutine(End());
    }

    public IEnumerator End()
    {
        yield return new WaitForSeconds(12f);
        yield return SceneController.Instance.transitionPanel.DOFade(1, 5f);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }
}
