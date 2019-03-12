using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinataController : MonoBehaviour
{

    public GameObject GoodPinata;
    public GameObject BrokenPinata;
    public GameObject ParticleHit;

    private Rigidbody _rigidBody;
    private int _health = 10;
    private GameObject[] _instanceBrokenPinata;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Die()
    {
        GameController.Instance.State = GameController.GameState.Broken;
        Transform t = this.transform.GetChild(0).transform;
        GameObject broken = Instantiate<GameObject>(BrokenPinata, t.position, t.rotation, this.transform);

        //instantiate every single parte of the broken pinata separately
        int i = broken.transform.childCount - 1;
        _instanceBrokenPinata = new GameObject[i + 1];
        while (i >= 0)
        {
            _instanceBrokenPinata[i] = broken.transform.GetChild(i).gameObject;
            _instanceBrokenPinata[i].transform.parent = null;
            i--;
        }

        //remove the original/unharmed pinata
        Destroy(t.gameObject);        
        //instantiate candies
        GameController.Instance.SpawnCandy(20, transform.position, 0.66f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bate"))
        {            
            //hit and make damage to pinata
            _rigidBody.AddForce(other.transform.right * 10f, ForceMode.Impulse);
            if (_health <= 0)
                return;
            _health -= 25;

            GameObject particle = Instantiate<GameObject>(ParticleHit, other.ClosestPointOnBounds(transform.position), transform.rotation);
            if (_health <= 0)
            {
                Die();
            }
        }
    }


    public void Restart()
    {
        //if has died, clean broken parts and restart the good pinata
        if (_health <= 0)
        {
            Transform t = this.transform.GetChild(0).transform;
            GameObject goo = Instantiate<GameObject>(GoodPinata, t.position, t.rotation, this.transform);
            Destroy(t.gameObject);

            foreach (GameObject g in _instanceBrokenPinata)
                Destroy(g);
        }


        _health = 10;
    }
}
