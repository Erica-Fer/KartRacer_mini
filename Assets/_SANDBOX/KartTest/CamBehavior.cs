using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBehavior : MonoBehaviour
{
    GameObject PlayerKart;
    // Start is called before the first frame update
    void Start()
    {
        PlayerKart = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position = PlayerKart.transform.position + new Vector3(0f, 1f, -4f);
        //transform.position = PlayerKart.transform.position;
        //transform.rotation = PlayerKart.transform.rotation;
        //transform.position += new Vector3(0f, 1f, -4);

    }
}
