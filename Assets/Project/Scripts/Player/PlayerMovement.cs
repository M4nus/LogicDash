using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Singleton<PlayerMovement>
{
    public Camera camera;
    public Nut nut;

    public LayerMask nutLayer;
    public LayerMask enemiesLayer;
    public LayerMask wallLayer;

    [Range(0f, 1000f)]
    public float playerSpeed;
    public float throwForce;

    private Vector2 force;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 direction;



    private Rigidbody2D rb;
    private float movement;

    private bool hasNut = false;
    private bool isDash = false;


    void Start()
    {
        camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        SettingValues();
        MoveHorizontally();
        Debug.DrawLine(startPoint, endPoint);
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<float>();
    }

    public void Throw(InputAction.CallbackContext context)
    {
        if(!hasNut)
        {
            return;
        }
        nut.Throw(force);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(hasNut || isDash)
        {
            return;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("NoNut!!");
        if(collision.gameObject.layer == nutLayer)
        {
            Debug.Log("HasNut!");
            hasNut = true;
            nut.transform.parent = transform;
        }
    }


    public void SettingValues()
    {
        startPoint = transform.position;
        endPoint = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        direction = (startPoint - endPoint).normalized;
        force = direction * throwForce;
    }

    public void MoveHorizontally()
    {
        rb.velocity = Vector2.right * movement * playerSpeed * Time.fixedDeltaTime;
    }
}
