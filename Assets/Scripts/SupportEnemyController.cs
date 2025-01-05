using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SupportEnemyController : EnemyController
{
    [Header("Player Detectino Raycast")]
    [SerializeField] private Transform RaycastPosition;
    [SerializeField] private float Range;
    [SerializeField] private LayerMask mask;

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
        player = TankController.instance;
        enemyManager = EnemyManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        //detekcja gracza
        RaycastHit hit;

        Physics.Raycast(RaycastPosition.transform.position, (player.transform.position - RaycastPosition.transform.position).normalized, out hit, Range, mask);

        if (hit.collider != null)
        {
            //Debug.Log(hit.collider.gameObject);

            GameObject hitObject;
            if (hit.collider.gameObject != null)
            {
                hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Player")) // jesli widac gracza
                {
                    if (playerDetected != PlayerDetectionState.Detected)
                    {

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
    }


    IEnumerator LostTarget()
    {
        yield return new WaitForSeconds(losingTime);
        playerDetected = PlayerDetectionState.NotDetected;
        turretController.PlayerNotDetected();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, Range);
    }
}
