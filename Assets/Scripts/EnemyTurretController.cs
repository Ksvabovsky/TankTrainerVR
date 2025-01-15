using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyTurretController : MonoBehaviour
{

    [SerializeField] private TurretState turretState;
    [SerializeField] private PlayerDetectionState playerDetected;
    [SerializeField] Vector3 turretTarget;

    [Header("Turret")]

    [SerializeField] private Transform Turret;
    [SerializeField] private float turretRotSpeed;
    [SerializeField] private Transform TurretTargetRotation;

    [Space(10)]
    [SerializeField] private Transform Cannon;
    [SerializeField] private float CannonRotSpeed;
    [SerializeField] private Transform CannonTargetRotation;

    [Header("Shooting")]

    [SerializeField] private Transform firepoint;
    [SerializeField] Vector3 targetOffset;
    [SerializeField] Vector3 offsetValue;
    [SerializeField] float accuracy;
    [SerializeField] float diff;

    [SerializeField] private float ReloadTime;

    Coroutine reloadCoroutine;

    [Header("Bullet")]

    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private float BulletDamage;
    [SerializeField] private float BulletSpeed;
    [SerializeField] LayerMask BulletMask;
    [SerializeField] Transform bulletParent;




    // Start is called before the first frame update
    void Start()
    {
        bulletParent = BulletManager.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerDetected != PlayerDetectionState.NotDetected)
        {
            targetOffset = turretTarget + offsetValue;

            TurretTargetRotation.LookAt(targetOffset);
            TurretTargetRotation.localEulerAngles = new Vector3(0f, TurretTargetRotation.localEulerAngles.y, 0f);
            CannonTargetRotation.LookAt(targetOffset);
            CannonTargetRotation.localEulerAngles = new Vector3( CannonTargetRotation.localEulerAngles.y, 0f, 0f);
        }

        //distance = Vector3.Distance(target.position, this.transform.position);
        //Debug.Log(Mathf.DeltaAngle(Turret.rotation.eulerAngles.y, TurretTargetRotation.eulerAngles.y));
        Turret.rotation = Quaternion.Lerp(Turret.rotation, TurretTargetRotation.rotation, (turretRotSpeed / Mathf.Abs(Mathf.DeltaAngle(Turret.rotation.eulerAngles.y, TurretTargetRotation.eulerAngles.y)) )* Time.deltaTime);
        Cannon.rotation = Quaternion.Lerp(Cannon.rotation, CannonTargetRotation.rotation, (CannonRotSpeed / Mathf.Abs(Mathf.DeltaAngle(Cannon.rotation.eulerAngles.y, CannonTargetRotation.eulerAngles.y))) * Time.deltaTime);

        if (playerDetected == PlayerDetectionState.Detected && turretState == TurretState.Ready) {
            diff = Mathf.Abs(Mathf.DeltaAngle(Turret.rotation.eulerAngles.y, TurretTargetRotation.rotation.eulerAngles.y));
            if (diff < 5f)
            {
                Fire();
            }
        }

    }

    public void PlayerNotDetected()
    {
        playerDetected = PlayerDetectionState.NotDetected;
        TurretTargetRotation.localEulerAngles = Vector3.zero;
        CannonTargetRotation.localEulerAngles = Vector3.zero;
    }

    public void PlayerDetected()
    {
        playerDetected = PlayerDetectionState.Detected;
    }

    public void Target(Vector3 position)
    {
        turretTarget = position;
    }

    void Fire()
    {
        if (BulletPrefab)
        {
            GameObject bullet = Instantiate(BulletPrefab, firepoint.position, firepoint.rotation, bulletParent);
            IBullet bs = bullet.GetComponent<IBullet>();
            bs.BulletInit(BulletDamage, BulletSpeed, BulletMask);
        }
        else
        {
            Debug.Log("PEW");
        }

        offsetValue = new Vector3(accuracy * Random.value, accuracy * Random.value, accuracy * Random.value);
        TurretReload();
    }

    void TurretReload()
    {
        turretState = TurretState.Reloading;
        reloadCoroutine = StartCoroutine(Reloading());
    }

    IEnumerator Reloading()
    {
        yield return new WaitForSeconds(ReloadTime);
        turretState = TurretState.Ready;

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(targetOffset, 0.5f);

    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(this.transform.position, shootingDistance);
    }
}

public enum TurretState
{
    Ready,
    Reloading,
    Disabled
}
