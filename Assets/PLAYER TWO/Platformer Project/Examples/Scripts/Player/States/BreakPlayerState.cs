
using UnityEngine;

public class BreakPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        
    }

    protected override void OnExit(Player player)
    {
        
    }

    protected override void OnStep(Player player)
    {
        var inputDirection = player.inputs.GetMovementCamerDirection();

        if(player.stats.current.canBackflip && Vector3.Dot(inputDirection,player.transform.forward) <0 && 
            player.inputs.GetJumpDown())
        {
            player.Backflip(player.stats.current.backflipBackwardTurnForece);
        }
        else
        {
            if(player.lateralVelocity.sqrMagnitude==0)
            {
                player.states.Change<IdelPlayerState>();
            }
        }

        player.Decelerate();

        if(player.lateralVelocity.sqrMagnitude ==0)
        {
            player.states.Change<IdelPlayerState>();
        }
    }
}

