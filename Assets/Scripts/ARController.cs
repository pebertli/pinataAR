//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class ARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a feature point.
        /// </summary>
        public GameObject AndyPointPrefab;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        public GameObject pinata;
        public GameObject plane;

        /// <summary>
        /// The rotation in degrees need to apply to model when the Andy model is placed.
        /// </summary>
        private const float k_ModelRotation = 180.0f;

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        private DetectedPlane ARPlane;
        private Anchor ARAnchor;

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {          
            _UpdateApplicationLifecycle();

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<DetectedPlane>(m_AllPlanes);            
            bool showSearchingUI = true;
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                //detected a floor with minimun size
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking
                    && m_AllPlanes[i].PlaneType == DetectedPlaneType.HorizontalUpwardFacing
                    && (m_AllPlanes[i].ExtentX > 2f || m_AllPlanes[i].ExtentZ > 2f)
                    )
                {
                    //if there was no plane detected before
                    if (ARPlane == null)
                        ARPlane = m_AllPlanes[i];
                 
                    showSearchingUI = false;
                    break;
                }
            }

            //hide snackbar
            SearchingForPlaneUI.SetActive(showSearchingUI);
            //found a plane and was not playing yet
            if (!showSearchingUI && GameController.Instance.State == GameController.GameState.SearchingFloor)
            {
                //Playing
                GameController.Instance.State = GameController.GameState.Started;
                //hide AR plane visual helpers
                DetectedPlanePrefab.SetActive(false);
                //get all anchors in the detected floor
                List<Anchor> anchorList = new List<Anchor>();
                ARPlane.GetAllAnchors(anchorList);
                //if there is no pinata/anchor instanced yet
                if (ARPlane != null && anchorList.Count == 0)
                {
                    //instancing pinata and invisible floor
                    GameObject prefabPinata = Instantiate<GameObject>(pinata, ARPlane.CenterPose.position + 
                        new Vector3(0, FirstPersonCamera.transform.position.y+2f, 0), Quaternion.identity);
                    GameObject prefabPlane = Instantiate<GameObject>(plane, ARPlane.CenterPose.position, Quaternion.identity);

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
            if (showSearchingUI && 
                (GameController.Instance.State == GameController.GameState.Started 
                || GameController.Instance.State == GameController.GameState.Broken))
            {
                //not playing
                GameController.Instance.State = GameController.GameState.SearchingFloor;
                //show plane visual helper
                DetectedPlanePrefab.SetActive(true);
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

                

            }

        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
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

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
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
