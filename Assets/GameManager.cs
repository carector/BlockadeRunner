using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioClip[] sfx;
    public int checkpoint = 0;
    public bool running;
    public CheckpointScript[] checkpoints;
    public GameObject explosion;

    AudioSource footstepSource;
    AudioSource sfxSource;
    AudioSource musicSource;

    Transform cam;
    PlayerController ply;
    SpriteRenderer spr;
    BoxCollider2D col;

    ExploderScript[] allExploders;

    Image logo;
    Text beginText;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();
        sfxSource = transform.GetChild(1).GetComponent<AudioSource>();
        footstepSource = transform.GetChild(2).GetComponent<AudioSource>();

        beginText = GameObject.Find("BeginText").GetComponent<Text>();
        logo = GameObject.Find("Logo").GetComponent<Image>();

        ply = FindObjectOfType<PlayerController>();
        spr = ply.GetComponentInChildren<SpriteRenderer>();
        col = ply.GetComponent<BoxCollider2D>();
        cam = FindObjectOfType<Camera>().transform;

        allExploders = FindObjectsOfType<ExploderScript>();

        StartCoroutine(FirstTimeSetup());
    }

    // Update is called once per frame
    void Update()
    {
        // TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP TEMP 
        if (Input.GetKey(KeyCode.Alpha1))
            MoveToCheckpoint(0);
        else if (Input.GetKey(KeyCode.Alpha2))
            MoveToCheckpoint(1);
        else if (Input.GetKey(KeyCode.Alpha3))
            MoveToCheckpoint(2);
        else if (Input.GetKey(KeyCode.Alpha4))
            MoveToCheckpoint(3);
        else if (Input.GetKey(KeyCode.Alpha5))
            MoveToCheckpoint(4);
    }

    void MoveToCheckpoint(int checkpointNumber)
    {
        CheckpointScript c = checkpoints[checkpointNumber];
        ply.dir = c.dir;
        spr.flipX = (c.dir == -1);
        ply.transform.position = new Vector2(c.transform.position.x, c.transform.position.y - 0.5f);
    }

    IEnumerator FirstTimeSetup()
    {
        running = false;
        beginText.color = Color.black;
        yield return WaitForSpacePress();
        PlayMusic();
        beginText.color = Color.clear;
        StartCoroutine(FadeOutLogo());
        //yield return new WaitForSeconds(6.4f);
        //Instantiate(explosion, ply.transform.position, Quaternion.identity);
        //yield return new WaitForSeconds(0.5f);
        spr.color = Color.white;
        running = true;
    }

    IEnumerator FadeOutLogo()
    {
        while (logo.color.a > 0)
        {
            logo.color = new Color(1, 1, 1, logo.color.a - 0.01f);
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator ResetToCheckpoint()
    {
        if (checkpoint == -1)
        {
            ply.dir = 1;
            spr.flipX = false;
            ply.transform.position = new Vector2(-18, -1);
        }
        else
        {
            MoveToCheckpoint(checkpoint);
        }

        for (int i = 0; i < allExploders.Length; i++)
            allExploders[i].ResetSelf();

        col.size = new Vector2(0.68f, col.size.y);
        spr.color = Color.white;
        cam.transform.position = new Vector3(ply.transform.position.x, ply.transform.position.y, cam.transform.position.z);
        running = false;
        beginText.color = Color.black;
        yield return WaitForSpacePress();
        beginText.color = Color.clear;
        running = true;
    }

    IEnumerator WaitForSpacePress()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
    }

    public void PlaySFX(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx);
    }

    public void PlayFootstepSFX(AudioClip sfx)
    {
        footstepSource.PlayOneShot(sfx);
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void PlaySFXWithPitch(AudioClip sfx, float pitch)
    {

    }
}
