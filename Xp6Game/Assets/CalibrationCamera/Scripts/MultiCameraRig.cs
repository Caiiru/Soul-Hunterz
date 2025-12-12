using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Calibration
{
    [System.Serializable]
    public class ICPCamera
    {
        public string name;
        public int display;
        public float fov;
        public float rx;
        public float ry;
        public float rz;
    }

    [System.Serializable]
    public class ICPCameras
    {
        public ICPCamera[] items;
    }


    public class MultiCameraRig : MonoBehaviour
    {

        public string FileName = "Calibration/cameras.json";


        void Start()
        {
            string filePath = "";

            Debug.Log("--- MultiCameraRig::Start TotalDisplays=" + Display.displays.Length);

            if (Application.isEditor)
            {
                filePath = Application.dataPath + "/" + FileName;
                Debug.Log("--- MultiCameraRig::Start[Editor] filePath=[" + filePath + "]");
            }
            else
            {
                //When not running from inside the Editor, look for the file starting from the exe folder
                filePath = Application.dataPath + "/../" + FileName;
                Debug.Log("--- MultiCameraRig::Start[Exe] filePath=[" + filePath + "]");
            }

            //If the file does nto exists
            if (File.Exists(filePath))
            {
                // Read the json from the file into a string
                string dataAsJson = File.ReadAllText(filePath);

                Debug.Log("--- MultiCameraRig::Start JSON=(" + dataAsJson + ")");

                ICPCameras cameras = JsonUtility.FromJson<ICPCameras>(dataAsJson);

                if (cameras.items.Length > 0)
                {

                    Camera origCamera = gameObject.GetComponent<Camera>();

                    //Disable the original camera because new cameras will be created
                    origCamera.enabled = false;

                    for (int c = 0; c < cameras.items.Length; c++)
                    {
                        string name = cameras.items[c].name;
                        int display = cameras.items[c].display;
                        int displayIndex = display - 1;
                        float fov = cameras.items[c].fov;
                        float rx = cameras.items[c].rx;
                        float ry = cameras.items[c].ry;
                        float rz = cameras.items[c].rz;

                        Debug.Log("--- MultiCameraRig::Start Creating camera=N:" + name + " D:" + display + " F:" + fov + " RX:" + rx + " RY:" + ry + " RZ:" + rz);

                        //Creata rhe new camera position and rotation vector
                        Vector3 pos = new Vector3(0, 0, 0);
                        Vector3 rot = new Vector3(rx, ry, rz);

                        //Create new camera
                        GameObject newCamera = new GameObject(name);

                        //Add the Camera component and enable the camera
                        Camera cam = newCamera.AddComponent<Camera>();
                        //Copy all the parameters from the original camera
                        cam.CopyFrom(origCamera);
                        //Enable the camera
                        cam.enabled = true;

                        //make the camera chile of this object
                        newCamera.transform.parent = this.gameObject.transform;

                        //Setup the camera
                        cam.targetDisplay = displayIndex;
                        if (displayIndex < Display.displays.Length)
                        {
                            Display.displays[displayIndex].Activate();
                        }
                        else
                        {
                            //In Editor only one display is reported
                            //If not in editor more, report error
                            if (!Application.isEditor)
                            {
                                Debug.Log("--- MultiCameraRig::Start ERROR Camera Display " + display + " does not exist");
                            }
                        }

                        cam.fieldOfView = fov;
                        newCamera.transform.localPosition = pos;
                        newCamera.transform.localEulerAngles = rot;

                    }

                }

            }
            else
            {
                Debug.Log("--- MultiCameraRig::Start ERROR Can not find file=[" + filePath + "]");
            }
        }





        public void SetParent(GameObject camera, GameObject newParent)
        {
            Debug.Log("111 SetParent: newParent name: " + newParent.name);
            //Makes the GameObject "newParent" the parent of the GameObject "player".
            camera.transform.parent = newParent.transform;

            //Display the parent's name in the console.
            Debug.Log("222 SetParent: Camera's Parent: " + camera.transform.parent.name);

            // Check if the new parent has a parent GameObject.
            if (newParent.transform.parent != null)
            {
                //Display the name of the grand parent of the player.
                Debug.Log("333 Camera's Grand parent: " + camera.transform.parent.parent.name);
            }
        }
    }
}