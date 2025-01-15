using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using static UnityEngine.GraphicsBuffer;

public class Heavy_Tank_Controller : EnemyController
{

    [Header("Player Detectino Raycast")]
    [SerializeField] private Transform RaycastPosition;
    [SerializeField] private float Range;
    [SerializeField] private LayerMask mask;

    [Header("Move Sensors")]
    [SerializeField] private Transform rightSensor;
    [SerializeField] private Transform leftSensor;

    private Vector3 thisPos; //pozycja 2d tego
    private Vector3 playerPosition; //pozycja 2d gracza

    [SerializeField] private Transform desiredPoint;
    private Vector3 desiredPosition; //pozycja 2d docelowa

    [SerializeField] private float distanceToPosition;
    [SerializeField] private float distanceToPlayer;

    [SerializeField] private Vector3 directionToTarget;
    [SerializeField] private Quaternion targetRotation;
    [SerializeField] private float angleToPosition;


    private bool isFacingTarget = false;


    [Header("State")]
    [SerializeField] private EnemyState state;
    [SerializeField] private PlayerDetectionState playerDetected;

    [SerializeField] private float losingTime;

    [SerializeField] private float CloseDistance;
    
    Coroutine detectionCoroutine;


    private void Awake()
    {
        turretController = GetComponent<EnemyTurretController>();
    }


    // Start is called before the first frame update
    void Start()
    {
        player  = ProtoTankController.instance;
        enemyManager = EnemyManager.instance;
        enemyManager.AddEnemy(this);

        if (desiredPoint != null)
        {
            desiredPosition = new Vector3(desiredPoint.position.x, 0f, desiredPoint.position.z);
        }
        else
        {
            desiredPosition = new Vector3(this.transform.position.x,0f, this.transform.position.z) ;
        }
    }



    // Update is called once per frame
    void Update()
    {
        //detekcja gracza
        RaycastHit hit;

        Physics.Raycast(RaycastPosition.transform.position, (player.transform.position - RaycastPosition.transform.position).normalized, out hit, Range, mask); 

        if(hit.collider != null)
        {
            //Debug.Log(hit.collider.gameObject);

            GameObject hitObject;
            if (hit.collider.gameObject != null)
            {
                hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Player")) // jesli widac gracza
                {
                    if(playerDetected != PlayerDetectionState.Detected) {

                        playerDetected = PlayerDetectionState.Detected;
                        turretController.PlayerDetected();
                        playerInRange = true;

                        if (detectionCoroutine != null)
                        {
                            StopCoroutine(detectionCoroutine);
                        }
                    }
                    turretController.Target(player.transform.position);
                }
                else //jesli nie 
                {
                    if (playerDetected == PlayerDetectionState.Detected)
                    {
                        playerInRange = false;
                        playerDetected = PlayerDetectionState.Lost;
                        detectionCoroutine = StartCoroutine(LostTarget());
                    }
                }
            }
            

        }
        

        thisPos = new Vector3(this.transform.position.x,0f, this.transform.position.z);

        playerPosition = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
        distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);

        if (playerDetected == PlayerDetectionState.Detected || playerDetected == PlayerDetectionState.Lost)
        {

            desiredPosition = playerPosition - (playerPosition - thisPos).normalized * CloseDistance;
        }


        // dystans do celu
        distanceToPosition = Vector3.Distance(thisPos, desiredPosition);


        //docelowy kierunek
        directionToTarget = (desiredPosition - thisPos).normalized;
        //quaternion
        targetRotation = Quaternion.LookRotation(directionToTarget);


        angleToPosition = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, targetRotation.eulerAngles.y);
        //Debug.Log(angleToPosition);

        TurnToTarget();
        // Check if the object is already facing the target
        if (isFacingTarget)
        {
            MoveToTarget();
        }

    }


    private void MoveToTarget()
    {
        // If the object is within the stop distance, stop moving
        if (distanceToPosition <= 0.5f)
        {
            return;
        }

        // Move the object toward the target
        transform.position += this.transform.forward * 3f * Time.deltaTime;
    }

void TurnToTarget()
    {
        // Smoothly rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (25f* Time.deltaTime)/ Mathf.Abs(angleToPosition));

        // Check if the rotation is almost complete
        if (Quaternion.Angle(transform.rotation, targetRotation) < 20f)
        {
            isFacingTarget = true;
        }
        else { isFacingTarget = false; }
    }

    IEnumerator LostTarget()
    {
        yield return new WaitForSeconds(losingTime);
        playerDetected = PlayerDetectionState.NotDetected;
        turretController.PlayerNotDetected();
        desiredPosition = this.transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, Range);
    }

}

