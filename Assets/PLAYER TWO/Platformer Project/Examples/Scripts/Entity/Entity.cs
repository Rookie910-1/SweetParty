using UnityEngine;
using UnityEngine.UIElements;

public class Entity : MonoBehaviour
{
    public EntityEvents entityEvents;
    public bool isGrounded { get; protected set; } = true;

    public Vector3 velocity { get; set; }

    public float turningDragMultiplier { get; set; } = 1f;

    public float topSpeedMultiplier { get; set; } = 1f;

    public float acclerationMultiplier { get; set; } = 1f;

    public float decelerationMultiple { get; set; } = 1f;

    public float gravityMultiplier { get; set; } = 1f;

    public CharacterController controller { get; protected set; }

    public float lastGroundTime { get; protected set; }

    protected readonly float m_groundOffset = 0.1f;

    public RaycastHit groundHit { get; protected set; }

    public Vector3 lateralVelocity
    {
        get
        {
            return new Vector3(velocity.x, 0, velocity.z);
        }

        set
        {
            velocity = new Vector3(value.x, velocity.y, value.z);
        }
    }

    public Vector3 verticalVelocity
    {
        get
        {
            return new Vector3(0, velocity.y, 0);
        }

        set
        {
            velocity = new Vector3(velocity.x, value.y, velocity.z);
        }
    }

    public float height =>controller.height;

    public float radius => controller.radius;

    public Vector3 center=>controller.center;

    public Vector3 position => transform.position + center;

    public virtual bool Spherecast(Vector3 direction,float distance,out RaycastHit hit
        ,int layer=Physics.DefaultRaycastLayers
        ,QueryTriggerInteraction queryTriggerInteraction=QueryTriggerInteraction.Ignore)
    {
        var castDistance = Mathf.Abs(distance - radius);
        return Physics.SphereCast(position, radius, direction,out hit, castDistance, layer, queryTriggerInteraction);
    }

    public Vector3 stepPosition => position - transform.up * (height * 0.5f - controller.stepOffset);

    public virtual bool isPointUnderStep(Vector3 point) => stepPosition.y >point.y;
}
public class Entity<T> : Entity where T : Entity<T>
{
    public EntityStateManager<T> states {  get; private set; }
    protected void HandleStates() => states.Step();
    protected virtual void InitializeStateManager() => states= GetComponent<EntityStateManager<T>>();

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

    protected virtual void Update()
    {
        if (controller.enabled)
        {
            HandleStates();
            HandleController();
            HandleGround();
        }
    }

    protected virtual void HandleGround()
    {
        var distance = (height * 0.5f) + m_groundOffset;
        if(Spherecast(Vector3.down,distance,out var hit) && velocity.y <= 0)
        {
            if(!isGrounded)
            {
                if(EvaluateLanding(hit))
                {
                    EnterGround(hit);
                    
                }
                else
                {
                    HandleHighLedge(hit);
                }
            }
        }
        else
        {
            ExitGround();
        }
    }

    protected virtual void HandleController()
    {
        if(controller.enabled)
        {
            controller.Move(velocity * Time.deltaTime);
        }
        transform.position += velocity * Time.deltaTime;
    }

    protected virtual void HandleHighLedge(RaycastHit hit)
    {

    }

    protected virtual void EnterGround(RaycastHit hit)
    {
        if(!isGrounded)
        {
            groundHit = hit;
            isGrounded = true;
            entityEvents.onGroundEnter?.Invoke();
        }
    }

    protected virtual void ExitGround()
    {
        if(isGrounded)
        {
            isGrounded = false;
            transform.parent = null;
            lastGroundTime = Time.time;
            verticalVelocity = Vector3.Max(verticalVelocity, Vector3.zero);
            entityEvents.onGroundExit?.Invoke();
        }
    }

    protected virtual bool EvaluateLanding(RaycastHit hit)
    {
        return isPointUnderStep(hit.point) && Vector3.Angle(hit.normal,Vector3.up) < controller.slopeLimit;
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

