using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    HotasInput input;

    Rigidbody rb;

    public float drive;

    public float strafe;

    public float speed;

    public float turn;

    public float rotSpeed;

    public Vector2 aiming;

    public GameObject Turret;

    public GameObject BulletPrefab;
    public GameObject Barrel;

    public float turretRotSpeed;

    public GameObject Cannon;

    public float CannonRotSpeed;

    public bool VR;
    public GameObject Rig;
    public GameObject PlayerCamer;

    public float multiplier;
    public float moveForce, turnTorque;

    public Transform[] anchors = new Transform[4];
    RaycastHit[] hits = new RaycastHit[4];


    private void Awake()
    {
        input = new HotasInput();
        input.Enable();

        rb = GetComponent<Rigidbody>();

        if(VR == false)
        {
            Rig.SetActive(false);
            PlayerCamer.SetActive(true);
        }

        input.Hotas.Trigger.performed += Shoot;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        drive = input.Hotas.Drive.ReadValue<float>();
        strafe = input.Hotas.Strafe.ReadValue<float>();
        turn = input.Hotas.Turn.ReadValue<float>();
        aiming = input.Hotas.AIm.ReadValue<Vector2>();


        rb.AddForce(drive *-1f * moveForce * transform.forward);
        rb.AddTorque(strafe * turnTorque * transform.up);

        //this.transform.Rotate(0f, rotSpeed * turn * Time.deltaTime, 0f);

        Turret.transform.Rotate(new Vector3(0f,turretRotSpeed * aiming.x * Time.deltaTime), Space.Self);

        Cannon.transform.Rotate(new Vector3(CannonRotSpeed * aiming.y * Time.deltaTime,0f,0f), Space.Self);

        for(int i = 0;i< 4; i++)
        {
            ApplyForce(anchors[i], hits[i]);
        }
    }

    void ApplyForce(Transform anchor, RaycastHit hit)
    {
        if(Physics.Raycast(anchor.position, -anchor.up, out hit))
        {
            float force = 0;
            force = Mathf.Abs(1/(hit.point.y - anchor.position.y));
            rb.AddForceAtPosition(transform.up * force * multiplier, anchor.position, ForceMode.Acceleration);
        }
    }


    void Shoot(InputAction.CallbackContext context)
    {
        GameObject bullet = Instantiate(BulletPrefab, Barrel.transform.position,Barrel.transform.rotation);
        Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
        rigidbody.velocity = Barrel.transform.forward * 200;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for(int i = 0; i<4 ; i++)
        {
            RaycastHit hit;
            Gizmos.DrawSphere(anchors[i].position, 0.2f);
            
            Physics.Raycast(anchors[i].position, -anchors[i].up, out hit);
            Gizmos.DrawSphere(hit.point, 0.2f);
            Gizmos.DrawLine(anchors[i].position, hit.point);
        }
    }
}
