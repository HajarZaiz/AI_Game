using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class enemy1Movement : MonoBehaviour
{
    //Variables for movement
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private bool wandering;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float satisfactionRadius;
    [SerializeField] private Vector2 randomPoint;
    [SerializeField] private Vector3 enemyPosition;
    [SerializeField] private Vector3 randomDestination;
    [SerializeField] private float distance;
    [SerializeField] private float timeToTarget;
    private bool chasing = false;
    private bool noise = false;
    private Vector3 noiseSource;
    [SerializeField] private Vector3 initialPosition;

    //Variables for components
    [SerializeField] private Rigidbody rb;
    [SerializeField] private UnityEngine.AI.NavMeshAgent nm;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform transform;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject sceneManager;

    //Variable for rotation
    Quaternion enemyRot;

    //Vision variables
    [SerializeField] private float visionAngle;
    [SerializeField] private float visionDistance;

    //Player Hiding variables
    [SerializeField] private GameObject[] coffins;
    private bool hidingStatus;
    private bool newDest = false;

    //Branch noises variables
    [SerializeField] private GameObject[] branches;
    private bool crunchStatus;

    void Start()
    {
        //Hiding in coffin info
        hidingStatus = false;
        coffins = GameObject.FindGameObjectsWithTag("HidingSpot");
        //Noisy branches info
        branches = GameObject.FindGameObjectsWithTag("Branch");
        crunchStatus = false;
        initialPosition = transform.position;
        //Select random destination for the enemy to wander towards within wandering radius
        enemyPosition = transform.position;
        do {
            randomPoint = Random.insideUnitCircle * wanderRadius;
            randomDestination = enemyPosition + new Vector3(randomPoint.x, 0, randomPoint.y);
        } while ((randomDestination.x < -49 || randomDestination.x > 25) && (randomDestination.z < -45 || randomDestination.z > 22));
        distance = Vector3.Distance(randomDestination, enemyPosition);

        //Rotate enemy towards destination
        transform.LookAt(randomDestination);

        //Set wander speed
        wanderSpeed = distance / timeToTarget;
        if (wanderSpeed > maxSpeed)
        {
            wanderSpeed = maxSpeed;
        }
    }

    void Update()
    {

        if (chasing)
        {
            randomDestination = new Vector3(player.position.x, 0, player.position.z);
        }
        //When the player is not within a satisfaction radius from the destination
        if(distance > satisfactionRadius)
        {
            //Move it towards the destination
            if (!chasing)
            {
                nm.speed = wanderSpeed;
                nm.SetDestination(randomDestination);
            }
            else {
                nm.speed = runSpeed;
                nm.SetDestination(randomDestination);
                //transform.position = Vector3.MoveTowards(transform.position, randomDestination, runSpeed * Time.deltaTime); 
            }
            animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
            enemyPosition = transform.position;
            distance = Vector3.Distance(randomDestination, enemyPosition);
            transform.LookAt(randomDestination);

        }
        else
        {
            //In case the reached destination is the player
            if (chasing)
            {
                //If player hid
                if (hidingStatus)
                {
                    chasing = false;
                    //Go back near gate
                    nm.speed = wanderSpeed;
                    nm.SetDestination(initialPosition);
                }
                else
                {
                    Debug.Log("CAUGHT YOU!");
                    SceneManager.LoadScene("Lose Screen");
                }

            }
            else
            {
                //Go back near gate and then wander again
                //Go back near gate
                enemyPosition = transform.position;
                randomDestination = initialPosition;
                distance = Vector3.Distance(randomDestination, enemyPosition);

                //Rotate enemy towards destination
                transform.LookAt(randomDestination);

                //Set wander speed
                wanderSpeed = distance / timeToTarget;
                if (wanderSpeed > maxSpeed)
                {
                    wanderSpeed = maxSpeed;
                }
                nm.speed = wanderSpeed;
                nm.SetDestination(randomDestination);

                if (distance <= satisfactionRadius)
                {
                    //Generate new destination
                    enemyPosition = transform.position;
                    do
                    {
                        randomPoint = Random.insideUnitCircle * wanderRadius;
                        randomDestination = enemyPosition + new Vector3(randomPoint.x, 0, randomPoint.y);
                    } while ((randomDestination.x < -49 || randomDestination.x > 25) && (randomDestination.z < -45 || randomDestination.z > 22));
                    distance = Vector3.Distance(randomDestination, enemyPosition);

                    //Rotate enemy towards destination
                    transform.LookAt(randomDestination);

                    //Set wander speed
                    wanderSpeed = distance / timeToTarget;
                }

            }
        }

        //2- Search for sound source if player makes any within hearing range (Just a sphere collider/sphere raycast)
        Collider[] noises = Physics.OverlapSphere(transform.position, 500000, 1 << 9);
        foreach (var noise in noises)
        {
            Debug.Log("I HEARD YOU " + noise.tag);
        }
        //In case of player scream
        var noiseStatus = player.GetComponent<MainMovement>().screamingStatus();
        if (noiseStatus)
        {
            randomDestination = player.position;
            transform.LookAt(randomDestination);
        }
        //In case of branch crunch
        branches = GameObject.FindGameObjectsWithTag("Branch");
        foreach(GameObject b in branches)
        {
            //Check if any of the branches are making noise
            crunchStatus = b.GetComponent<Noise>().GetCrunchStatus();
            //If there is crunch noise
            if (crunchStatus)
            {
                randomDestination = player.position;
                transform.LookAt(randomDestination);
            }
        }

        //3- Chase player if spotted
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);
        if (angle < visionAngle && Vector3.Distance(player.position, transform.position) < visionDistance) {
            chasing = true;
        }

        coffins = GameObject.FindGameObjectsWithTag("HidingSpot");
        foreach (GameObject coffin in coffins)
        {
            //If player hides at one of the coffins while far away enough from the enemy he stops chasing
            hidingStatus = coffin.GetComponent<Hiding>().GetHidenStatus();
            if (hidingStatus)
            {
                chasing = false;
                break;
            }
        }


    }

    public bool GetChasingStatus()
    {
        return chasing;
    }
  

}
