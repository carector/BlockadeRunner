using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterScript : MonoBehaviour
{
    Animator anim;
    AudioSource audio;
    Rigidbody2D rb;

    bool hitPlayer;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
