using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    public static TankController instance;


    InputReader input;

    Rigidbody rb;

    [Header("HoverPhysics")]

    [SerializeField] private Transform[] anchors = new Transform[4];
    [SerializeField] RaycastHit[] hits = new RaycastHit[4];

    public AnimationCurve hoverPower;
    [Space(10)]
    [SerializeField] private float currentPower;
    [SerializeField] private float multiplier;
    [SerializeField] private float moveForce, turnTorque;

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

    [Header("ControlsVis")]

    [SerializeField] private Transform joystick;
    [SerializeField] private float joyAngle;
    
    [Space(10)]
    [SerializeField] private Transform throttle;
    private Vector3 throttleOrigin;
    [SerializeField] private float throttleDist;

    [Header("Lock")]

    [SerializeField] bool Locked;
    Coroutine lockCoroutine;
    [SerializeField] Material material;
    //[SerializeField] AudioClip lockSound;
    //[SerializeField] AudioClip newContact;
    [SerializeField] AudioSource source;


    private void Awake()
    {
        input = GetComponent<InputReader>();
        rb = GetComponent<Rigidbody>();

        Application.targetFrameRate = -1;

        instance = this;

        material.DisableKeyword("_EMISSION");
    }
    void Start()
    {
        input.TriggerAction += Shoot;
        input.ZoomAction += Zoom;
        throttleOrigin = throttle.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Turret.transform.Rotate(new Vector3(0f, turretRotSpeed * input.GetAim().x * Time.deltaTime), Space.Self);

            Cannon.transform.Rotate(new Vector3(CannonRotSpeed * input.GetAim().y * Time.deltaTime, 0f, 0f), Space.Self);
            float rot = Cannon.transform.localEulerAngles.x;
            if(rot > 180f)
            {
            rot = rot - 360f;
            }
            if ( rot > 9f)
            {
                Cannon.localEulerAngles = new Vector3(9f, 0f, 0f);
                //Debug.Log(Cannon.localEulerAngles);
            }
            if( rot < -25f)
            {
                Cannon.localEulerAngles = new Vector3(-25f, 0f, 0f);
                //Debug.Log(Cannon.localEulerAngles);
            }


        for (int i = 0; i < 4; i++)
        {
            ApplyForce(anchors[i], hits[i]);
        }

        rb.AddForce(input.GetDrive() * moveForce * transform.forward);
        rb.AddForce(input.GetStrafe() * moveForce * transform.right);
        rb.AddTorque(input.GetTurn() * turnTorque * transform.up);
    }

    private void Update()
    {
        if (TurretHUD)
            TurretHUD.transform.localEulerAngles = Turret.transform.localEulerAngles;

        if (joystick)
            joystick.transform.localEulerAngles = new Vector3(input.GetAim().y * joyAngle, 0f, input.GetAim().x * joyAngle * -1);
        if (throttle)
            throttle.transform.localPosition = new Vector3(throttleOrigin.x, throttleOrigin.y, throttleOrigin.z + throttleDist * input.GetDrive());

    }

    void ApplyForce(Transform anchor, RaycastHit hit)
    {
        if (Physics.Raycast(anchor.position, -anchor.up, out hit))
        {
            float force = 0;
            force = Mathf.Abs(1 / (hit.point.y - anchor.position.y));
            //currentPower = Mathf.Sqrt(multiplier * (hoverPower.Evaluate(force) + 1));
            rb.AddForceAtPosition(transform.up * force, anchor.position, ForceMode.Acceleration);
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(BulletPrefab, Barrel.transform.position, Barrel.transform.rotation);
        Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
        rigidbody.velocity = Barrel.transform.forward * 200;
    }

    void Zoom()
    {
        if(zoomed)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < 4; i++)
        {
            RaycastHit hit;
            Gizmos.DrawSphere(anchors[i].position, 0.2f);

            Physics.Raycast(anchors[i].position, -anchors[i].up, out hit);
            Gizmos.DrawSphere(hit.point, 0.2f);
            Gizmos.DrawLine(anchors[i].position, hit.point);
        }
    }

    public void PlayerLocked()
    {
        if (!Locked)
        {
            Locked = true;
            if (lockCoroutine == null)
            {
                lockCoroutine = StartCoroutine(LockSound());
                material.EnableKeyword("_EMISSION");
            }
        }
    }

    public void PlayerUnlocked()
    {
        if (Locked)
        {
            Locked = false;
            if (lockCoroutine != null)
            {
                StopCoroutine(lockCoroutine);
                lockCoroutine = null;
                material.DisableKeyword("_EMISSION");
            }
        }
    }

    public bool IsLocked()
    {
        return Locked;
    }

    IEnumerator LockSound() {

        //source.Play();
        //yield return new WaitForSeconds(0.25f);
        //material.DisableKeyword("_EMISSION");
        //yield return new WaitForSeconds(0.25f);
        //material.EnableKeyword("_EMISSION");
        //yield return new WaitForSeconds(0.25f);
        //material.DisableKeyword("_EMISSION");
        //yield return new WaitForSeconds(0.25f);
        //material.EnableKeyword("_EMISSION");
        //yield return new WaitForSeconds(2f);

        //source.clip = lockSound;
        while (true)
        {
            
            source.Play();
            yield return new WaitForSeconds(3f);
        }
    }
}
