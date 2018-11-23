    using System.Collections.Generic;    
    using UnityEngine;


    public class ARControllerDummy : MonoBehaviour
    {

        public Camera FirstPersonCamera;

        public GameObject pinata;
        public GameObject plane;

        private GameObject prefabPlane;

    private bool showSearchingUI = true;


    public void Update()
        {

        if (Input.GetKeyDown("g"))
        {
            showSearchingUI = false;
        }
        else if (Input.GetKeyDown("h"))
        {
            showSearchingUI = true;
        }

        if (!showSearchingUI && GameController.Instance.State == GameController.GameState.SearchingFloor)
            {

                GameController.Instance.State = GameController.GameState.Started;
                GameObject prefabPinata = Instantiate<GameObject>(pinata, new Vector3(0, 2.8f, 0),
                    Quaternion.identity);
                prefabPlane = Instantiate<GameObject>(plane, Vector3.zero, Quaternion.identity);
                Pose pose;
                pose.position = new Vector3(0, 0f, 0);
                pose.rotation = Quaternion.identity;

                GameController.Instance.pinata = prefabPinata.GetComponent<PinataController>();

                
                prefabPinata.transform.parent = prefabPlane.transform;
                //prefabPlane.transform.parent = anchor.transform;
            }

            if (showSearchingUI && GameController.Instance.State == GameController.GameState.Started)
            {
                GameController.Instance.State = GameController.GameState.SearchingFloor;
            Destroy(prefabPlane);
            }

        }
       
    }

