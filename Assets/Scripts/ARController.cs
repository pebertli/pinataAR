namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    public class ARController : MonoBehaviour
    {


        public Camera FirstPersonCamera;
        public GameObject DetectedPlaneController;
        //snackBar parent
        public GameObject SearchingForPlaneUI;
        public GameObject Pinata;
        public GameObject Plane;

        private const float _sizePlan = 1.0f;
        private List<DetectedPlane> _allPlanes = new List<DetectedPlane>();
        private bool _isQuitting = false;
        private bool _hasPlaneDetected = false;
        private DetectedPlane _arPlane;
        private Anchor _arAnchor;


        public void Update()
        {
            _UpdateApplicationLifecycle();

            if (GameController.Instance.State == GameController.GameState.EntryMenu)
                return;

            //Get all planes deteted by ARCore engine
            Session.GetTrackables<DetectedPlane>(_allPlanes);

            //iterate over each plane searching for a suitable plane (status, size and orientation)
            _hasPlaneDetected = false;
            for (int i = 0; i < _allPlanes.Count; i++)
            {
                //detected a floor with minimum size
                if (_allPlanes[i].TrackingState == TrackingState.Tracking
                    && _allPlanes[i].PlaneType == DetectedPlaneType.HorizontalUpwardFacing
                    && (_allPlanes[i].ExtentX > _sizePlan || _allPlanes[i].ExtentZ > _sizePlan))
                {
                    //if there was no plane detected before
                    //else use the previous detection
                    //if (ARPlane == null)
                    _arPlane = _allPlanes[i];

                    //a plane was detected, so there is no need to iterate more
                    _hasPlaneDetected = true;
                    break;
                }
            }
            //hide snackbar
            SearchingForPlaneUI.SetActive(!_hasPlaneDetected);

            //found a plane and was not playing yet
            if (_hasPlaneDetected && GameController.Instance.State == GameController.GameState.SearchingFloor)
            {
                //Playing
                GameController.Instance.State = GameController.GameState.Started;
                //hide AR plane visual helpers
                DetectedPlaneController.SetActive(false);
                //get all anchors in the detected floor
                List<Anchor> anchorList = new List<Anchor>();
                _arPlane.GetAllAnchors(anchorList);
                
                //make pinata look to camera
                var lookPos = transform.position - FirstPersonCamera.transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);

                //instancing pinata and invisible floor
                //how high is the pinata?
                Vector3 yOffset = new Vector3(0, FirstPersonCamera.transform.position.y + 2f, 0);
                GameObject prefabPinata = Instantiate<GameObject>(Pinata, _arPlane.CenterPose.position + yOffset, rotation);
                GameObject prefabPlane = Instantiate<GameObject>(Plane, _arPlane.CenterPose.position, Quaternion.identity);
           
                //set an anchor post
                Pose pose;
                pose.position = _arPlane.CenterPose.position;
                pose.rotation = Quaternion.identity;
                //make game controller aware of this new pinata
                //TODO game controller should create this
                GameController.Instance.Pinata = prefabPinata.GetComponentInChildren<PinataController>();
                //create anchor and attach pinata and floor
                _arAnchor = _arPlane.CreateAnchor(pose);
                prefabPinata.transform.parent = _arAnchor.transform;
                prefabPlane.transform.parent = _arAnchor.transform;

            }
            //if lost the detected plane and was playing
            if (!_hasPlaneDetected &&
                (GameController.Instance.State == GameController.GameState.Started
                || GameController.Instance.State == GameController.GameState.Broken))
            {
                //reset detected plane, but the same plane can be detected again              
                GameController.Instance.RestartPinata();
                //get all anchors in the detected floor
                if (_arPlane != null)
                {
                    DestroyImmediate(_arAnchor.gameObject);
                    _arAnchor = null;
                    _arPlane = null;
                }

                //not playing
                GameController.Instance.State = GameController.GameState.SearchingFloor;
                //show plane visual helper
                DetectedPlaneController.SetActive(true);
            }

        }

        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

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

            if (_isQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                _isQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                _isQuitting = true;
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
