namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    public class ARController : MonoBehaviour
    {

        public Camera FirstPersonCamera;
        public GameObject DetectedPlanePrefab;
        public GameObject AndyPlanePrefab;
        public GameObject AndyPointPrefab;

        //snackBar parent
        public GameObject SearchingForPlaneUI;

        private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();
        private bool m_IsQuitting = false;


        private bool hasPlaneDetected = false;
        private DetectedPlane ARPlane;
        private Anchor ARAnchor;
        public GameObject Pinata;
        public GameObject Plane;




        //debug
        //private bool searching = true;

        //private void Simulate(int i)
        //{
        //    switch (i)
        //    {
        //        case 1:
        //            searching = false;
        //            //GameController.Instance.State = 
        //            break;
        //        case 3:
        //            searching = true;
        //            GameController.Instance.State = GameController.GameState.Broken;
        //            break;
        //    }
        //}

        public void Update()
        {          
            _UpdateApplicationLifecycle();

            //if (Input.GetKeyDown("1"))
            //    Simulate(1);
            //if (Input.GetKeyDown("3"))
            //    Simulate(3);

            //Get all planes deteted by ARCore engine
            Session.GetTrackables<DetectedPlane>(m_AllPlanes);
            //debug
            //bool showSearchingUI = true && searching;

            //iterate over each plane searching for a suitable plane (status, size and orientation)
            hasPlaneDetected = false;
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                //detected a floor with minimum size
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking
                    && m_AllPlanes[i].PlaneType == DetectedPlaneType.HorizontalUpwardFacing
                    && (m_AllPlanes[i].ExtentX > 1f || m_AllPlanes[i].ExtentZ > 1f)
                    )
                {
                    //if there was no plane detected before
                    //else use the previous detection
                    //if (ARPlane == null)
                        ARPlane = m_AllPlanes[i];

                    //a plane was detected, so there is no need to itereate more
                    hasPlaneDetected = true;
                    break;
                }
                //else
                //{

                //    if (ARPlane != null)
                //    {
                //        List<Anchor> anchorList = new List<Anchor>();
                //        ARPlane.GetAllAnchors(anchorList);
                //        foreach (Anchor anchor in anchorList)
                //            Destroy(anchor);
                //    }
                //    if(ARAnchor != null)
                //        DestroyImmediate(ARAnchor.gameObject);
                //        ARAnchor = null;
                //        ARPlane = null;
                    
                //}
            }
            //debug
            //showSearchingUI = true && searching;            

            //hide snackbar
            SearchingForPlaneUI.SetActive(!hasPlaneDetected);

            //found a plane and was not playing yet
            if (hasPlaneDetected && GameController.Instance.State == GameController.GameState.SearchingFloor)
            {
                //Playing
                GameController.Instance.State = GameController.GameState.Started;
                //hide AR plane visual helpers
                DetectedPlanePrefab.SetActive(false);
                //get all anchors in the detected floor
                List<Anchor> anchorList = new List<Anchor>();
                //if(ARPlane!=null) //debug
                ARPlane.GetAllAnchors(anchorList);
                //if there is no pinata/anchor instanced yet
                //if (ARPlane != null /*&& anchorList.Count == 0*/)
                //if (anchorList.Count == 0)//debug
                {

                    var lookPos = transform.position - FirstPersonCamera.transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);

                    //instancing pinata and invisible floor
                    GameObject prefabPinata = Instantiate<GameObject>(Pinata, ARPlane.CenterPose.position +
                        new Vector3(0, FirstPersonCamera.transform.position.y + 2f, 0), rotation);
                    GameObject prefabPlane = Instantiate<GameObject>(Plane, ARPlane.CenterPose.position, Quaternion.identity);

                    //debug
                    //GameObject prefabPinata = Instantiate<GameObject>(Pinata, new Vector3(0, 2, 2), Quaternion.identity);
                    //GameObject prefabPlane = Instantiate<GameObject>(Plane, Vector3.zero, Quaternion.identity);

                    //set an anchor post
                    Pose pose;
                    pose.position = ARPlane.CenterPose.position;
                    pose.rotation = Quaternion.identity;
                    //make game controller aware of this new pinata
                    //TODO game controller should create this
                    GameController.Instance.pinata = prefabPinata.GetComponentInChildren<PinataController>();
                    //create anchor and attach pinata and floor
                    ARAnchor = ARPlane.CreateAnchor(pose);
                    prefabPinata.transform.parent = ARAnchor.transform;
                    prefabPlane.transform.parent = ARAnchor.transform;
                }
            }
            //if lost the detected plane and was playing
            if (!hasPlaneDetected && 
                (GameController.Instance.State == GameController.GameState.Started 
                || GameController.Instance.State == GameController.GameState.Broken))
            {

              
                //reset detected plane, but the same plane can be detected again                
                GameController.Instance.RestartPinata();               
                //get all anchors in the detected floor
                if(ARPlane != null)
                {
                    //List<Anchor> anchorList = new List<Anchor>();
                    //ARPlane.GetAllAnchors(anchorList);
                    //foreach (Anchor anchor in anchorList)
                    //        Destroy(anchor);
                    
                    DestroyImmediate(ARAnchor.gameObject);
                    ARAnchor = null;
                ARPlane = null;
                }

                //not playing
                GameController.Instance.State = GameController.GameState.SearchingFloor;
                //show plane visual helper
                DetectedPlanePrefab.SetActive(true);




            }

        }

        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            //if(Input.GetKeyDown("c"))
            //{
            //    hasPlaneDetected = true;
            //    GameController.Instance.State = GameController.GameState.SearchingFloor;
            //}

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        private void _DoQuit()
        {
            Application.Quit();
        }

        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
