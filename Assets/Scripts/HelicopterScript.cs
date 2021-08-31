using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterScript : MonoBehaviour
{
    Animator anim;
    AudioSource audio;
    Rigidbody2D rb;

    public bool hitPlayer;

    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;    
        audio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetHelicopter()
    {
        transform.position = startingPos;
        hitPlayer = false;
        audio.Stop();
        rb.velocity = Vector2.zero;

        anim.Play("HelicopterIdle");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !hitPlayer)
        {
            hitPlayer = true;
            anim.Play("HelicopterEnter");
            audio.Play();
            rb.velocity = new Vector2(10, 0);
        }
    }
}
