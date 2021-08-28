using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    SpriteRenderer spr;
    Rigidbody2D rb;
    Animator anim;
    GameManager gm;
    BoxCollider2D col;

    public int dir = 1;
    public bool isGrounded;
    public bool isGrabbing;
    public bool isDying;
    public bool isFrozen;
    public GameObject jumpDirt;

    int storedDir;

    IEnumerator flipCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        anim = spr.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isGrabbing && gm.running && !isDying && !isFrozen)
            rb.velocity = new Vector2(10 * dir, rb.velocity.y);
    }

    private void Update()
    {
        if (gm.running && !isDying && !isFrozen)
        {
            if (!isGrabbing)
            {
                if (isGrounded)
                    CheckAndPlayClip("Player_Run", anim);
                else
                    CheckAndPlayClip("Player_Jump", anim);
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                gm.PlaySFX(gm.sfx[0]);
                GameObject d = Instantiate(jumpDirt, new Vector2(transform.position.x, transform.position.y - 1 * Mathf.Sign(rb.gravityScale)), transform.rotation);
                d.transform.localScale = new Vector2(1, 1 * Mathf.Sign(rb.gravityScale));
                rb.velocity = new Vector2(rb.velocity.x, 13.5f * Mathf.Sign(rb.gravityScale));
                isGrounded = false;
            }
        }
    }

    IEnumerator GrabAndFlip(Transform t)
    {
        transform.position = new Vector2(t.position.x - dir, transform.position.y);
        rb.velocity = Vector2.zero;
        dir *= -1;
        spr.flipX = !spr.flipX;

        // Only grab wall if we're in the air
        if (!isGrounded)
        {
            transform.parent = t;
            if (t.name == "LeftExploder")
                transform.localPosition = new Vector2(-0.72f, 0);
            else if (t.name == "RightExploder")
                transform.localPosition = new Vector2(0.72f, 0);

            rb.isKinematic = true;
            CheckAndPlayClip("Player_Grab", anim);
            yield return WaitForSpacePress();
            transform.parent = null;
            gm.PlaySFX(gm.sfx[0]);
            rb.isKinematic = false;
            rb.velocity = new Vector2(rb.velocity.x, 13.5f * Mathf.Sign(rb.gravityScale));
        }
        isGrabbing = false;
    }

    IEnumerator WaitForSpacePress()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
    }

    public void FreezePlayer(bool frozen)
    {
        if (frozen)
        {
            storedDir = dir;
            dir = 0;
            anim.SetFloat("RunSpeed", 0);
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        else
        {
            dir = storedDir;
            anim.SetFloat("RunSpeed", 1.1f);
            rb.isKinematic = false;
        }

        isFrozen = frozen;
    }

    public void InvertGravity()
    {
        transform.localScale = new Vector2(1, transform.localScale.y * -1f);
        rb.gravityScale *= -1;
    }

    public void ResetGravity()
    {
        transform.localScale = new Vector2(1, Mathf.Abs(transform.localScale.y));
        rb.gravityScale = Mathf.Abs(rb.gravityScale);
    }

    public IEnumerator Die(Collider2D other)
    {
        transform.parent = null;
        isDying = true;
        col.size = new Vector2(0.38f, col.size.y);
        Vector3 point = other.ClosestPoint(transform.position);
        Vector3 diff = transform.position - point;

        if(isGrabbing)
        {
            isGrabbing = false;
            dir *= -1;
            rb.isKinematic = false;
            StopCoroutine(flipCoroutine);
        }

        // Knock back player
        gm.PlaySFX(gm.sfx[2]);
        dir *= -1;
        rb.velocity = new Vector2(10 * dir, 9 * Mathf.Sign(rb.gravityScale));
        isGrounded = false;
        CheckAndPlayClip("Player_Falling", anim);

        // Wait until grounded
        while (!isGrounded)
            yield return null;

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        CheckAndPlayClip("Player_Down", anim);
        yield return new WaitForSeconds(0.5f);
        gm.PlaySFX(gm.sfx[3]);
        spr.color = Color.clear;
        yield return new WaitForSeconds(0.75f);
        gm.running = false;
        isDying = false;
        CheckAndPlayClip("Player_Idle", anim);
        ResetGravity();
        rb.isKinematic = false;
        gm.StartCoroutine(gm.ResetToCheckpoint());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDying)
        {
            if ((other.tag == "Reverser" || other.tag == "Exploder") && !isGrabbing)
            {
                if(other.tag == "Exploder")
                    StartCoroutine(other.GetComponentInParent<ExploderScript>().TriggerExplosion());

                isGrabbing = true;
                flipCoroutine = GrabAndFlip(other.transform);
                StartCoroutine(flipCoroutine);
            }
            if (other.tag == "DeathTrigger")
            {
                StartCoroutine(Die(other));
            }
        }
        if (other.tag == "GravityInverter")
            InvertGravity();
        else if (other.tag == "GravityResetter")
            ResetGravity();

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground" && ((rb.velocity.y <= 1 && rb.gravityScale > 0) || (rb.velocity.y >= -1 && rb.gravityScale < 0)))
            isGrounded = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
            isGrounded = false;
    }
    public void CheckAndPlayClip(string clipName, Animator anim)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(clipName))
        {
            anim.Play(clipName);
        }
    }
}
