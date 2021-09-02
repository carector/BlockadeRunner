using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockMedalOnCollide : MonoBehaviour
{
    public int medalID;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            gm.UnlockMedal(medalID);
            Destroy(this.gameObject);
        }
    }
}
