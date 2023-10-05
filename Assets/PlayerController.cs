using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    HotasInput input;

    public float drive;

    public float speed;

    public float turn;

    public float rotSpeed;

    public Vector2 aiming;

    public GameObject Turret;

    public float turretRotSpeed;

    public GameObject Cannon;

    public float CannonRotSpeed;

    private void Awake()
    {
        input = new HotasInput();
        input.Enable();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        drive = input.Hotas.Drive.ReadValue<float>();
        turn = input.Hotas.Turn.ReadValue<float>();
        aiming = input.Hotas.AIm.ReadValue<Vector2>();

        this.transform.Translate(this.transform.forward * speed * drive *Time.deltaTime);
        this.transform.Rotate(0f, rotSpeed * turn * Time.deltaTime, 0f);

        Turret.transform.Rotate(new Vector3(0f,turretRotSpeed * aiming.x * Time.deltaTime), Space.Self);

        Cannon.transform.Rotate(new Vector3(CannonRotSpeed * aiming.y * Time.deltaTime,0f,0f), Space.Self);
    }
}
