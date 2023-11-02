using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{

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

    [Header("ControlsVis")]

    [SerializeField] private Transform joystick;
    [SerializeField] private float joyAngle;
    
    [Space(10)]
    [SerializeField] private Transform throttle;
    private Vector3 throttleOrigin;
    [SerializeField] private float throttleDist;

    private void Awake()
    {
        input = InputReader.instance;
        rb = GetComponent<Rigidbody>();

        Application.targetFrameRate = -1;
    }
    void Start()
    {
        input.TriggerAction += Shoot;
        throttleOrigin = throttle.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Turret.transform.Rotate(new Vector3(0f, turretRotSpeed * input.GetAim().x * Time.deltaTime), Space.Self);

        Cannon.transform.Rotate(new Vector3(CannonRotSpeed * input.GetAim().y * Time.deltaTime, 0f, 0f), Space.Self);

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
}
