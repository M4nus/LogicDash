using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Menu : MonoBehaviour
{
    public CanvasGroup transition;

    public float fadeDuration = 3.0f;

    public float cooldown = 2f;
    private bool inputWait = true;


    IEnumerator Start()
    {
        transition.alpha = 1f;
        yield return new WaitForSeconds(1f);
        transition.DOFade(0, 2f);
        inputWait = false;
    }

    void Update()
    {
        if(!inputWait)
        {
            if(Keyboard.current.spaceKey.IsPressed() || (Gamepad.current != null && Gamepad.current.buttonSouth.IsPressed()))
            {
                inputWait = true;
                StartCoroutine(ChangeScene());
            }
        }
    }

    private IEnumerator ChangeScene()
    {
        yield return transition.DOFade(1, fadeDuration);
        //AudioManager.Instance.BlendSongs(fadeDuration + 0.5f, 1f);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(1);
    }
}