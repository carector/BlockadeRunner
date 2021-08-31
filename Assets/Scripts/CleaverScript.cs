using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaverScript : MonoBehaviour
{
    Transform tank;
    TankScript ts;
    GameManager gm;
    public Animator spinAnimator;
    bool hitTank;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        tank = GameObject.Find("CannonCenter").transform;
        ts = FindObjectOfType<TankScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!hitTank)
            transform.position = Vector2.MoveTowards(transform.position, tank.position, 0.01f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "TankCannon" && !hitTank)
        {
            spinAnimator.enabled = false;
            transform.parent = tank;
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 45);
            transform.localPosition = new Vector2(0.715f, -0.237f);
            transform.localRotation = Quaternion.identity;
            gm.PlaySFX(gm.sfx[10]);
            ts.Explode();
            hitTank = true;
        }
    }
}
