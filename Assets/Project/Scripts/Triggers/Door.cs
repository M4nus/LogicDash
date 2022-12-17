using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public LayerMask player;
    public TextMeshProUGUI text;
    [Range(0, 100)]
    public float blendDistance = 3f;
    public float distance = 4f;
    public float doorDelay = 0.5f;

    private bool canTrigger = false;
    private bool isOpened = false;

    private void Update()
    {
        FadeText();
    }

    void FadeText()
    {
        var blend = distance * blendDistance / 100f;
        var dist = (PlayerMovement.Instance.transform.localPosition - transform.localPosition).magnitude;
        text.alpha = Mathf.InverseLerp(distance, blend, dist) / distance;

        if(dist < distance)
        {
            if(isOpened)
            {
                return;
            }
            if(Keyboard.current.eKey.IsPressed() || (Gamepad.current != null && Gamepad.current.buttonEast.IsPressed()))
            {
                isOpened = true;
                text.gameObject.SetActive(false);
                OpenDoor();
            }
        }
    }

    public void OpenDoor()
    {
        GetComponent<Animator>().SetTrigger("OPENTHEDOOR");
        SoundSpawner.Instance.SpawnSound("Door");
        CameraShake.Instance.Shake(0.05f, 2f, 0.2f);
        StartCoroutine(DoorDelay());
    }

    private IEnumerator DoorDelay()
    {
        yield return new WaitForSeconds(doorDelay);
        canTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!canTrigger)
        {
            return;
        }

        if(collision.gameObject.layer == player.GetIndex())
        {
            canTrigger = false;
            SceneController.Instance.NextLevel();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!canTrigger)
        {
            return;
        }

        if(collision.gameObject.layer == player.GetIndex())
        {
            canTrigger = false;
            SceneController.Instance.NextLevel();
        }
    }
}
