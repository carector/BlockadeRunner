using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    public bool activated;
    public bool canRotate;

    Transform cannonPivot;
    Rigidbody2D rb;
    PlayerController ply;
    Rigidbody2D plyRb;
    Animator anim;
    CameraFollow cam;

    // Start is called before the first frame update
    void Start()
    {
        cannonPivot = GameObject.Find("TankCannonPivot").transform;
        rb = GetComponent<Rigidbody2D>();
        ply = FindObjectOfType<PlayerController>();
        plyRb = ply.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cam = FindObjectOfType<CameraFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            rb.velocity = new Vector2(plyRb.velocity.x * 1, 0);
        }

        if (canRotate)
            RotateCannon();
    }

    public void EnableTank()
    {
        activated = true;
        canRotate = true;
        anim.Play("TankFall");
        StartCoroutine(WaitAndShoot());
    }

    IEnumerator WaitAndShoot()
    {
        yield return new WaitForSeconds(6);
        anim.Play("TankFire");
        yield return new WaitForSeconds(1);
        StartCoroutine(WaitAndShoot());
    }

    public void RotateCannon()
    {
        Vector3 diff = ply.transform.position - cannonPivot.transform.position;
        float rot_z = Mathf.Clamp(Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg, -7f, 187f);
        cannonPivot.rotation = Quaternion.Lerp(cannonPivot.rotation, Quaternion.RotateTowards(cannonPivot.rotation, Quaternion.Euler(0, 0, rot_z), 1f), 0.15f);
    }

    public void EnableRotation()
    {
        canRotate = true;
    }

    public void ShakeCam()
    {
        //cam.ShakeCamera(8);
    }
}
