using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    float horizontalSpeed = 2.0f;
    float verticalSpeed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float Horizontal = horizontalSpeed * Input.GetAxis("Mouse X");
        float Vertical = verticalSpeed * Input.GetAxis("Mouse Y");
        transform.Rotate(Vertical, Horizontal, 0f);

        Quaternion Rotation = transform.rotation;
        transform.rotation = Quaternion.Euler(Rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
    }
}
