using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    public static TankController instance;

    bool active;

    Rigidbody rb;
    InputReader input;
    
    ProtoDriveController driveController;
    TankTurretController turretScript;
    [SerializeField] HoloSightScript HUDController;

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
        driveController = GetComponent<ProtoDriveController>();
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
    private void Update()
    {
        if (active)
        {

            if (joystick)
                joystick.transform.localEulerAngles = new Vector3(input.GetAim().y * joyAngle, 0f, input.GetAim().x * joyAngle * -1);
            if (throttle)
                throttle.transform.localPosition = new Vector3(throttleOrigin.x, throttleOrigin.y, throttleOrigin.z + throttleDist * input.GetDrive());
            float rpm = 0.5f + 0.15f * Mathf.Abs(input.GetDrive()) + 0.1f * Mathf.Abs(input.GetStrafe());
            gauges.SetNumbers(vrpm1: rpm, vrpm2: rpm);
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
        
        //gauges.enabled = true;
        
        startAnim.Play("ScreensOn");
        glassAnim.Play("Hello World");
        EngineAudio.Play();

        input.ChangeToTank();

        StartCoroutine(StartEnum());

    }

    IEnumerator StartEnum()
    {
        yield return new WaitForSeconds(2.0f);

        driveController.enabled = true;
        rb.constraints = RigidbodyConstraints.None;

        yield return new WaitForSeconds(2.0f);
        radarScript.enabled = true;

        yield return new WaitForSeconds(0.5f);
        turretScript.enabled = true;


    }

    public void TankLost()
    {
        GameStateManager.instance.GameOver();
        active = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        turretScript.enabled = false;
        driveController.enabled = false;
        radarScript.enabled = false;
        gauges.Zero();

        startAnim.Play("ScreensOFF");
        glassAnim.Play("CoverGlass");
        EngineAudio.Stop();

        HUDController.MissionOver();
        PlayerUnlocked();

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
        //Debug.Log("klika");
    }

    public void AddForce(Vector3 direction, Vector3 position)
    {
        rb.AddForceAtPosition(direction, position, ForceMode.Impulse);
    }

}
