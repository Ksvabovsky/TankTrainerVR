using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    HotasInput input;

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

    private void Awake()
    {
        input = new HotasInput();
        input.Enable();

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
        

        this.transform.Translate(this.transform.forward * speed * drive *Time.deltaTime);
        this.transform.Translate(this.transform.forward * 5f * strafe * Time.deltaTime);
        this.transform.Rotate(0f, rotSpeed * turn * Time.deltaTime, 0f);

        Turret.transform.Rotate(new Vector3(0f,turretRotSpeed * aiming.x * Time.deltaTime), Space.Self);

        Cannon.transform.Rotate(new Vector3(CannonRotSpeed * aiming.y * Time.deltaTime,0f,0f), Space.Self);
    }


    void Shoot(InputAction.CallbackContext context)
    {
        GameObject bullet = Instantiate(BulletPrefab, Barrel.transform.position,Barrel.transform.rotation);
        Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
        rigidbody.velocity = Barrel.transform.forward * 200;
    }
}
