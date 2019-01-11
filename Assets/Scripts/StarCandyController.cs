using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCandyController : MonoBehaviour {

    public GameObject player;
    public GameObject floor;
    public GameObject[] candy;    
    public int maxPick = 3;

    Vector3 nextPosition;
    bool moving = false;
    float animationCooldown = 1.5f;
    Animator animator;
	// Use this for initialization
	void Start () {
        animator = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        if (animationCooldown > 0)
            animationCooldown -= Time.deltaTime;
        else
        {
            animationCooldown = 1.5f;
            int animRandom = Random.Range(0, 2);
            if (animRandom == 0)
                animator.SetTrigger("spin");
            else
                animator.SetTrigger("lala");

        }

        if (!moving)
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) < 3)
            {
                Vector3 dir = new Vector3(player.transform.forward.normalized.x*Random.Range(-1,1),0, player.transform.forward.normalized.z * Random.Range(-1, 1)) ;
                //float y = 
                nextPosition = (this.transform.position + (dir.normalized*3));               
                //nextPosition.y = floor.transform.position.y;
                moving = true;

            }
        }
        else
        {
            //run
            this.transform.position = Vector3.Lerp(this.transform.position, nextPosition, 3f * Time.deltaTime);
            if (Vector3.Distance(nextPosition, this.transform.position) <= 0.01f)
                moving = false;
        }

        Quaternion lookRotation =
     Quaternion.LookRotation((player.transform.position - transform.position).normalized);
        
        //over time
        transform.rotation =
            Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4f);
    }

    public void Damage(int power)
    {
        maxPick -= power;

        GameController.Instance.SpawnCandy(10, this.transform.position, 0.1f);

        if (maxPick <= 0)
            Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }   
}
