using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    public static TankController instance;

    bool active;

    InputReader input;

    TankTurretController turretScript;
    [SerializeField] HoloSightScript HUDController;
    

    Rigidbody rb;

    [Header("HoverPhysics")]

    [SerializeField] private Transform[] anchors = new Transform[4];
    [SerializeField] RaycastHit[] hits = new RaycastHit[4];

    public AnimationCurve hoverPower;
    [Space(10)]
    [SerializeField] private float currentPower;
    [SerializeField] private float multiplier;
    [SerializeField] private float moveForce, turnTorque;

    

    [Header("ControlsVis")]

    [SerializeField] private Transform joystick;
    [SerializeField] private float joyAngle;
    
    [Space(10)]
    [SerializeField] private Transform throttle;
    private Vector3 throttleOrigin;
    [SerializeField] private float throttleDist;
    [Space(10)]
    [SerializeField] private GaugeController gauges;
    [Space(10)]
    [SerializeField] RadarScript radarScript;


    [Header("PlayerLock")]

    [SerializeField] bool Locked;
    Coroutine lockCoroutine;
    [SerializeField] Material material;
    //[SerializeField] AudioClip lockSound;
    //[SerializeField] AudioClip newContact;
    [SerializeField] AudioSource source;

    [Header("TankStart")]

    [SerializeField] AnimationClip screensOff;
    [SerializeField] AnimationClip screendOn;
    [SerializeField] Animation startAnim;

    [SerializeField] AnimationClip glassUncover;
    [SerializeField] AnimationClip glassCover;
    [SerializeField] Animation glassAnim;

    [SerializeField] AudioSource EngineAudio;
    [SerializeField] AudioSource UIAudio;
    [SerializeField] AudioClip clickAudio;



    private void Awake()
    {
        input = GetComponent<InputReader>();
        rb = GetComponent<Rigidbody>();
        turretScript = GetComponent<TankTurretController>();

        Application.targetFrameRate = -1;

        instance = this;

        material.DisableKeyword("_EMISSION");
    }
    void Start()
    {
        throttleOrigin = throttle.localPosition;
        startAnim.Play("ScreensOFF");
        rb.constraints = RigidbodyConstraints.FreezePosition;
        radarScript.enabled = false;

        active = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        


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

    public float GetSpeed()
    {
        return rb.velocity.magnitude;
    }

    public bool IsActive()
    {
        return active;
    }

    public void StartTank()
    {
        GameStateManager.instance.GameStarted();  
        active = true;
        rb.constraints = RigidbodyConstraints.None;
        //gauges.enabled = true;
        turretScript.enabled = true;
        
        startAnim.Play("ScreensOn");
        glassAnim.Play("Hello World");
        EngineAudio.Play();

        input.ChangeToTank();

    }

    public void TankLost()
    {
        GameStateManager.instance.GameOver();
        active = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        turretScript.enabled = false;
        startAnim.Play("ScreensOFF");
        glassAnim.Play("CoverGlass");
        EngineAudio.Stop();

        HUDController.MissionOver();
        input.ChangeToUI();

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

    public void PlayClick()
    {
        UIAudio.PlayOneShot(clickAudio);
        Debug.Log("chujaudio");
    }

}
