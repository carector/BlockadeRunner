using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollider : MonoBehaviour
{
    PlayerController ply;
    Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
        ply = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        col.enabled = (ply.transform.position.y - 1.25f >= transform.position.y);
    }
}
