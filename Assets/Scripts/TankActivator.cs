using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankActivator : MonoBehaviour
{
    TankScript tank;
    CameraFollow cam;
    GameManager gm;

    public bool jumpUp;
    public AudioClip music;
    bool playedMusic;

    // Start is called before the first frame update
    void Start()
    {
        tank = FindObjectOfType<TankScript>();
        cam = FindObjectOfType<CameraFollow>();
        gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (!jumpUp)
            {
                if(!playedMusic)
                {
                    gm.PlayMusic(music);
                    playedMusic = true;
                }
                tank.EnableTank();
                cam.orientation = CameraFollow.CamOrientation.left;
                enabled = false;
            }
            else
            {
                Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
                tank.GetComponent<Animator>().Play("TankJump");
                tank.transform.position = new Vector3(playerPos.x - 8f, 162, 0);
                cam.orientation = CameraFollow.CamOrientation.left;
            }
        }
    }
}
