using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening;

public class ImageFader : MonoBehaviour
{
    // The game objects to be shown, with each game object having multiple children
    // that represent different layers of the image
    public GameObject[] images;

    // The index of the current image
    private int currentImageIndex = 0;

    // The duration of the fade in/out animation
    public float fadeDuration = 2.0f;

    // The input action used to change the image
    public InputAction changeImageAction;

    public float cooldown = 2f;
    private bool inputWait = false;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        // Set the initial image to be the first game object in the array
        images[currentImageIndex].SetActive(true);
        images[currentImageIndex].GetComponent<Animator>().SetTrigger("IsActive");

        yield return new WaitForSeconds(0.5f);
        foreach(Transform child in images[currentImageIndex].transform)
        {
            child.GetComponent<CanvasGroup>().DOFade(1, 2f);
        }

        // Set up the input action to call the ChangeImage method when triggered
        //changeImageAction.performed += ctx => ChangeImage();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the change image action has been triggered

        if(!inputWait)
        {
            if(Keyboard.current.spaceKey.IsPressed() || (Gamepad.current != null && Gamepad.current.buttonSouth.IsPressed()))
            {
                inputWait = true;
                ChangeImage();
                StartCoroutine(Cooldown());
            }
        }
    }

    // Change to the next image in the array
    private void ChangeImage()
    {
        // Increment the current image index and wrap it around if necessary
        int nextImageIndex = currentImageIndex + 1;

        if(nextImageIndex >= images.Length)
        {
            StartCoroutine(ChangeScene());
        }

        // Fade out the current image and fade in the next image simultaneously
        foreach(Transform child in images[currentImageIndex].transform)
        {
            child.GetComponent<CanvasGroup>().DOFade(0, fadeDuration);
        }

        images[nextImageIndex].SetActive(true);

        foreach(Transform child in images[nextImageIndex].transform)
        {
            child.GetComponent<CanvasGroup>().DOFade(1, fadeDuration);
        }

        // Set the next image index as the current image index
        images[nextImageIndex].GetComponent<Animator>().SetTrigger("IsActive");
        currentImageIndex = nextImageIndex;

    }

    private IEnumerator ChangeScene()
    {
        foreach(Transform child in images[currentImageIndex].transform)
        {
            yield return child.GetComponent<CanvasGroup>().DOFade(0, fadeDuration);
        }
        AudioManager.Instance.BlendSongs(2f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        inputWait = false;
    }
}