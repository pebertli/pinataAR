using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandySpawnController : MonoBehaviour {

    public GameObject[] CandyPrefabList;

    private List<GameObject> CandyInstanceList = new List<GameObject>();

    public void SpawnCandy(int amount, Vector3 position)
    {                  
               
            for (int i = 0; i < amount; i++)
            {
                Vector3 pos = Random.insideUnitCircle * 0.77f;
                int r = Random.Range(0, 7);
                GameObject c = Instantiate<GameObject>(CandyPrefabList[r], position + pos, Quaternion.identity);
                CandyInstanceList.Add(c);
                c.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2))
                    , ForceMode.Impulse);
            }        
    }

    public void DestroyCandies()
    {        
            foreach (GameObject c in CandyInstanceList)
            {
                Destroy(c);
            }
        }
}
