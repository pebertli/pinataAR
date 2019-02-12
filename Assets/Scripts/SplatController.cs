using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatController : MonoBehaviour
{
    public GameObject[] splat;
    public GameObject HUD;

    bool activated = false;
    float activatedTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            for (int i = 0; i < 6; i++)
                NewSplat();
            activated = true;
            activatedTime = 3f;
        }

        if(activated)
        {
            activatedTime -= Time.deltaTime*0.5f;
            if(activatedTime <= 0)
            {
                activated = false;
            }
        }
    }

    public void DoSplash(int amount)
    {
        for (int i = 0; i < amount; i++)
            NewSplat();
        activated = true;
        activatedTime = 3f;
    }

    IEnumerator UpdateSplats(Transform splatTransform, SpriteRenderer renderer)
    {
        for (float f = 1f; f >= 0; f -= 0.001f)
        {
            Color color = renderer.color;
            color.a -= 0.001f;
            renderer.color = color;

            Vector3 pos = splatTransform.localPosition;
            pos.y -= 0.001f;
            splatTransform.localPosition = pos;            


            yield return new WaitForSeconds(0.005f);
        }
    }

    void NewSplat()
    {
        GameObject newSplat = Instantiate<GameObject>(splat[Random.Range(0, 3)], HUD.transform);
        SpriteRenderer render = newSplat.GetComponent<SpriteRenderer>();
        Color color = render.color;
        color.a = 0.9f;
        color.r -= Random.Range(0, 0.3f);
        render.color = color;
        StartCoroutine(UpdateSplats(newSplat.transform, render));

        //newSplat.transform.parent = HUD.transform;
        newSplat.transform.Translate(new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0));
        newSplat.transform.Rotate(Vector3.forward, Random.Range(0, 360));
        
        //Vector3 pos = pointInWorld(new Vector2(Random.Range(0.42f, 0.58f), Random.Range(0.42f, 0.58f)));
        //newSplat.transform.position = new Vector3(pos.x, pos.y, HUD.transform.position.z);
        float s = Random.Range(0.05f, 0.35f);
        newSplat.transform.localScale = new Vector3(s, s, 1f);
    }

    private Vector3 pointInWorld(Vector2 point)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(point.x, point.y, HUD.transform.position.z));
    }
}
