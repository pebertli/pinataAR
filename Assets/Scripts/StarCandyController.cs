using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCandyController : MonoBehaviour {

    public GameObject PlayerInstance;
    public GameObject FloorInstance;
    public int MaxPick = 3;

    Vector3 _nextPosition;
    bool _moving = false;
    float _animationCooldown = 1.5f;
    Animator _animator;
	// Use this for initialization
	void Start () {
        _animator = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        if (_animationCooldown > 0)
            _animationCooldown -= Time.deltaTime;
        else
        {
            _animationCooldown = 1.5f;
            int animRandom = Random.Range(0, 2);
            if (animRandom == 0)
                _animator.SetTrigger("spin");
            else
                _animator.SetTrigger("lala");

        }

        if (!_moving)
        {
            if (Vector3.Distance(PlayerInstance.transform.position, this.transform.position) < 3)
            {
                Vector3 dir = new Vector3(PlayerInstance.transform.forward.normalized.x*Random.Range(-1,1),0, PlayerInstance.transform.forward.normalized.z * Random.Range(-1, 1)) ;
                //float y = 
                _nextPosition = (this.transform.position + (dir.normalized*3));               
                //nextPosition.y = floor.transform.position.y;
                _moving = true;

            }
        }
        else
        {
            //run
            this.transform.position = Vector3.Lerp(this.transform.position, _nextPosition, 3f * Time.deltaTime);
            if (Vector3.Distance(_nextPosition, this.transform.position) <= 0.01f)
                _moving = false;
        }

        Quaternion lookRotation =
     Quaternion.LookRotation((PlayerInstance.transform.position - transform.position).normalized);
        
        //over time
        transform.rotation =
            Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4f);
    }

    public void Damage(int power)
    {
        MaxPick -= power;

        GameController.Instance.SpawnCandy(10, this.transform.position, 0.1f);

        if (MaxPick <= 0)
            Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }   
}
