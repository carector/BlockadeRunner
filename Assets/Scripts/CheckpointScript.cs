using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointScript : MonoBehaviour
{
    public bool activated;
    public Sprite activeSprite;
    public int checkpointNumber;
    public int dir;

    GameManager gm;
    PlayerController ply;
    SpriteRenderer spr;

    Text checkText;
    Text pointText;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        ply = FindObjectOfType<PlayerController>();
        spr = GetComponent<SpriteRenderer>();
        checkText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        pointText = transform.GetChild(0).GetChild(1).GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EnableCheckPoint()
    {
        activated = true;
        spr.sprite = activeSprite;
        gm.checkpoint = checkpointNumber;
        ply.FreezePlayer(true);
        gm.PlaySFX(gm.sfx[4]);
        checkText.color = Color.white;
        yield return new WaitForSeconds(0.35f);
        checkText.color = Color.clear;
        pointText.color = Color.white;
        yield return new WaitForSeconds(0.35f);
        pointText.color = Color.clear;
        ply.FreezePlayer(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == ply.gameObject && !activated)
        {
            StartCoroutine(EnableCheckPoint());
        }
    }
}
