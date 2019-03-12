using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandySpawnController : MonoBehaviour {

    public GameObject[] CandyPrefabList;

    private List<GameObject> _candyInstanceList = new List<GameObject>();

    public void SpawnCandy(int amount, Vector3 position, float spread)
    {                  
               
            for (int i = 0; i < amount; i++)
            {
                //distribute candies over the floor
                Vector3 pos = Random.insideUnitCircle * spread;
                position.x += pos.x;
                position.z += pos.y;
                //choose a candy
                int r = Random.Range(0, 7);
                GameObject c = Instantiate<GameObject>(CandyPrefabList[r], position, Quaternion.identity);
                _candyInstanceList.Add(c);
                //add a force to simulate the inertia of the hit
                //c.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2)), ForceMode.Impulse);
            }        
    }

    public void DestroyCandies()
    {        
            foreach (GameObject c in _candyInstanceList)
            {
                Destroy(c);
            }
        }
}
