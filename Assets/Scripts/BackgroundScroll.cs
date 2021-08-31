using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float multiplier = 0.95f;

    Transform ply;
    Vector3 storedPlayerPos;

    // Start is called before the first frame update
    void Start()
    {
        ply = FindObjectOfType<Camera>().transform;
        storedPlayerPos = ply.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 difference = ply.transform.position - storedPlayerPos;
        storedPlayerPos = ply.transform.position;
        transform.position -= difference * multiplier;
    }
}
