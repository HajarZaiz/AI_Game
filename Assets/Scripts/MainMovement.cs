using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMovement : MonoBehaviour
{
    //Variables for movement
    private Vector3 direction;
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;

    //Variables to check whether the player is grounded
    private float distToGround;
    private bool isGrounded;

    //Variables to correctly orient the player
    Quaternion playerRot;
    [SerializeField] private float rotSpeed = 0.5f;

    //Variables for components
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform transform;
    [SerializeField] private GameObject hidePrompt;
    [SerializeField] private GameObject scream;
    //To check the chasing status
    [SerializeField] private GameObject enemy;
    //The two gate doors
    [SerializeField] private Transform gate1;
    [SerializeField] private Transform gate2;

    //Variables for scream
    [SerializeField] private Vector3 screamLocation;
    private bool screaming = false;

    //private Vector3 moveDirection = Vector3.zero;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpSpeed;
    private float velocityY;

    // Start is called before the first frame update
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        //Check if player is near gate and not being chased
        var distanceToGate1 = gate1.position - transform.position;
        var dist1 = Vector3.Distance(gate1.position, transform.position);
        var dot1 = Vector3.Dot(distanceToGate1, transform.forward);
        var distanceToGate2 = gate2.position - transform.position;
        var dist2 = Vector3.Distance(gate2.position, transform.position);
        var dot2 = Vector3.Dot(distanceToGate2, transform.forward);
        if ((dot1 > 0 || dot2 >0) && (dist2 < 3 || dist1 < 3) && !enemy.GetComponent<enemy1Movement>().GetChasingStatus())
        {
            SceneManager.LoadScene("Win Screen");
        }
    }

    //This function handles player Movement
    private void Move()
    {
        //Check if player is grounded 
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
       
        //Moving along the x and z axis
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        //Direction of movement
        direction = new Vector3(moveX, 0, moveZ);

        //Where to rotate player to look
        if(direction != Vector3.zero)
            playerRot = Quaternion.LookRotation(direction);

        //Different actions and animations depending on keys pressed
        //Arrow Keys Alone --> Normal Walk
        //Arrow Keys + Left Shift --> Run
        //Arrow Keys + c --> Slow walk while crouching
        //No Arrow Keys pressed --> Idle
        //Space Bar --> Jump
        if (direction != Vector3.zero && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.C))
        {
            Walk();
        }
        else if (direction != Vector3.zero && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.C))
        {
            Run();
        }
        else if (direction != Vector3.zero && !Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C))
        {
            Crouch();
        }
        else if (direction == Vector3.zero)
        {
            Idle();
        }
        //Jump if player is on the ground
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        else
        {
            animator.SetBool("isGrounded", true);
        }
        //Scream
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Scream();
        }
        //Slowly rotate player
        transform.rotation = playerRot;
        //Apply gravity
        velocityY -= gravity * Time.deltaTime;
        direction.y = velocityY;
        //Move
        controller.Move(direction * moveSpeed* Time.deltaTime);

    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
        animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }

    private void Run()
    {
        moveSpeed = runSpeed;
        animator.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
    }

    private void Crouch()
    {
        moveSpeed = crouchSpeed;
        animator.SetFloat("Speed", 1.5f, 0.1f, Time.deltaTime);
    }

    private void Idle()
    {
        animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }


    private void Jump()
    {
        Debug.Log(rb.velocity.y);
        Debug.Log(isGrounded);
        if (rb.velocity.y == 0 && isGrounded)
        {
            velocityY = jumpSpeed;
            //Add Jump animation
            animator.SetBool("isGrounded", false);
            animator.SetFloat("Speed", 3);
        }
        else
        {
            
        }
    }

    private void Scream()
    {
        Debug.Log("I just screamed");
        scream.SetActive(true);
        screamLocation = transform.position;
        screaming = true;
        Invoke("StopScreaming", 3.5f);
    }

    private void StopScreaming ()
    {
        screaming = false;
        scream.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "HidingSpot")
        {
            hidePrompt.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "HidingSpot")
        {
            hidePrompt.SetActive(false);
        }
    }

    public bool screamingStatus()
    {
        return screaming;
    }
}

