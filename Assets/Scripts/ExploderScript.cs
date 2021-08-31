using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploderScript : MonoBehaviour
{
    public Text screenText;
    public GameObject explosion;

    bool exploding;

    PlayerController ply;
    Transform leftExploder;
    Transform rightExploder;
    GameManager gm;

    Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;

        gm = FindObjectOfType<GameManager>();
        ply = FindObjectOfType<PlayerController>();
        leftExploder = transform.GetChild(1);
        rightExploder = transform.GetChild(2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetSelf()
    {
        transform.position = initialPos;
        screenText.text = "";
        exploding = false;
        leftExploder.transform.localPosition = new Vector2(-1.5f, 0);
        rightExploder.transform.localPosition = new Vector2(1.5f, 0);
    }

    public IEnumerator TriggerExplosion()
    {
        if (exploding)
            yield break;

        yield return new WaitForEndOfFrame();
        exploding = true;
        gm.PlaySFX(gm.sfx[5]);
        screenText.text = "!";
        // Check player position for which side we should detonate
        if (ply.transform.position.x < transform.position.x)
            leftExploder.transform.localPosition = new Vector2(-1, 0);
        else
            rightExploder.transform.localPosition = new Vector2(1, 0);

        yield return new WaitForSeconds(0.45f);
        Instantiate(explosion, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.25f);
        transform.position = new Vector2(transform.position.x, -250);
    }
}
