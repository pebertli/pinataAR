using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinataController : MonoBehaviour
{

    //prefabs
    public GameObject WholePinataPrefab;
    public GameObject BrokenPinataPrefab;
    public GameObject HitParticlePrefab;

    public AudioClip audioCLipHit;
    public AudioClip audioClipDie;

    private Rigidbody _rigidBody;
    private AudioSource _audioHit;
    private int _health = ConstantHelper.HEALTH_PINATA;
    private GameObject[] _brokenPinataInstance;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _audioHit = GetComponent<AudioSource>();
    }

    private void Die()
    {
        GameController.Instance.State = GameController.GameState.Broken;
        Transform t = this.transform.GetChild(0).transform;
        GameObject broken = Instantiate<GameObject>(BrokenPinataPrefab, t.position, t.rotation, this.transform);

        //instantiate every single parte of the broken pinata separately
        int i = broken.transform.childCount - 1;
        _brokenPinataInstance = new GameObject[i + 1];
        while (i >= 0)
        {
            _brokenPinataInstance[i] = broken.transform.GetChild(i).gameObject;
            _brokenPinataInstance[i].transform.parent = null;
            i--;
        }

        _audioHit.clip = audioClipDie;
        _audioHit.pitch = Random.Range(0f, 3f);
        _audioHit.Play();
        //remove the original/unharmed pinata
        Destroy(t.gameObject);        
        //instantiate candies
        GameController.Instance.SpawnCandy(20, transform.position, ConstantHelper.CANDY_SPREAD_POSITION_RANGE);
        GameController.Instance.SpawnStarCandy(ConstantHelper.CANDY_SPREAD_POSITION_RANGE);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bate"))
        {            
            //hit and make damage to pinata
            _rigidBody.AddForce(other.transform.right * 10f, ForceMode.Impulse);
            if (_health <= 0)
                return;
            _health -= ConstantHelper.HIT_DAMAGE;

            _audioHit.clip = audioCLipHit;
            _audioHit.pitch = Random.Range(0f, 3f);
            _audioHit.Play();
            Instantiate<GameObject>(HitParticlePrefab, other.ClosestPointOnBounds(transform.position), transform.rotation);
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
            Instantiate<GameObject>(WholePinataPrefab, t.position, t.rotation, this.transform);
            Destroy(t.gameObject);

            foreach (GameObject g in _brokenPinataInstance)
                Destroy(g);
        }

        _health = ConstantHelper.HEALTH_PINATA;
    }
}
