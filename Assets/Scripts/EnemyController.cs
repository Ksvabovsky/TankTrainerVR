using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletParent;
    [SerializeField] float bulletSpeed;

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
            if(reloadValue <= 0f)
            {
                Shoot();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetOffset, 0.5f);
    }
}
