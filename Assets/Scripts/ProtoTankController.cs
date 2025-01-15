using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ProtoTankController : MonoBehaviour
{
    public static ProtoTankController instance;

    bool active;

    Rigidbody rb;
    InputReader input;

    [SerializeField] TankState state;
    
    ProtoDriveController driveController;
    ProtoTurretController turretScript;
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
        turretScript = GetComponent<ProtoTurretController>();

        Application.targetFrameRate = -1;

        instance = this;

        radarScript.ClearWarningLight();
    }
    void Start()
    {
        throttleOrigin = throttle.localPosition;
        startAnim.Play("ScreensOFF");
        rb.constraints = RigidbodyConstraints.FreezePosition;
        radarScript.enabled = false;

        state = TankState.start;
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
        
        startAnim.Play("ScreensOn");
        glassAnim.Play("Hello World");
        EngineAudio.Play();

        input.ChangeToTank();

        StartCoroutine(StartEnum());
        state = TankState.running;
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

   
    public void TankStop(GameState gameState)
    {
        active = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        turretScript.enabled = false;
        driveController.enabled = false;
        radarScript.enabled = false;
        gauges.Zero();

        startAnim.Play("ScreensOFF");
        glassAnim.Play("CoverGlass");
        EngineAudio.Stop();

        if(gameState == GameState.Completed)
        {
            HUDController.MissionCompleted();
            state = TankState.stop;
        }
        else
        {
            HUDController.MissionOver();
            state = TankState.dead;
        }

        
        radarScript.ClearWarningLight();

        input.ChangeToUI();


    }

    public void TankLost()
    {
        GameStateManager.instance.GameOver();
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


public enum TankState{
    start,
    running,
    stop,
    dead,
}