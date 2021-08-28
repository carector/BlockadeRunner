using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankActivator : MonoBehaviour
{
    TankScript tank;
    CameraFollow cam;

    // Start is called before the first frame update
    void Start()
    {
        tank = FindObjectOfType<TankScript>();
        cam = FindObjectOfType<CameraFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            tank.EnableTank();
            cam.orientation = CameraFollow.CamOrientation.left;
            enabled = false;
        }
    }
}
