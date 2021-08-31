using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    public bool activated;
    public bool dontSpawnProjectiles;
    public float[] delayTimes;

    public GameObject explosion;
    public LayerMask explosionMask;
    Transform cannonPivot;
    Transform crosshair;
    Rigidbody2D rb;
    PlayerController ply;
    Rigidbody2D plyRb;
    Animator anim;
    CameraFollow cam;
    GameManager gm;
    AudioSource audio;

    bool exploding;

    Vector3 initialPos;

    IEnumerator aimCoroutine;
    IEnumerator shootCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        gm = FindObjectOfType<GameManager>();
        crosshair = GameObject.Find("Crosshair").transform;
        cannonPivot = GameObject.Find("TankCannonPivot").transform;
        rb = GetComponent<Rigidbody2D>();
        ply = FindObjectOfType<PlayerController>();
        plyRb = ply.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cam = FindObjectOfType<CameraFollow>();
        audio = transform.GetChild(1).GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activated && !exploding)
        {
            if (ply.transform.position.x < transform.position.x + 8.5f)
                rb.velocity = new Vector2(7f, 0);
            else
                rb.velocity = new Vector2(10, 0);
        }
    }

    public void EnableTank()
    {
        print("Enabled tank");
        anim.Play("TankFall");
        shootCoroutine = WaitAndShoot();
        StartCoroutine(shootCoroutine);
    }

    public void ResetTank()
    {
        if(aimCoroutine != null)
            StopCoroutine(aimCoroutine);
        if(shootCoroutine != null)
            StopCoroutine(shootCoroutine);

        print("RESET TANK");
        activated = false;
        rb.velocity = Vector2.zero;
        anim.Play("TankPreFall");
        transform.position = initialPos;
        audio.Stop();

        

        cannonPivot.rotation = Quaternion.identity;
    }

    IEnumerator WaitAndShoot()
    {
        activated = true;
        int curDelay = 0;
        while (activated && !ply.isDying && curDelay < delayTimes.Length)
        {
            for(int i = 0; i < delayTimes[curDelay]; i++)
            {
                yield return new WaitForSecondsRealtime(1f);

                // this is so fucking stupid
                if (!activated || ply.isDying)
                    yield break;
            }

            aimCoroutine = AimCrosshair();
            StartCoroutine(aimCoroutine);
            yield return new WaitForSecondsRealtime(2f);

            if (!activated || ply.isDying)
                yield break;
            
            yield return new WaitForSecondsRealtime(2f);

            if (!activated || ply.isDying)
                yield break;

            anim.Play("TankFire");
            yield return new WaitForSecondsRealtime(1);

            if (!activated || ply.isDying)
                yield break;

            curDelay++;
        }
    }

    IEnumerator AimCrosshair()
    {
        gm.PlaySFXStoppable(gm.sfx[7]);
        crosshair.transform.localPosition = new Vector2(3.25f, 2.5f);
        anim.Play("TankLockOn");
        float time = 0;
        while (time < 2.75f)
        {
            if (!activated || ply.isDying)
                yield break;

            crosshair.position = Vector2.Lerp(crosshair.position, Vector2.MoveTowards(crosshair.position, ply.transform.position, Vector2.Distance(crosshair.position, ply.transform.position) / 10f), 0.1f);
            RotateCannon();
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        //cam.orientation = CameraFollow.CamOrientation.left;
    }

    public void ResetCannonRotation()
    {
        print("Resetting rotation");
        StartCoroutine(ResetCannonCoroutine());
    }

    IEnumerator ResetCannonCoroutine()
    {
        while(Mathf.Abs(cannonPivot.rotation.eulerAngles.z) > 1)
        {
            if (!activated || ply.isDying)
                yield break;

            ResetCannon();            
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        if (!activated || ply.isDying)
            yield break;

        cannonPivot.rotation = Quaternion.Euler(0, 0, 0);
    }

    void ResetCannon()
    {
        Vector3 diff = crosshair.transform.position - cannonPivot.transform.position;
        float rot_z = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        cannonPivot.rotation = Quaternion.Lerp(cannonPivot.rotation, Quaternion.Euler(0, 0, 0), 0.1f);
    }

    public void SpawnExplosion()
    {
        if (exploding)
        {
            GameObject g = Instantiate(explosion, transform.position, Quaternion.identity);
            g.transform.localScale = Vector2.one * 4;
            rb.velocity = Vector2.zero;
            audio.Stop();
            this.enabled = false;
        }
        else if(!dontSpawnProjectiles)
        {

            Debug.DrawRay(cannonPivot.position, cannonPivot.right * 20, Color.red, Mathf.Infinity);
            RaycastHit2D hit = Physics2D.CircleCast(cannonPivot.position, 1, cannonPivot.right, 120, explosionMask);
            if (hit.collider != null)
            {
                print(hit.point);
                StartCoroutine(ExplosionDelay(hit.point));
            }
        }
    }

    IEnumerator ExplosionDelay(Vector2 pos)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Instantiate(explosion, pos, Quaternion.identity);
    }

    public void PlayTankAudio()
    {
        audio.Play();
    }

    public void RotateCannon()
    {
        Vector3 diff = crosshair.transform.position - cannonPivot.transform.position;
        float rot_z = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        cannonPivot.rotation = Quaternion.Lerp(cannonPivot.rotation, Quaternion.Euler(0, 0, rot_z), 0.1f);
    }

    public void Explode()
    {
        GameObject g = Instantiate(explosion, transform.position, Quaternion.identity);
        g.transform.localScale = Vector2.one * 4;
        rb.velocity = Vector2.zero;
        audio.Stop();
        this.enabled = false;

        exploding = true;
    }

    public void ShakeCam()
    {
        //cam.ShakeCamera(8);
    }
}
