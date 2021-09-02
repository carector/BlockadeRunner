using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.U2D;
public class GameManager : MonoBehaviour
{
    public AudioClip[] sfx;
    public int checkpoint = 0;
    public bool running;
    public CheckpointScript[] checkpoints;
    public GameObject explosion;
    public string[] creditsLines;
    public AudioMixer mixer;
    public Image[] xouts;
    AudioSource footstepSource;
    AudioSource sfxSource;
    AudioSource musicSource;
    AudioSource stoppableSfx;

    [System.Serializable]
    public class PlayerStats
    {
        public int numDeaths;
        public float runtime;
    }

    PlayerStats pStats;

    public io.newgrounds.core ngio_core;

    CameraFollow cam;
    PixelPerfectCamera ppcam; // nice
    PlayerController ply;
    SpriteRenderer spr;
    BoxCollider2D col;

    ExploderScript[] allExploders;
    MovingReverserScript[] allPlatforms;

    TankScript tank;
    HelicopterScript heli;

    bool soundEnabled = true;
    bool musicEnabled = true;
    public bool timerActive = false;

    int storedScreenTouches;

    Image logo;
    Text beginText;
    Text creditsText;
    Text timerText;
    Text deathsText;

    // Start is called before the first frame update
    void Start()
    {
        ngio_core = FindObjectOfType<io.newgrounds.core>();
        pStats = new PlayerStats();

        Application.targetFrameRate = 60;
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();
        sfxSource = transform.GetChild(1).GetComponent<AudioSource>();
        footstepSource = transform.GetChild(2).GetComponent<AudioSource>();
        stoppableSfx = transform.GetChild(3).GetComponent<AudioSource>();

        beginText = GameObject.Find("BeginText").GetComponent<Text>();
        creditsText = GameObject.Find("CreditsText").GetComponent<Text>();
        timerText = GameObject.Find("TimerText").GetComponent<Text>();
        deathsText = GameObject.Find("DeathsText").GetComponent<Text>();
        timerText.color = Color.clear;
        deathsText.color = Color.clear;
        logo = GameObject.Find("Logo").GetComponent<Image>();

        if (Application.platform == RuntimePlatform.Android)
            beginText.text = "Touch the screen to begin";
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            beginText.text = "Press space or touch the screen to begin";

        ply = FindObjectOfType<PlayerController>();
        spr = ply.GetComponentInChildren<SpriteRenderer>();
        col = ply.GetComponent<BoxCollider2D>();
        cam = FindObjectOfType<CameraFollow>();
        ppcam = FindObjectOfType<PixelPerfectCamera>();
        tank = FindObjectOfType<TankScript>();
        heli = FindObjectOfType<HelicopterScript>();

        allExploders = FindObjectsOfType<ExploderScript>();
        allPlatforms = FindObjectsOfType<MovingReverserScript>();

        StartCoroutine(FirstTimeSetup());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToCheckpoint();
        }

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {

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
            else if (Input.GetKey(KeyCode.Alpha6))
                MoveToCheckpoint(5);
            if (Input.GetKey(KeyCode.Alpha7))
                MoveToCheckpoint(6);
            if (Input.GetKey(KeyCode.Alpha8))
                MoveToCheckpoint(7);
        }
    }

    private void FixedUpdate()
    {
        if (timerActive)
        {
            deathsText.text = "Deaths: " + pStats.numDeaths.ToString();

            pStats.runtime += Time.deltaTime;
            float timer = pStats.runtime;
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer - minutes * 60);
            int milliseconds = Mathf.FloorToInt(((timer - (minutes * 60) - seconds)) * 100);
            string niceTime = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            timerText.text = niceTime;
        }
    }

    private void LateUpdate()
    {
        storedScreenTouches = Input.touchCount;
    }

    public int GetPlayerDeaths()
    {
        return pStats.numDeaths;
    }

    public void IncreasePlayerDeaths()
    {
        pStats.numDeaths++;
    }

    public float GetPlayerTime()
    {
        return pStats.runtime;
    }

    public void UnlockMedal(int id)
    {
        io.newgrounds.components.Medal.unlock medalToUnlock = new io.newgrounds.components.Medal.unlock();
        medalToUnlock.id = id;
        medalToUnlock.callWith(ngio_core);
        print("Unlocked medal " + id);
    }

    // Times: 10717
    // Deaths: 10718
    public void PostScore(int id, int value)
    {
        io.newgrounds.components.ScoreBoard.postScore scoreToPost = new io.newgrounds.components.ScoreBoard.postScore();
        scoreToPost.id = id;
        scoreToPost.value = value;
        scoreToPost.callWith(ngio_core);
        print("Posted score " + id);
    }

    void OnMedalUnlocked(io.newgrounds.results.Medal.unlock result)
    {
        io.newgrounds.objects.medal medal = result.medal;
        Debug.Log("Medal Unlocked: " + medal.name + " (" + medal.value + " points)");
    }


    void MoveToCheckpoint(int checkpointNumber)
    {
        CheckpointScript c = checkpoints[checkpointNumber];
        ply.dir = c.dir;
        spr.flipX = (c.dir == -1);
        ply.transform.position = new Vector2(c.transform.position.x, c.transform.position.y - 0.5f);
    }

    public IEnumerator PlayCredits()
    {
        for (int i = 0; i < creditsLines.Length; i++)
        {
            creditsText.text = creditsLines[i];
            yield return new WaitForSeconds(4);
            creditsText.text = "";
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(1);
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;

        Color c = Color.clear;
        float volume = 0;
        if (!musicEnabled)
        {
            volume = -80;
            c = Color.white;
        }

        mixer.SetFloat("MusicVolume", volume);
        xouts[0].color = c;
    }

    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;

        Color c = Color.clear;
        float volume = 0;
        if (!soundEnabled)
        {
            volume = -80;
            c = Color.white;
        }

        mixer.SetFloat("SFXVolume", volume);
        xouts[1].color = c;
    }


    IEnumerator FirstTimeSetup()
    {
        running = false;
        beginText.color = Color.white;
        yield return WaitForInputPress();
        PlayMusic();
        beginText.color = Color.clear;
        StartCoroutine(FadeOutLogo());
        yield return new WaitForSeconds(6.4f);
        Instantiate(explosion, ply.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        spr.color = Color.white;
        deathsText.color = Color.white;
        timerText.color = Color.white;
        timerActive = true;
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

    public void ResetToCheckpoint()
    {
        if (running && !ply.inEndgameSequence)
            StartCoroutine(ResetToCheckpointCoroutine());
    }

    public IEnumerator ResetToCheckpointCoroutine()
    {
        ply.Reset();
        cam.ResetOrientationInstant();

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

        for (int i = 0; i < allPlatforms.Length; i++)
            allPlatforms[i].ResetPlatform();

        if (tank.activated)
            tank.ResetTank();

        if (heli.hitPlayer)
            heli.ResetHelicopter();

        sfxSource.Stop();

        col.size = new Vector2(0.68f, col.size.y);
        spr.color = Color.white;
        cam.transform.position = new Vector3(ply.transform.position.x, ply.transform.position.y, cam.transform.position.z);
        running = false;
        beginText.color = Color.white;
        yield return WaitForInputPress();

        // Reset anything player might've been attached to during reset
        for (int i = 0; i < allExploders.Length; i++)
            allExploders[i].ResetSelf();

        for (int i = 0; i < allPlatforms.Length; i++)
            allPlatforms[i].ResetPlatform();

        beginText.color = Color.clear;
        running = true;
    }

    public bool PressingInput()
    {
        bool value = false;
        if (Input.GetKeyDown(KeyCode.Space))
            return true;
        else if (Input.touchCount > storedScreenTouches)
        {
            // Make sure our touch is within the bounds of the screen so we don't jump when we press a UI button
            Touch touch = Input.GetTouch(0);
            Vector2 screenPos = touch.position;

            //float touchableWidth = ppcam.refResolutionX * ((float)Screen.height / ppcam.refResolutionY); // Gets ratio of cam space to full window
            //float offset = (Screen.width - touchableWidth) / 2;
            float offset = Screen.height - (Screen.height / 4f);

            if (screenPos.y <= offset && !Input.GetMouseButtonUp(0))
                value = true;
        }
        return value;
    }

    IEnumerator WaitForInputPress()
    {
        while (!PressingInput())
            yield return null;
    }

    public void PlaySFX(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusic(AudioClip music)
    {
        musicSource.Stop();
        musicSource.clip = music;
        musicSource.Play();
    }

    public void PlaySFXStoppable(AudioClip sfx)
    {
        stoppableSfx.Stop();
        stoppableSfx.PlayOneShot(sfx);
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
