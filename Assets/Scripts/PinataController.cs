using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinataController : MonoBehaviour
{

    public GameObject goodPinata;
    public GameObject brokenPinata;

    public GameObject particleHit;

    Rigidbody rigidBody;

    private int health = 10;
    private GameObject[] instanceBrokenPinata;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    //private void Update()
    //{
    //    //debug
    //    if (Input.GetKeyDown("2"))
    //    {
    //        health = 0;
    //        Die();
    //    }
    //}

    private void Die()
    {
        GameController.Instance.State = GameController.GameState.Broken;
        Transform t = this.transform.GetChild(0).transform;
        GameObject broken = Instantiate<GameObject>(brokenPinata, t.position, t.rotation, this.transform);

        int i = broken.transform.childCount - 1;
        instanceBrokenPinata = new GameObject[i + 1];
        while (i >= 0)
        {
            instanceBrokenPinata[i] = broken.transform.GetChild(i).gameObject;
            instanceBrokenPinata[i].transform.parent = null;
            i--;
        }

        Destroy(t.gameObject);
        //other.isTrigger = false;
        GameController.Instance.SpawnCandy(20, transform.position, 0.66f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bate"))
        {
            //Debug.Log(other.gameObject.transform.forward);
            rigidBody.AddForce(other.transform.right * 10f, ForceMode.Impulse);
            if (health <= 0)
                return;
            health -= 25;
            GameObject particle = Instantiate<GameObject>(particleHit, other.ClosestPointOnBounds(transform.position), transform.rotation);
            if (health <= 0)
            {
                Die();
            }
        }
    }


    public void Restart()
    {

        if (health <= 0)
        {
            Transform t = this.transform.GetChild(0).transform;
            GameObject goo = Instantiate<GameObject>(goodPinata, t.position, t.rotation, this.transform);
            Destroy(t.gameObject);
            foreach (GameObject g in instanceBrokenPinata)
                Destroy(g);
        }


        health = 10;
    }
}
