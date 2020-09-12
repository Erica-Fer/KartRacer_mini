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
    float Timer = 0f;
    
    float TurnSpeed = 180f;
    float SpinSpeed; // set on collision with obstacles that cause spinning out
    float SpinLength;

    bool IsTurning = false;
    bool IsSpinning = false;
    bool WallCollide = false;

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
        if (IsSpinning)
        {
            Spin();
        }
        else
        {
            Movement();
        }
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
            UnlockYRotation();
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
        
        //? bug where reversing gets a bit fucked with this installed
        //?so when you turn while going backward, you go opposite direction of what you wanted
        //float VelThreshold = .5f;
        ////? should magnitude be used instead??
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
    // ? TODO
    void Jump()
    {
        //If jump button held for X seconds, call Drift()
    }

    //Initiated by jump, ala mariokart
    //? does this need to be seperate?
    // ? TODO
    void Drift()
    {

    }

    void Spin()
    {
        //Quaternion deltaRotate = Quaternion.Euler(0f, 180f, 0f);
        //rb.MoveRotation(rb.rotation * deltaRotate);
        //rb.AddTorque(transform.up * SpinSpeed);

        UnlockYRotation();

        //? TODO: Implement spin for when collide w/ tirewall
        // Current implement makes it all wonky :/
        float Degrees = 540f;
        //if(WallCollide)
        //{
        //    Degrees *= -1;
        //}

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles
                                              + new Vector3(0f, 1 * Degrees * Time.deltaTime * (SpinLength - Timer)/SpinLength, 0f));

        Timer += Time.deltaTime;
        if(Timer >= SpinLength)
        {
            IsSpinning = false;
            Timer = 0;
        }
    }

    #endregion

    #region Collision Behavior

    //Handles all collision types
    //Also helps for scenario where kart is going so fast, it goes through colliders
    private void OnCollisionEnter(Collision collision)
    {
        GameObject Col = collision.gameObject;
        WallCollide = true;

        if (IsSpinning)
        {
            return;
        }

        // If player collides with a tirewall, push player back and spin them some
        // ? TODO: add spin when player hits wall, and make controlling kart more difficult
        if (Col.tag == "Tirewall")
        {
            Debug.Log("Impact!!!");
            rb.velocity = new Vector3(0f, 0f, 0f);

            var ReflectPosition = transform.position;

            Debug.Log("At impact: " + ReflectPosition);

            ReflectPosition = Vector3.Reflect(ReflectPosition, Vector3.right);
            Debug.Log("Reflected: " + ReflectPosition + "\n");

            rb.AddForce(-ReflectPosition*VelMagnitude*.01f, ForceMode.Impulse);
            
            //? TODO: finish implementing small spin when collide into wall
            //IsSpinning = true;          
            //SpinSpeed = 2;
            //SpinLength = .5f;
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        GameObject Col = collision.gameObject;

        //If collide with oilslick, spin out and lose speed
        // ? TODO
        // ? should a kart going a slow speed be able to drive over a slick, or not?
        if (Col.tag == "Oilslick")
        {
            IsSpinning = true;
            SpinSpeed = 5;
            // ? TODO add 4 second timer for spinning
            SpinLength = 2f;
        }
    }

    #endregion

    #region Misc

    void UnlockYRotation()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    #endregion
}

