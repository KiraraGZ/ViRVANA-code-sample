using Magia.Buddy;
using Magia.StateMachines;
using UnityEngine;

public class BuddyRepositionState : IState
{
    private BuddyMovementStateMachine stateMachine;
    private BuddyController buddy;
    private Vector3 direction;

    public BuddyRepositionState(BuddyMovementStateMachine movementStateMachine)
    {
        stateMachine = movementStateMachine;
        buddy = stateMachine.Buddy;
    }

    public void Enter()
    {

    }

    public void Exit() { }

    public void HandleInput() { }

    public void Update()
    {
        direction = (buddy.Player.transform.position - buddy.transform.position).normalized;

        if (Vector3.Distance(buddy.transform.position, buddy.Player.transform.position) <= 5f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public void PhysicsUpdate()
    {
        float repositionSpeed = buddy.Data.MovementData.BaseSpeed * buddy.Data.MovementData.RepositionSpeedModifier;
        buddy.Rigidbody.MovePosition(buddy.transform.position + direction * repositionSpeed * Time.fixedDeltaTime);
    }

    public void OnAnimationEnterEvent() { }
    public void OnAnimationEndEvent() { }
    public void OnAnimationTransitionEvent() { }
}
