using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Nut : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Throw(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
