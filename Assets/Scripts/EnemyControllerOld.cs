using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerOld : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletParent;
    [SerializeField] float bulletSpeed;

    [SerializeField] EnemyManager enemyManager;
    public GameObject square;
    public GameObject direction;

    [SerializeField] bool isShooting;
    [SerializeField] bool playerInRange;
    [SerializeField] Transform turret;
    [SerializeField] Transform cannon;
    [SerializeField] Transform firepoint;
    [SerializeField] Transform target;
    [SerializeField] Vector3 targetOffset;
    [SerializeField] Vector3 offsetValue;
    [SerializeField] float accuracy;

    [SerializeField] float reloadTime;
    [SerializeField] float reloadValue;

    [SerializeField] float distance;
    [SerializeField] float shootingDistance;


    // Start is called before the first frame update
    void Start()
    {
        target = TankController.instance.transform;
        enemyManager = EnemyManager.instance;
        //enemyManager.AddEnemy(this);
        bulletParent = BulletManager.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(target.position, this.transform.position);

        turret.LookAt(targetOffset);
        turret.localEulerAngles = new Vector3(0f, turret.localEulerAngles.y, 0f);

        firepoint.LookAt(targetOffset);

        targetOffset = target.position + offsetValue;

        reloadValue -= Time.deltaTime;

        if(distance < shootingDistance)
        {
            playerInRange = true;
            if(reloadValue <= 0f)
            {
                if(isShooting)
                Shoot();
            }
        }
        else
        {
            if(playerInRange)
            {
                playerInRange = false;
            }
        }

    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation, bulletParent);
        Rigidbody rb= bullet.GetComponent<Rigidbody>();
        rb.velocity = bullet.transform.forward * bulletSpeed;

        offsetValue = new Vector3(accuracy * Random.value, accuracy * Random.value, accuracy * Random.value);
        reloadValue = reloadTime;
    }

    public bool IsPlayerLocked()
    {
        return playerInRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetOffset, 0.5f);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, shootingDistance);
    }

    public void MissonOver()
    {
        Debug.Log("Roger, Stop");
        this.enabled = false;
    }
}
