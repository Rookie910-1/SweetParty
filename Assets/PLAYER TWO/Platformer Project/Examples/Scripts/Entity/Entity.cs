using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


public class Entity : MonoBehaviour
{
    
}
public class Entity<T> : Entity where T : Entity<T>
{
    public EntityStateManager<T> states {  get; private set; }
    protected void HandleStates() => states.Step();

    protected virtual void InitializeStateManager() => states= GetComponent<EntityStateManager<T>>();

    public bool isGrounded { get; protected set; } = true;

    public Vector3 velocity { get;  set; }

    public float turningDragMultiplier { get; set; } = 1f;

    public float topSpeedMultiplier { get; set; } = 1f;

    public float acclerationMultiplier { get; set; } = 1f;

    public float decelerationMultiple { get; set; } = 1f;

    public float gravityMultiplier { get; set; }

    public CharacterController controller { get; protected set; }

    public Vector3 lateralVelocity {
        get
        {
            return new Vector3(velocity.x, 0 ,velocity.z);
        }

        set{
            velocity = new Vector3(value.x,velocity.y,value.z);
        }
    }

    public Vector3 verticalVelocity
    {
        get
        {
            return new Vector3(0, velocity.y,0);
        }

        set
        {
            velocity = new Vector3(velocity.x, value.y, velocity.z);
        }
    }

    protected virtual void InitializeController()
    {
        controller = GetComponent<CharacterController>();

        if(!controller)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }

        controller.skinWidth = 0.005f;
        controller.minMoveDistance = 0;
    }

    protected virtual void Awake()
    {
        InitializeController();
        InitializeStateManager();
    }

    protected void Update()
    {
        HandleStates();
        HandleController();
    }

    protected virtual void HandleController()
    {
        if(controller.enabled)
        {
            controller.Move(velocity * Time.deltaTime);
        }
        transform.position += velocity * Time.deltaTime;
    }

    public virtual void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
    {
        if (direction.sqrMagnitude > 0)
        {
            var speed = Vector3.Dot(direction, lateralVelocity);
            var velocity = direction * speed;
            var turningVelocity = lateralVelocity - velocity;
            var turningDelta = turningDrag * turningDragMultiplier * Time.deltaTime;
            var targetTopSpeed = topSpeed * topSpeedMultiplier;

            if (lateralVelocity.magnitude < targetTopSpeed || speed < 0)
            {
                speed += acceleration * acclerationMultiplier * Time.deltaTime;
                speed = Mathf.Clamp(speed, -targetTopSpeed, targetTopSpeed);
            }

            velocity = direction * speed;
            turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
            lateralVelocity = velocity + turningVelocity;
        }
    }

    public virtual void FaceDirection(Vector3 direction, float degressPerSpeed)
    {
        if(direction !=  Vector3.zero)
        {
            var rotation = transform.rotation;
            var rotationDelta= degressPerSpeed * Time.deltaTime;
            var target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation =Quaternion.RotateTowards(rotation, target, rotationDelta);
        }
    }

    public virtual void Decelerate(float deceleration)
    {
        var delta=deceleration * decelerationMultiple * Time.deltaTime;
        lateralVelocity = Vector3.MoveTowards(lateralVelocity, Vector3.zero, delta);
    }

    public virtual void Gravity(float gravity)
    {
        if(!isGrounded)
        {
            verticalVelocity += Vector3.down * gravity * gravityMultiplier * Time.deltaTime;
        }
    }
}

