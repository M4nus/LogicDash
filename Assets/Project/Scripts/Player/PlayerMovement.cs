using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Singleton<PlayerMovement>
{
    public Camera camera;
    public Nut nut;
    public Trajectory trajectory;

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
    private bool canDash = false;
    //private bool 

    private Coroutine cooldown;


    void Start()
    {
        camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        SettingValues();
        MoveHorizontally();
        trajectory.UpdateDots(nut.transform.position, force);
        Debug.Log("CanDash? : " + isDash);
    }

    public void SettingValues()
    {
        startPoint = transform.position;
        endPoint = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        direction = (endPoint - startPoint).normalized;
        force = direction * throwForce;
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
        if(context.performed)
        {
            hasNut = false;
            nut.rb.isKinematic = false;
            nut.transform.parent = null;
            nut.Throw(force);
            trajectory.Hide();
            canDash = false;
            cooldown = StartCoroutine(Cooldown());
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(hasNut || isDash || !canDash)
        {
            return;
        }

        if(context.performed)
        {
            rb.MovePosition(nut.transform.position);
            isDash = true;
            canDash = false;
            cooldown = StartCoroutine(Cooldown());
        }
    }

    public void Summon(InputAction.CallbackContext context)
    {
        if(hasNut)
        {
            return;
        }

        if(context.performed)
        {
            nut.transform.localPosition = transform.localPosition;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == nutLayer.GetIndex())
        {
            if(cooldown != null)
            {
                StopCoroutine(cooldown);
            }
            isDash = false;
            CatchNut();
        }
    }

    public void CatchNut()
    {
        hasNut = true;
        nut.transform.parent = transform;
        nut.transform.position = transform.position;
        nut.rb.velocity = Vector3.zero;
        nut.rb.isKinematic = true;

        Physics2D.IgnoreCollision(nut.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
        trajectory.Show();
    }

    public void MoveHorizontally()
    {
        rb.velocity = Vector2.right * movement * playerSpeed * Time.fixedDeltaTime;
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Here I am!");
        Physics2D.IgnoreCollision(nut.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), false);
        isDash = false;
        canDash = true;
    }
}
