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
    CameraFollow cam;

    public int dir = 1;
    public bool isGrounded;
    public bool isGrabbing;
    public bool isDying;
    public bool isFrozen;
    public GameObject jumpDirt;
    public GameObject cleaver;

    [HideInInspector]
    public bool inEndgameSequence;

    bool canFlipGravity = true;
    bool inGameEndTrigger;
    int storedDir;

    IEnumerator flipCoroutine;
    IEnumerator coyoteFramesCoroutine;
    IEnumerator deathCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        cam = FindObjectOfType<CameraFollow>();
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

            if (gm.PressingInput() && isGrounded)
            {
                if (inGameEndTrigger)
                {
                    FindObjectOfType<TankScript>().dontSpawnProjectiles = true;
                    gm.StopMusic();
                    isFrozen = true;
                    Time.timeScale = 0.33f;
                    CheckAndPlayClip("Player_ThrowCleaver", anim);
                    gm.PlaySFX(gm.sfx[0]);
                    GameObject d = Instantiate(jumpDirt, new Vector2(transform.position.x, transform.position.y - 1 * Mathf.Sign(rb.gravityScale)), transform.rotation);
                    d.transform.localScale = new Vector2(1, 1 * Mathf.Sign(rb.gravityScale));
                    rb.velocity = new Vector2(14.5f, 13.5f * Mathf.Sign(rb.gravityScale));
                    isGrounded = false;
                    cam.orientation = CameraFollow.CamOrientation.left;
                    StartCoroutine(EndSoundEffects());
                }
                else
                {
                    gm.PlaySFX(gm.sfx[0]);
                    GameObject d = Instantiate(jumpDirt, new Vector2(transform.position.x, transform.position.y - 1 * Mathf.Sign(rb.gravityScale)), transform.rotation);
                    d.transform.localScale = new Vector2(1, 1 * Mathf.Sign(rb.gravityScale));
                    rb.velocity = new Vector2(rb.velocity.x, 13.5f * Mathf.Sign(rb.gravityScale));
                    isGrounded = false;
                }
            }
        }
    }

    IEnumerator EndSoundEffects()
    {
        inEndgameSequence = true;
        yield return new WaitForSeconds(0.25f);
        gm.PlaySFX(gm.sfx[8]);
        yield return new WaitForSeconds(0.25f);
        gm.PlaySFX(gm.sfx[9]);
        yield return new WaitForSeconds(1);
        Time.timeScale = 1;
        cam.orientation = CameraFollow.CamOrientation.center;
        yield return new WaitForSeconds(6);
        StartCoroutine(gm.PlayCredits());
    }

    public void SpawnCleaver()
    {
        Instantiate(cleaver, GameObject.Find("CleaverPosition").transform.position, Quaternion.identity);
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
        while (!gm.PressingInput() || isFrozen)
            yield return null;
    }

    IEnumerator FlipGravityDelay()
    {
        canFlipGravity = false;
        yield return new WaitForSeconds(0.66f);
        canFlipGravity = true;
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
        gm.PlaySFX(gm.sfx[6]);
        transform.localScale = new Vector2(1, transform.localScale.y * -1f);
        rb.gravityScale *= -1;
        transform.localPosition += new Vector3(0, 0.5f * Mathf.Sign(rb.gravityScale));
        cam.transform.localPosition += new Vector3(0, 0.5f * Mathf.Sign(rb.gravityScale));
        StartCoroutine(FlipGravityDelay());
    }

    public void ResetGravity()
    {
        transform.localScale = new Vector2(1, Mathf.Abs(transform.localScale.y));
        rb.gravityScale = Mathf.Abs(rb.gravityScale);
    }

    public IEnumerator Die(Collider2D other)
    {
        cam.orientation = CameraFollow.CamOrientation.center;
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
        gm.StartCoroutine(gm.ResetToCheckpointCoroutine());
    }

    public void Reset()
    {
        if (deathCoroutine != null)
            StopCoroutine(deathCoroutine);
        isDying = false;
        rb.isKinematic = false;
        cam.orientation = CameraFollow.CamOrientation.center;
        transform.parent = null;
        rb.velocity = Vector2.zero;
        CheckAndPlayClip("Player_Idle", anim);
        ResetGravity();
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
                deathCoroutine = Die(other);
                StartCoroutine(deathCoroutine);
            }
        }
        if (other.tag == "GravityInverter" && canFlipGravity)
            InvertGravity();
        else if (other.tag == "GravityResetter")
            ResetGravity();

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground" && ((rb.velocity.y <= 1 && rb.gravityScale > 0) || (rb.velocity.y >= -1 && rb.gravityScale < 0)))
            isGrounded = true;

        if (collision.tag == "GameEndTrigger")
            inGameEndTrigger = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            if (coyoteFramesCoroutine != null)
                StopCoroutine(coyoteFramesCoroutine);
            coyoteFramesCoroutine = CoyoteFrames();
            StartCoroutine(coyoteFramesCoroutine);
        }

        if (collision.tag == "GameEndTrigger")
            inGameEndTrigger = false;
    }

    IEnumerator CoyoteFrames()
    {
        for (int i = 0; i < 14; i++)
            yield return new WaitForEndOfFrame();

        isGrounded = false;
        print("Finished");
        coyoteFramesCoroutine = null;
    }

    public void CheckAndPlayClip(string clipName, Animator anim)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(clipName))
        {
            anim.Play(clipName);
        }
    }
}
