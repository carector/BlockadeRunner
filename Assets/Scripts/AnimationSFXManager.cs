using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSFXManager : MonoBehaviour
{
    public AudioClip[] sfx;
    public AudioSource source;
    public GameObject receiver;
    GameManager gm;
    CameraFollow cam;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        cam = FindObjectOfType<CameraFollow>();
    }

    public void PlaySFX(int index)
    {
        if (sfx.Length <= index)
            return;

        if (gm == null)
            gm = FindObjectOfType<GameManager>();

        if (source != null)
            source.PlayOneShot(sfx[index]);
        else
            gm.PlaySFX(sfx[index]);
    }

    public void PlaySFXWithRandomPitch(int index)
    {
        float rand = Random.Range(0.8f, 1.2f);
        if (source != null)
        {
            source.pitch = rand;
            source.PlayOneShot(sfx[index]);
        }
        else
            gm.PlaySFXWithPitch(sfx[index], rand);
    }
    public void PlayRandomSFX()
    {
        if (source != null)
            source.PlayOneShot(sfx[Random.Range(0, sfx.Length)]);
        else
            gm.PlaySFX(sfx[Random.Range(0, sfx.Length)]);
    }

    public void PlayFootstepSFX()
    {
        gm.PlayFootstepSFX(sfx[Random.Range(0, sfx.Length)]);
    }

    public void PlaySFXStoppable(int index)
    {
        if (gm == null)
            gm = FindObjectOfType<GameManager>();
        gm.PlaySFXStoppable(sfx[index]);
    }

    public void SendMessageToReceiver(string message)
    {
        receiver.SendMessage(message);
    }
}
