using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, PlayerInputs.ITankControlActions
{
    PlayerInputs input;

    public static InputReader instance { get; private set; }

    public Action TriggerAction;
    public Action ZoomAction;

    [SerializeField] private float drive;

    [SerializeField] private float strafe;

    [SerializeField] private float turn;

    [SerializeField] private Vector2 aim;

    private void Awake()
    {
        instance = this;

        input = new PlayerInputs();
        input.TankControl.SetCallbacks(this);
        
    }

    public void ChangeToUI()
    {
        input.TankControl.Disable();
        input.UI.Enable();
    }

    public void ChangeToTank()
    {
        input.UI.Disable();
        input.TankControl.Enable();
    }


    public void OnTrigger(InputAction.CallbackContext context)
    {
        if (TriggerAction != null && context.performed)
        {
            TriggerAction.Invoke();
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (ZoomAction != null && context.performed)
        {
            ZoomAction.Invoke();
        }
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        aim = input.TankControl.Aim.ReadValue<Vector2>();
    }

    public void OnDrive(InputAction.CallbackContext context)
    {
        drive = input.TankControl.Drive.ReadValue<float>();
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        turn = input.TankControl.Turn.ReadValue<float>();
    }

    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe = input.TankControl.Strafe.ReadValue<float>();
    }

    public Vector2 GetAim()
    {
        return aim;
    }

    public float GetDrive()
    {
        return drive;
    }

    public float GetTurn()
    {
        return turn;
    }

    public float GetStrafe()
    {
        return strafe;
    }

}
