using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraOrientation : MonoBehaviour
{
    public CameraFollow.CamOrientation orientation;
    CameraFollow cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<CameraFollow>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            cam.orientation = orientation;
    }
}
