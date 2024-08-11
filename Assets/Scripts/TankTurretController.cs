using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TankTurretController : MonoBehaviour
{
    InputReader input;

    [Header("Turret")]

    [SerializeField] private Transform Turret;
    [SerializeField] private float turretRotSpeed;
    [Space(10)]
    [SerializeField] private Transform Cannon;
    [SerializeField] private float CannonRotSpeed;
    [Space(10)]
    [SerializeField] private Transform Barrel;
    [SerializeField] private GameObject BulletPrefab;
    [Space(10)]
    [SerializeField] private Transform TurretHUD;
    [Space(10)]
    [SerializeField] private Camera TurretCam;
    [SerializeField] private bool zoomed;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float zoomFOV;

    [Header("Locking")]

    [SerializeField] private GameObject Target;
    [SerializeField] private GameObject turretTargetRotation;
    [SerializeField] private GameObject cannonTargetRotation;
    private Vector3 debugPos;

    [SerializeField] private LockState lockState;
    [SerializeField] private float lockDist;
    [SerializeField] private LayerMask lockMask;

    private Coroutine lockingCoroutine;

    [SerializeField] private TMP_Text lockStateText;

    [Header("Layers")]
    [SerializeField] private string enemyLayerName;


    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<InputReader>();
    }

    void Start()
    {
        ReleaseLock();

    }

    private void OnEnable()
    {
        input.TriggerAction += Shoot;
        input.ZoomAction += Zoom;
        input.LockAction += Lock;
        input.ReleaseLockAction += ReleaseLock;
        lockState = LockState.NoLock;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (lockState)
        {
            case LockState.Locked:

                Debug.Log("lookat");

                //turretTargetRotation.transform.LookAt(Target.transform);

                //Turret.transform.rotation = Quaternion.Lerp(Turret.rotation, turretTargetRotation.transform.rotation, turretRotSpeed/Mathf.DeltaAngle(Turret.rotation.eulerAngles.y, turretTargetRotation.transform.rotation.eulerAngles.y));

                Turret.transform.LookAt(Target.transform);

                //Turret.transform.localEulerAngles = new Vector3(0f,Turret.rotation.y, 0f);

                //cannonTargetRotation.transform.LookAt(Target.transform);

                //Cannon.transform.rotation = Quaternion.Lerp(Cannon.rotation, cannonTargetRotation.transform.rotation, CannonRotSpeed / Mathf.DeltaAngle(Cannon.rotation.eulerAngles.y, cannonTargetRotation.transform.rotation.eulerAngles.y));

                Cannon.transform.LookAt(Target.transform);

                //Cannon.transform.localEulerAngles = new Vector3( Cannon.rotation.x, 0f, 0f);

                break;

            case LockState.Searching:
            case LockState.NoLock:

                Turret.transform.Rotate(new Vector3(0f, turretRotSpeed * input.GetAim().x * Time.deltaTime), Space.Self);

                Cannon.transform.Rotate(new Vector3(CannonRotSpeed * input.GetAim().y * Time.deltaTime, 0f, 0f), Space.Self);
                float rot = Cannon.transform.localEulerAngles.x;
                if (rot > 180f)
                {
                    rot = rot - 360f;
                }
                if (rot > 9f)
                {
                    Cannon.localEulerAngles = new Vector3(9f, 0f, 0f);
                    //Debug.Log(Cannon.localEulerAngles);
                }
                if (rot < -25f)
                {
                    Cannon.localEulerAngles = new Vector3(-25f, 0f, 0f);
                    //Debug.Log(Cannon.localEulerAngles);
                }

                break;
        }
        
    }


    private void Update()
    {
        if (TurretHUD)
            TurretHUD.transform.localEulerAngles = Turret.transform.localEulerAngles;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(BulletPrefab, Barrel.transform.position, Barrel.transform.rotation);
        Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
        rigidbody.velocity = Barrel.transform.forward * 200;
    }

    void Zoom()
    {
        if (zoomed)
        {
            TurretCam.fieldOfView = defaultFOV;
            zoomed = false;
        }
        else
        {
            TurretCam.fieldOfView = zoomFOV;
            zoomed = true;
        }
    }

    void Lock()
    {
        lockingCoroutine = StartCoroutine(SearchForLock());

        lockStateText.text = "SEARCHING";
        lockState = LockState.Searching;    
    }

    void ReleaseLock()
    {
        if(lockingCoroutine != null)
        {
            StopCoroutine(lockingCoroutine);
        }
        if(Target != null)
        {
            Target = null;

            input.LockAction = Lock;
            input.ReleaseLockAction += ReleaseLock;
        }

        lockStateText.text = "";
        lockState = LockState.NoLock;
    }

    void Locked()
    {

        lockStateText.text = "LOCK";
        lockState = LockState.Locked;

        input.LockAction = ReleaseLock;
        input.ReleaseLockAction -= ReleaseLock;
    }

    IEnumerator SearchForLock()
    {
        while (true)
        {
            RaycastHit hit;
            GameObject hitObject;
            HitboxPointer HB = null;

            Physics.Raycast(TurretCam.transform.position, TurretCam.transform.forward, out hit, lockDist, lockMask);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject);
                debugPos = hit.point;

                if (hit.collider.gameObject != null)
                {
                    hitObject = hit.collider.gameObject;

                    //Debug.Log("M1");

                    if (hitObject.tag == enemyLayerName)
                    {
                        //Debug.Log("M2");

                        if (hitObject.TryGetComponent<HitboxPointer>(out HB))
                        {
                            //Debug.Log("M3");
                            Target = HB.GetMainGameObject();
                        }
                    }

                    if (Target != null)
                    {
                        Locked();
                        yield return null;
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
        
        
    }


    enum LockState
    {
        NoLock,
        Searching,
        Locked
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (lockState == LockState.Searching)
        {
            Gizmos.DrawLine(TurretCam.transform.position, debugPos);
        }
    }
}
