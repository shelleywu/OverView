// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
//using UnityEngine.UI;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the
    /// tap gesture again to place.
    /// This script is used in conjunction with GazeManager, GestureManager,
    /// and SpatialMappingManager.
    /// </summary>

    public partial class TapToPlace : MonoBehaviour
    {
        bool placing = false;
        GameObject targeted = null;
        
        public TextMesh debug;
        public TextMesh debug2;
        public TextMesh debug3;
        public TextMesh debug4;
        public GameObject defaultNote;
        //private GameObject created;
        private RaycastHit _hitInfo;
        private Quaternion _toQuat;
        private bool _gazeHit = false;

        void Start() {
            //debug.text = "WHOO!";
            SpatialMappingManager.Instance.DrawVisualMeshes = false;

        }

        // Called by GazeGestureManager when the user performs a tap gesture.
        public void OnSelect(GameObject focusedObject)
        {
            if (focusedObject == null)
            {
                Debug.Log("OnSelect: Null");
            }
            else
            {
                Debug.Log("OnSelect: " + focusedObject.name);
            }

            GameObject obj;
            
            //debug.text = focusedObject.name; //"OnSelect";
            if (SpatialMappingManager.Instance != null)
            {
                debug2.text = "GazeHit: " +  _gazeHit.ToString(); //"OnSelect
                if (focusedObject == null)
                {
                    debug3.text = "Null";
                    debug4.text = "Null";
                } else if (focusedObject != null) {
                    debug3.text = "ComTagNote: " + (focusedObject.CompareTag("Note")).ToString();
                    //debug4.text = "ComTagModel: " + (true).ToString();
                }

                //&& !focusedObject.CompareTag("Model")

                if (focusedObject != null) {
                    if (!focusedObject.CompareTag("Note"))
                    {
                        obj = Instantiate(defaultNote);
                        //debug2.text = _hitInfo.point.ToString();
                        obj.transform.position = _hitInfo.point;
                        obj.transform.rotation = _toQuat;
                    }
                    else if (!focusedObject.CompareTag("Model"))
                    {
                        //DO MENU OR WHATEVER
                    }
                }
            }

                
            else
            {
                Debug.Log("TapToPlace requires spatial mapping.  Try adding SpatialMapping prefab to project.");
            }
        }

        public void OnHoldStarted(GameObject focusedObject) {
            // On each tap gesture, toggle whether the user is in placing mode.
            if (focusedObject == null)
            {
                Debug.Log("OnHoldStarted: Null");
            }
            else
            {
                Debug.Log("OnHoldStarted: " + focusedObject.name);
            }

            debug.text = "OnHoldStarted";
            if (SpatialMappingManager.Instance != null) {
            
                if (focusedObject != null && focusedObject.CompareTag("Note")) {

                targeted = focusedObject;
                placing = true;

                // If the user is in placing mode, display the spatial mapping mesh.
                //SpatialMappingManager.Instance.DrawVisualMeshes = true;
                }
            }
            else
            {
                Debug.Log("TapToPlace requires spatial mapping.  Try adding SpatialMapping prefab to project.");
            }
        }

        public void OnHoldCompleted(GameObject focusedObject) {
            if (focusedObject == null) {
                Debug.Log("OnHoldComplete: Null");
            } else {
                Debug.Log("OnHoldComplete: " + focusedObject.name);
            }
            

            if (SpatialMappingManager.Instance != null && focusedObject != null)
            {
                targeted = null;
                debug.text = "OnHoldCanceled";
                placing = false;

                // If the user is not in placing mode, hide the spatial mapping mesh.
                //SpatialMappingManager.Instance.DrawVisualMeshes = false;
            }
            else
            {
                Debug.Log("TapToPlace requires spatial mapping.  Try adding SpatialMapping prefab to project.");
            }
        }

        public void OnHoldCanceled(GameObject focusedObject) {
            OnHoldCompleted(focusedObject);
        }

        // Update is called once per frame.
        void Update()
        {

            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            _gazeHit = Physics.Raycast(headPosition, gazeDirection, out _hitInfo,
                    30.0f, SpatialMappingManager.Instance.LayerMask);

            if (_gazeHit)
            {
                _toQuat = Camera.main.transform.localRotation;
                // Rotate this object to face the user.
                _toQuat.x = 0;
                _toQuat.z = 0;
            }            

            // If the user is in placing mode,
            // update the placement to match the user's gaze.
            if (placing)
            {
                // Move this object to where the raycast
                // hit the Spatial Mapping mesh.
                // Here is where you might consider adding intelligence
                // to how the object is placed.  For example, consider
                // placing based on the bottom of the object's
                // collider so it sits properly on surfaces.
                targeted.transform.position = _hitInfo.point;
                targeted.transform.rotation = _toQuat;
            }
        }
    }
}
