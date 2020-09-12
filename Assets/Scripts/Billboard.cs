using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera MainCam;

    // Start is called before the first frame update
    void Start()
    {
        MainCam = Camera.main;   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = MainCam.transform.rotation;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
