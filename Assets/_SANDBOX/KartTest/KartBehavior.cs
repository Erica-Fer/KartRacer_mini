using UnityEngine;

/*
CONTROL SCHEME

WASD
W - forward
A - left
D - right
S - backward

SPACE - jump
SPACE held - drift
 */

public class KartBehavior : MonoBehaviour
{
    Rigidbody rb;
    float TurnSpeed = 180f;
    bool IsTurning = false;

    float VelMagnitude;

    [Header("Vertical Movement")]
    public float PlayerSpeed = 3f;

    [Header("Horizontal Movement")]
    public float Handling = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        VelMagnitude = rb.velocity.magnitude;
        Movement();
    }

    #region Physics Movements
    //Based on button input, this function calls the below methods for movement
    //If no input, gradually slows down to a stop
    void Movement()
    {
        float HorizontalInput = Input.GetAxis("Horizontal");
        
        IsTurning = false;
        //?float VerticalInput = Input.GetKey("Vertical");

        //If input left/right, call Turn()
        if (Input.GetKey("w"))
        {
            GoForward();
        }
        else if (Input.GetKey("s"))
        {
            GoBackward();
        }

        if (HorizontalInput != 0)
        {
            IsTurning = true;
            Turn(HorizontalInput);
        }

        //Prevent character wobbling when going forward
        if (IsTurning == false)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }


        //else if (Input.GetAxis("space") < 0)
        //{

        //}

        //If input forward, call GoForward()
        //If input backward, call GoBackward()
        //If input jump button, call Jump()
    }

    //Turn player left or right when A or D pressed
    //Turn character when moving forward, or moving backwards
    void Turn(float Val)
    {
        //SOURCE
        //https://www.youtube.com/watch?v=cqATTzJmFDY
        float ForwardMove = Input.GetAxis("Vertical");
        float VelThreshold = .5f;

        //? should magnitude be used instead??
        //if (ForwardMove < .3 && (rb.velocity.z > VelThreshold || rb.velocity.x > VelThreshold || rb.velocity.y > VelThreshold))
        //{
        //    ForwardMove = .3f;
        //}


        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles
                                              + new Vector3(0f, Val * (TurnSpeed/Handling) * Time.deltaTime * ForwardMove, 0f));
        
        
        //Debug.Log(Val);
        //Debug.Log(transform.up);

        ////If kart going backwards, do not add force
        ////Otherwise, add force so that the kart can turn even when stopped
        //if (Input.GetAxis("Vertical") >= 0)
        //{
        //    rb.AddForce(transform.forward);
        //}

        //rb.AddTorque(transform.up * Handling * Val);
    }

    //If stopped, will begin moving character forward
    //Forward movement will accel at X rate (x to be determined later)
    //If moving backward, will slow character to a stop
    void GoForward()
    {
        rb.AddForce(transform.forward * PlayerSpeed);
        //transform.position = transform.position + transform.forward;
    }

    //If moving forward, Slow the player down to a stop
    //If stopped, will begin going backwards
    //Max speed = X (X being a value much slower than forward movement)
    void GoBackward()
    {
        rb.AddForce(transform.forward * -(PlayerSpeed * .5f));
        //transform.position = transform.position - transform.forward;
    }

    //When button pressed, kart jumps
    //When button pressed while turning, turn is a bit sharper
    //If jump is held, drift initiated
    void Jump()
    {
        //If jump button held for X seconds, call Drift()
    }

    //Initiated by jump, ala mariokart
    //? does this need to be seperate?
    void Drift()
    {

    }
    #endregion

    #region Collision Behavior

    private void OnCollisionEnter(Collision collision)
    {
        GameObject Col = collision.gameObject;

        if (Col.tag == "Tirewall")
        {
            Debug.Log("Impact!!!");
            rb.velocity = new Vector3(0f, 0f, 0f);

            var ReflectPosition = transform.position;

            Debug.Log("At impact: " + ReflectPosition);

            ReflectPosition = Vector3.Reflect(ReflectPosition, Vector3.right);
            Debug.Log("Reflected: " + ReflectPosition + "\n");

            rb.AddForce(-ReflectPosition*VelMagnitude*.01f, ForceMode.Impulse);
        }
    }

    #endregion
}

