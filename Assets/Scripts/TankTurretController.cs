using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private LockState lockState;
    [SerializeField] private float lockDist;
    [SerializeField] private LayerMask lockMask;

    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<InputReader>();
    }

    void Start()
    {
        lockState = LockState.NoLock;

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
        //RaycastHit hit;

        //Physics.Raycast(TurretCam.transform.position, TurretCam.transform.forward, out hit, lockDist,lockMask);


        lockState = LockState.Searching;    
    }

    void ReleaseLock()
    {
        

        lockState = LockState.NoLock;
    }


    enum LockState
    {
        NoLock,
        Searching,
        Locked
    }
}
