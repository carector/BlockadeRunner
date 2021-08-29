using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingReverserScript : MonoBehaviour
{
    public bool moving;
    public int dir = 1;
    public float moveAmount;
    public AudioClip stopSound;
    AudioSource audio;
    Vector2 initialPos;
    int initialDir;

    IEnumerator movementCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        audio = transform.GetChild(0).GetComponent<AudioSource>();
        initialPos = transform.position;
        initialDir = dir;
    }

    public void ResetPlatform()
    {
        if(movementCoroutine != null)
            StopCoroutine(movementCoroutine);
        transform.position = initialPos;
        audio.Stop();
        dir = initialDir;
        moving = false;
    }

    IEnumerator MovePlatform()
    {
        moving = true;
        float initialYPos = transform.position.y;
        audio.Play();
        while (Mathf.Abs(transform.position.y - initialYPos) < moveAmount)
        {
            transform.position += new Vector3(0, dir) * 0.1f;
            yield return new WaitForFixedUpdate();
        }
        audio.Stop();
        audio.PlayOneShot(stopSound);
        transform.position = new Vector2(transform.position.x, initialYPos + moveAmount * dir);
        dir *= -1;
        moving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !moving)
        {
            movementCoroutine = MovePlatform();
            StartCoroutine(movementCoroutine);
        }
    }
}
