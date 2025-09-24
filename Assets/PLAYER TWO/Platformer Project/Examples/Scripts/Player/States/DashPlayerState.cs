
using UnityEngine;

/// <summary>
/// 冲刺状态
/// 进入时给一个强烈的向前推力
/// 在持续时间内维持冲刺运动
/// </summary>
public class DashPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.verticalVelocity = Vector3.zero;
        player.lateralVelocity = player.transform.forward * player.stats.current.dashForce;
        player.playerEvents.OnDashStart?.Invoke();
    }

    protected override void OnExit(Player player)
    {
         player.lateralVelocity=Vector3.ClampMagnitude(player.lateralVelocity,player.stats.current.topSpeed);
         player.playerEvents.OnDashStop?.Invoke();
    }

    protected override void OnStep(Player player)
    {
        player.Jump();
        if (timeSinceEntered > player.stats.current.dashDuration)
        {
            if(player.isGrounded)
                player.states.Change<WalkPlayerState>();
            else
                player.states.Change<FallPlayerState>();
        }
    }

    public override void OnContact(Player entity, Collider other)
    {
    
    }
}
