using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class Billboard : MonoBehaviour
    {
        Transform cam;

        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main.transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.LookAt(transform.position + cam.forward);
        }

        private void Update()
        {
         if(CameraControl.splitMode)
            {
                cam = transform.parent.GetComponentInChildren<Camera>().transform;
                
            }else
            {
                cam = Camera.main.transform;
            }
        }
    }
}