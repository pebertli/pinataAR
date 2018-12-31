using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {
    
    private Animator anim;
    private float attackCooldown = 0f;
    private float attackDelay = 0.2f;
    private bool attacking = false;    

    public Collider triggerAttack;
    


	// Use this for initialization
	void Start () {
        anim = this.GetComponentInChildren<Animator>();        
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Check if finger is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (GameController.Instance.Tool == GameController.PlayerTool.Bate)
        {
            if (attackCooldown > 0)
            {
                attackCooldown -= Time.deltaTime;

            }
            else
            {
                triggerAttack.enabled = false;
            }
            
                    if (Input.GetMouseButtonDown(0) && attackCooldown <= 0)
                    {
                        attacking = true;
                        attackCooldown = 0.75f;
                        int attackAnimation = Random.Range(1, 4);
                        anim.SetTrigger("attack" + attackAnimation);
                    }

           


            if (attacking)
            {
                if (attackDelay > 0)
                    attackDelay -= Time.deltaTime;
                else
                {
                    triggerAttack.enabled = true;
                    attackDelay = 0.2f;
                    attacking = false;
                }
            }
        }
        else if (GameController.Instance.Tool == GameController.PlayerTool.Hand)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 20.0f, 1 << 10 | 1 << 11 | 1 << 12))
                {
                    if (hit.transform.gameObject.CompareTag("Candy"))
                    {
                        CandyController c = hit.transform.gameObject.GetComponent<CandyController>();
                        GameController.Instance.addScore(c.points);
                        c.Destroy();
                    }
                    else if (hit.transform.gameObject.CompareTag("StarCandy"))
                        hit.transform.gameObject.GetComponent<StarCandyController>().Damage(1);
                    else if (hit.transform.gameObject.CompareTag("BadCandy"))
                        EventManager.TriggerEvent("Activate"+ hit.transform.gameObject.name);
                }
            }

        }
    }
}
