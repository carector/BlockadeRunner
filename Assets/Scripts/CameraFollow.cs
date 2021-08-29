using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum CamOrientation
    {
        left,
        center,
        right
    };

    public Transform target;
    public CamOrientation orientation = CamOrientation.center;
    bool shakingScreen;
    Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch(orientation)
        {
            case CamOrientation.left:
                cam.transform.localPosition = Vector2.Lerp(cam.transform.localPosition, new Vector2(-5, 0), 0.025f);
                break;
            case CamOrientation.center:
                cam.transform.localPosition = Vector2.Lerp(cam.transform.localPosition, Vector2.zero, 0.075f);
                break;
            case CamOrientation.right:
                cam.transform.localPosition = Vector2.Lerp(cam.transform.localPosition, new Vector2(5, 0), 0.025f);
                break;
        }
        transform.position = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, target.transform.position.y, transform.position.z), 0.05f);
    }

    public void ShakeCamera(int iterations)
    {
        StartCoroutine(ShakeCameraCoroutine(iterations));
    }

    IEnumerator ShakeCameraCoroutine(int iterations)
    {
        shakingScreen = true;
        int shakeIterations = iterations;
        int shakeAmount = 5;
        Vector3 storedPos = cam.transform.localPosition;
        while (shakeIterations > 0)
        {
            //float randX = UnityEngine.Random.Range(-shakeAmount, shakeAmount) * 0.05f;
            float randY = UnityEngine.Random.Range(-shakeAmount, shakeAmount) * 0.05f;
            cam.transform.localPosition = new Vector3(storedPos.x, storedPos.y + randY, cam.transform.localPosition.z);
            shakeIterations--;
            if (shakeIterations < 5)
                shakeAmount--;

            yield return new WaitForSeconds(0.025f);
        }

        transform.position = storedPos;
        shakingScreen = false;
    }
}
