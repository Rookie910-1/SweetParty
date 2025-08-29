using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{

    public InputActionAsset actions;

    protected InputAction m_jump;

    protected float m_movementDirectionUnlockTime;

    protected InputAction m_movement;

    protected InputAction m_run;

    protected Camera m_camera;

    protected float? m_lastJumpTime;

    protected const float k_jumpBuffer = 0.15f;

    protected virtual void Awake() => CacheActions();
    protected virtual void CacheActions()
    {
        m_movement = actions["Movement"];
        m_run = actions["Run"];
        m_jump = actions["Jump"];
    }
    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
        actions.Enable();
    }

    protected virtual void Update()
    {
        if(m_jump.WasPressedThisFrame())
        {
            m_lastJumpTime = Time.time;
        }
    }

    protected void OnEnable()
    {
        actions?.Enable(); 
    }

    protected void OnDisable()
    {
        actions?.Disable();
    }

    public virtual Vector3 GetMovementDirection()
    {
        if(Time.time < m_movementDirectionUnlockTime) return Vector3.zero;

        var movementValue= m_movement.ReadValue<Vector2>();

        return GetAxisWithCrossDeadZone(movementValue);
    }

    public virtual Vector3 GetAxisWithCrossDeadZone(Vector2 axis) 
    {
        var deadZone = InputSystem.settings.defaultDeadzoneMin;
        axis.x = Mathf.Abs(axis.x) > deadZone ? RemapToDeadZone(axis.x,deadZone) : 0;
        axis.y = Mathf.Abs(axis.y) > deadZone ? RemapToDeadZone(axis.y, deadZone) : 0;
        return new Vector3(axis.x, 0, axis.y);
    }

    protected float RemapToDeadZone(float value,float deadZone) 
    {
        return (value - deadZone) / (1 - deadZone); 
    }

    public virtual Vector3 GetMovementCamerDirection()
    {
        var direction=GetMovementDirection();

        if(direction.sqrMagnitude >0)
        {
            var rotation = Quaternion.AngleAxis(m_camera.transform.eulerAngles.y, Vector3.up);
            direction = rotation * direction;
            direction = direction.normalized;
        }

        return direction;
    }

    public virtual bool GetJumpDown()
    {

        if(m_lastJumpTime !=null && Time.time - m_lastJumpTime < k_jumpBuffer)
        {
            m_lastJumpTime = null;
            return true;
        }

        return false;
    }

    public virtual bool GetRun() => m_run.IsPressed();

    public virtual bool GetJumpUp() => m_jump.WasReleasedThisFrame();
}
