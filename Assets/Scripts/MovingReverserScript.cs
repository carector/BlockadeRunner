using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingReverserScript : MonoBehaviour
{
    public bool moving;
    public float dir = 1;
    public float moveAmount;
    public AudioClip stopSound;
    public AudioClip startSound;
    AudioSource audio;
    Vector2 initialPos;
    float initialDir;

    IEnumerator movementCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        audio = transform.GetChild(0).GetComponent<AudioSource>();
        initialPos = transform.localPosition;
        initialDir = dir;
    }

    public void ResetPlatform()
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);
        transform.localPosition = initialPos;
        audio.Stop();
        dir = initialDir;
        moving = false;
    }

    IEnumerator MovePlatform()
    {
        moving = true;
        float initialYPos = transform.localPosition.y;
        audio.PlayOneShot(startSound);
        yield return new WaitForSeconds(0.25f);
        audio.Play();
        while (Mathf.Abs(transform.localPosition.y - initialYPos) < moveAmount)
        {
            transform.position += new Vector3(0, dir) * 0.1f;
            yield return new WaitForFixedUpdate();
        }
        audio.Stop();
        audio.PlayOneShot(stopSound);
        transform.localPosition = new Vector2(transform.localPosition.x, initialYPos + moveAmount * Mathf.Sign(dir));
        dir *= -1;
        moving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !moving)
        {
            movementCoroutine = MovePlatform();
            StartCoroutine(movementCoroutine);
        }
    }
}
