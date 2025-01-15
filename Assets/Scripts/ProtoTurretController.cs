using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ProtoTurretController : MonoBehaviour
{
    InputReader input;

    ProtoTankController tankController;

    [Header("Turret")]

    [SerializeField] private Transform Turret;
    [SerializeField] private float turretRotSpeed;
    [Space(10)]
    [SerializeField] private Transform Cannon;
    [SerializeField] private float CannonRotSpeed;
    [Space(10)]
    [SerializeField] private Transform Barrel;
    [SerializeField] private float RecoilForce;
    [SerializeField] private Animation BarrelShotAnim;
    [Space(10)]
    [SerializeField] private float ReloadTime;
    private float reloadValue;

    [Space(10)]
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private float BulletDamage;
    [SerializeField] private float BulletSpeed;
    [SerializeField] LayerMask BulletMask;

    [Space(10)]
    [SerializeField] private Transform TurretHUD;
    [Space(10)]
    [SerializeField] private Camera TurretCam;
    [SerializeField] private bool zoomed;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float zoomFOV;

    [Header("TurretUI")]

    [SerializeField] private TurretState turretState;
    Coroutine reloadCoroutine;

    [SerializeField] private TMP_Text reloadText1;
    [SerializeField] private TMP_Text turretStateText1;
    [SerializeField] private TMP_Text reloadText2;
    [SerializeField] private TMP_Text turretStateText2;

    [Header("Locking")]

    [SerializeField] private GameObject Target;
    [SerializeField] private Transform turretTargetRotation;
    [SerializeField] private Transform cannonTargetRotation;
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
        tankController = GetComponent<ProtoTankController>();
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

        TurretReady();

    }

    private void OnDisable()
    {
        lockState = LockState.NoLock;
        turretState = TurretState.Disabled;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (lockState)
        {
            case LockState.Locked:

                Debug.Log("lookat");

                //turretTargetRotation.LookAt(Target.transform.position);
                //turretTargetRotation.localEulerAngles = new Vector3(0f, turretTargetRotation.localEulerAngles.y, 0f);
                //Turret.transform.rotation = Quaternion.Lerp(Turret.rotation, turretTargetRotation.rotation, (turretRotSpeed/Mathf.Abs( Mathf.DeltaAngle(Turret.eulerAngles.y, turretTargetRotation.eulerAngles.y))) * Time.deltaTime);

                Turret.transform.LookAt(Target.transform);
                Turret.localEulerAngles = new Vector3(0f, Turret.localEulerAngles.y, 0f);



                //cannonTargetRotation.LookAt(Target.transform.position);
                //cannonTargetRotation.localEulerAngles = new Vector3(cannonTargetRotation.localEulerAngles.x, 0f, 0f);
                //Cannon.transform.rotation = Quaternion.Lerp(Cannon.rotation, cannonTargetRotation.rotation, (CannonRotSpeed / Mathf.Abs(Mathf.DeltaAngle(Cannon.eulerAngles.x, cannonTargetRotation.eulerAngles.x))) * Time.deltaTime);
                
                Cannon.transform.LookAt(Target.transform);
                Cannon.localEulerAngles = new Vector3(Cannon.localEulerAngles.x, 0f, 0f);



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
            TurretHUD.transform.localEulerAngles = new Vector3(0f,Turret.transform.localEulerAngles.y,0f);
    }

    void Shoot()
    {
        if (turretState == TurretState.Ready)
        {
            GameObject bullet = Instantiate(BulletPrefab, Barrel.transform.position, Barrel.transform.rotation);
            IBullet b = bullet.GetComponent<IBullet>();
            float damage = Random.Range(BulletDamage - 10f, BulletDamage + 10f); 
            b.BulletInit(BulletDamage, BulletSpeed, BulletMask);
            TurretReload();
            tankController.AddForce(Barrel.transform.forward * -1 * RecoilForce, Barrel.transform.position);
            BarrelShotAnim.Play();
        }
    }

    void TurretReady()
    {
        turretState = TurretState.Ready;
        turretStateText1.text = "READY";
        turretStateText2.text = "READY";

        reloadValue = ReloadTime;
        reloadText1.text = reloadValue.ToString();
        reloadText2.text = reloadValue.ToString();
    }

    void TurretReload()
    {
        turretState = TurretState.Reloading;
        turretStateText1.text = "RELOADING";
        turretStateText2.text = "RELOADING";

        reloadCoroutine = StartCoroutine(Reloading());
    }

    IEnumerator Reloading()
    {
        
        while (reloadValue > 0)
        {
            yield return new WaitForSeconds(0.1f);
            reloadValue -= 0.1f;
            reloadValue = Mathf.Round( reloadValue * 10f);
            reloadValue = reloadValue / 10f;
            reloadText1.text = reloadValue.ToString();
            reloadText2.text = reloadValue.ToString();

        }

        TurretReady();
        
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

        if (lockingCoroutine != null)
        {
            StopCoroutine(lockingCoroutine);
        }

        input.LockAction = ReleaseLock;
        input.ReleaseLockAction -= ReleaseLock;
    }

    IEnumerator SearchForLock()
    {
        while (true)
        {
            RaycastHit hit;
            GameObject hitObject;
            HitboxPointer HB;

            Physics.Raycast(TurretCam.transform.position, TurretCam.transform.forward, out hit, lockDist, lockMask);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject);
                debugPos = hit.point;

                if (hit.collider.gameObject != null)
                {
                    hitObject = hit.collider.gameObject;

                    //Debug.Log("M1");

                        if (hitObject.TryGetComponent<HitboxPointer>(out HB))
                        {
                            //Debug.Log("M2");
                            Target = HB.GetMainGameObject();
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

    enum TurretState
    {
        Ready,
        Reloading,
        Disabled
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
