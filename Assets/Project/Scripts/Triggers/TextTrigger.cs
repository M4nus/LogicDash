using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextTrigger : MonoBehaviour
{
    public TextMeshProUGUI text;
    public LayerMask layer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(layer.Contains(collision.gameObject.layer))
        {
            text.DOFade(1f, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(layer.Contains(collision.gameObject.layer))
        {
            text.DOFade(0f, 0.5f);
        }
    }
}
