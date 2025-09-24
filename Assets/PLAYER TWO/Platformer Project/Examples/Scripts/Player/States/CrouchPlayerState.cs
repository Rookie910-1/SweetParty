
using UnityEngine;

public class CrouchPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.ResizeCollider(player.stats.current.crouchHeight);
    }

    protected override void OnExit(Player player)
    {
        player.ResizeCollider(player.originalHeight);
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        player.Fall();
        player.Decelerate(player.stats.current.crouchFriction);

        var inputDirection = player.inputs.GetMovementDirection();

        //玩家仍在按下下蹲或爬行键或遇到障碍物不能起身时
        if (player.inputs.GetCrouchAndCraw() || !player.canStandUp)
        {
            if (inputDirection.sqrMagnitude > 0 && !player.holding)
            {
                //爬行
                player.states.Change<CrawlingPlayerState>();
            }
        }//玩家下蹲时按下跳跃键->后空翻执行
        else if(player.inputs.GetJumpDown())
        {
            
        }
        else
        {
            player.states.Change<IdelPlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other) { }
}
