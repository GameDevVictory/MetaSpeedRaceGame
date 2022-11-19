using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    
    private TPSActionAssets tpsActionAsset;
    // Start is called before the first frame update
    private void Awake()
    {
        tpsActionAsset = new TPSActionAssets();
    }
    private void OnEnable()
    {
        tpsActionAsset.Enable();
    }
    private void OnDisable()
    {
        tpsActionAsset.Disable();
    }

    internal void ToggleInputs(bool enabled)
    {
        if (enabled)
        {
            tpsActionAsset.Enable();
        }
        else
        {
            tpsActionAsset.Disable();
        }
    }


    #region Handle Input
    public Vector2 GetPlayerMovement()
    {
        return tpsActionAsset.Player.Move.ReadValue<Vector2>();
    }
    public Vector2 GetMouseDelta()
    {
        return tpsActionAsset.Player.Look.ReadValue<Vector2>();
    }
    public bool PlayerJumpedThisFrame()
    {
        return tpsActionAsset.Player.Jump.triggered;
    }
    public bool Reloaded()
    {
        return tpsActionAsset.Player.Reload.triggered;
    }
    public bool ISSpriting()
    {
        return tpsActionAsset.Player.Sprint.IsPressed();
    }
    public bool ShootPressed()
    {
        return tpsActionAsset.Player.Shoot.WasPressedThisFrame();
    }
    public bool ShootCancelled()
    {
        return tpsActionAsset.Player.Shoot.WasReleasedThisFrame();
    }
    public bool IsShooting()
    {
        return tpsActionAsset.Player.Shoot.IsPressed();
    }

    public float GetMouseScroll()
    {
        return tpsActionAsset.Player.WeaponChange.ReadValue<float>();
    }

    public bool IsCouching()
    {
        return tpsActionAsset.Player.Crouch.IsPressed();
    }

    internal bool GetNextWeapon()
    {
        return tpsActionAsset.Player.NextWeapon.WasPressedThisFrame();
    }

    internal bool GetPreviouWeepon()
    {
        return tpsActionAsset.Player.PrevWeapon.WasReleasedThisFrame();
    }

    public bool GetAimButton()
    {
        return tpsActionAsset.Player.Aim.IsPressed();
    }
    public bool GetInteractButton()
    {
        return tpsActionAsset.Player.Interact.WasReleasedThisFrame();
    }

    public bool Braked()
    {
        return tpsActionAsset.Player.Brake.IsPressed();
    }
    public bool BrakeReleased()
    {
        return tpsActionAsset.Player.Brake.WasReleasedThisFrame();
    }

    public bool CheckLeftAlt()
    {
        return tpsActionAsset.Player.CursonState.WasPressedThisFrame();
    }

    public bool CheckMouseMiddleButton()
    {
        return tpsActionAsset.Player.MouseMiddleButton.IsPressed();
    }
    public bool CheckNitroInput()
    {
        return tpsActionAsset.Player.Nitro.IsPressed();
    }

    internal bool GetMouseLeftButton()
    {
        return tpsActionAsset.Player.LeftMouseButton.triggered;
    }
    public bool GetEmoteButton()
    {
        return tpsActionAsset.Player.EmoteButton.IsPressed();
    }

    internal bool ResetCar()
    {
        return tpsActionAsset.Player.ResetCar.IsPressed();
    }
    [SerializeField] float resetCarTimer;
    float currentTimer;

    private void Update()
    {
        if (ResetCar())
        {
            currentTimer += Time.deltaTime;
            if(currentTimer> resetCarTimer)
            {
                currentTimer = 0;
                if (MetaManager.Instance.myCarController != null)
                {
                    MetaManager.Instance.myCarController.RespawnCar();
                }
            }
        }
        else
        {
            currentTimer = 0;
        }
    }

    #endregion
}
