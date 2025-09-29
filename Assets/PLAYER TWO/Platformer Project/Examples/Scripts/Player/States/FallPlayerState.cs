
using UnityEngine;

public class FallPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {

    }

    protected override void OnExit(Player player)
    {

    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        //平滑转向（使角色朝向移动方向）
        player.FaceDirectionSmooth(player.lateralVelocity);
        player.AccelerateToInputDirection();
        player.Jump();
        player.Spin();
        player.StompAttack();
        player.Dash();
        player.AirDive();
        player.Glide();
        
        if(player.isGrounded)
        {
            player.states.Change<IdelPlayerState>();
        }
    }

    public override void OnContact(Player entity, Collider other) { }
}

