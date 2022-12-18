using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerMovement : Singleton<PlayerMovement>
{
    public Camera camera;
    public Nut nut;
    public Trajectory trajectory;
    public LineRenderer line;
    public VisualEffect trail;
    public ParticleSystem dashEffect;
    public Transform spawnPoint;
    public Transform nutSpawnPoint;

    public LayerMask nutLayer;
    public LayerMask enemiesLayer;
    public LayerMask triggerLayer;
    public LayerMask sleepLayer;
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public LayerMask ignoreLayer;

    [Range(0f, 1000f)]
    public float playerSpeed;
    public float throwForce;
    public float teleportDelay;

    private Vector2 force;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 direction;
    private Vector2 oldPos;

    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator anim;
    private float movement;
    private float distToGround;

    public bool hasNut = false;
    private bool isDash = false;
    private bool canDash = false;

    private Coroutine cooldown;

    public Material lineBefore;
    public Material lineAfter;

    public List<GameObject> eraseable = new List<GameObject>();

    #region Behaviour

    void Start()
    {
        camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        distToGround = GetComponent<Collider2D>().bounds.extents.y;

        if(SceneController.Instance.currentScene != 11)
        {
            GameManager.Instance.Spawn(spawnPoint.localPosition);
        }
        else
        {
            AudioManager.Instance.musicSources[0].pitch = 1f;
            AudioManager.Instance.musicSources[1].pitch = 1f;
            GameManager.Instance.isDead = false;
            anim.SetBool("isDead", false);
            anim.SetBool("isStanding", true);
            GameManager.Instance.canMove = false;
            StartCoroutine(WaitForAnim());
        }
    }

    void FixedUpdate()
    {
        SettingValues();
        MoveHorizontally();
        trajectory.UpdateDots(nut.transform.position, force);
        line.gameObject.SetActive(canDash);
    }

    private void Update()
    {
        if(GameManager.Instance.isDead)
        {
            if(Keyboard.current.spaceKey.IsPressed() || (Gamepad.current != null && Gamepad.current.buttonSouth.IsPressed()))
            {
                GameManager.Instance.isDead = false;
                GameManager.Instance.Spawn(spawnPoint.position);
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(nutLayer.Contains(collision.gameObject.layer))
        {
            if(cooldown != null)
            {
                StopCoroutine(cooldown);
            }
            isDash = false;
            CatchNut();
        }
        if(enemiesLayer.Contains(collision.gameObject.layer))
        {
            CameraShake.Instance.Shake(0.2f, 0.2f);
            GameManager.Instance.Death();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(sleepLayer.Contains(collision.gameObject.layer))
        {
            GameManager.Instance.canMove = false;
            rb.velocity = Vector2.zero;
            anim.SetBool("isDead", true);
            AudioManager.Instance.BlendSongs(7f, 1f);
            AudioManager.Instance.musicSources[0].pitch = 1f;
            AudioManager.Instance.musicSources[1].pitch = 1f;
            StartCoroutine(ChangeCoscienscious());
        }
    }

    #endregion

    #region Controls

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<float>();
        if(Gamepad.current != null )
        {
            if(movement > 0.1f)
            {
                movement = 1f;
            }
            else if(movement < -0.1f)
            {
                movement = -1f;   
            }
        }

        anim.SetFloat("isMoving", Mathf.Abs(movement));
        GetComponent<SpriteRenderer>().flipX = movement >= 0 ? false : true;
    }

    public void Throw(InputAction.CallbackContext context)
    {
        if(!hasNut || !GameManager.Instance.canMove)
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
        if(hasNut || isDash || !canDash || !GameManager.Instance.canMove)
        {
            return;
        }

        if(context.performed)
        {
            CameraShake.Instance.Shake(0.1f, 0.2f);
            SoundSpawner.Instance.SpawnSound("Dash");
            oldPos = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.2f, 0f);
            transform.localPosition = nut.transform.localPosition;
            rb.velocity = Vector2.zero;
            isDash = true;
            canDash = false;

            trail.SetVector3("Start Position", oldPos);
            trail.SetVector3("End Position", nut.transform.localPosition);
            trail.SendEvent("OnDash");
            var dashEffectInstance = Instantiate(dashEffect, transform.position, Quaternion.identity);
            Destroy(dashEffectInstance.gameObject, 2f);
            RemoveTriggered();
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
            CameraShake.Instance.Shake(0.1f, 0.2f);
            SoundSpawner.Instance.SpawnSound("Dash");
            nut.transform.localPosition = transform.localPosition;
            RemoveTriggered();
        }
    }

    #endregion

    #region Functionality

    public void SettingValues()
    {
        line.SetPosition(0, new Vector3(transform.localPosition.x, transform.localPosition.y - 0.3f, 0f));
        line.SetPosition(1, (line.GetPosition(2) + line.GetPosition(0)) / 2.0f);
        line.SetPosition(2, nut.transform.localPosition);

        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.localPosition, nut.transform.localPosition, triggerLayer);
        if(hits.Length > 0)
        {
            eraseable = new List<GameObject>(hits.Length);
            line.sharedMaterial = lineAfter;
            for(int i = 0; i < hits.Length; i++)
            {
                eraseable.Add(hits[i].collider.gameObject);
            }
        }
        else
        {
            line.sharedMaterial = lineBefore;
            eraseable.Clear();
        }
        startPoint = transform.position;
        endPoint = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        
        if(Gamepad.current != null && Gamepad.current.rightStick.ReadValue() != Vector2.zero)
        {
            direction = Gamepad.current.rightStick.ReadValue();
        }
        else
        {
            direction = (endPoint - startPoint).normalized;
        }

        force = direction * throwForce;
    }

    public void CatchNut()
    {
        canDash = false;
        hasNut = true;
        nut.transform.parent = transform;
        nut.transform.position = transform.position;
        nut.rb.velocity = Vector3.zero;
        nut.rb.isKinematic = true;

        Physics2D.IgnoreCollision(nut.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
        trajectory.Show();
    }

    public void RemoveTriggered()
    {
        foreach(GameObject trigger in eraseable)
        {
            SoundSpawner.Instance.SpawnSound("Explosion");
            trigger.SetActive(false);
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, distToGround + 0.5f, ~ignoreLayer);
        bool isGrounded = (hit.collider != null);

        anim.SetBool("isFlying", !isGrounded);
        return isGrounded;
    }

    public void MoveHorizontally()
    {
        if(!IsGrounded() || !GameManager.Instance.canMove)
        {
            return;
        }
        rb.velocity = new Vector2(movement * playerSpeed * Time.fixedDeltaTime, rb.velocity.y);
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(teleportDelay);
        Physics2D.IgnoreCollision(nut.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), false);
        isDash = false;
        canDash = true;
    }

    public IEnumerator WaitForAnim()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.canMove = true;
        anim.SetBool("isStanding", false);
    }

    public IEnumerator ChangeCoscienscious()
    {
        yield return new WaitForSeconds(4f);
        SceneController.Instance.NextLevel();
    }

    #endregion
}
