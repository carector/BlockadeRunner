﻿// Retrieved from https://forum.unity.com/threads/creating-a-trail-of-sprites-getting-current-sprite-in-animation.251629/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTrailRenderer : MonoBehaviour
{
    public bool spawnClones = true;
    public int ClonesPerSecond = 10;
    private SpriteRenderer sr;
    private Transform tf;
    private List<SpriteRenderer> clones;
    public Color colorPerSecond = new Color(255, 255, 255, 1f);
    void Start()
    {
        tf = GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        clones = new List<SpriteRenderer>();
        StartTrailCoroutine();
    }

    private void OnEnable()
    {

    }

    void FixedUpdate()
    {
        for (int i = 0; i < clones.Count; i++)
        {
            clones[i].color -= colorPerSecond * Time.deltaTime*3;
            if (clones[i].color.a <= 0f || clones[i].transform.localScale == Vector3.zero)
            {
                Destroy(clones[i].gameObject);
                clones.RemoveAt(i);
                i--;
            }
        }
    }

    public void StartTrailCoroutine()
    {
        StartCoroutine(trail());
    }

    IEnumerator trail()
    {
        while(spawnClones || clones.Count > 0)
        {
            if (spawnClones)
            {
                var clone = new GameObject("trailClone");
                clone.transform.position = tf.position;

                var cloneRend = clone.AddComponent<SpriteRenderer>();
                cloneRend.sprite = sr.sprite;
                cloneRend.flipX = sr.flipX;
                cloneRend.color = new Color(1, 1, 1, 0.5f);
                cloneRend.sortingOrder = sr.sortingOrder - 1;
                clones.Add(cloneRend);
            }
            yield return new WaitForSeconds(1f / ClonesPerSecond);
        }
    }
}