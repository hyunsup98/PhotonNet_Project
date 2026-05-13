
using UnityEngine;

public class WalkState : MonoBehaviour, IState
{
    private PlayerController _playerController;

    public WalkState(PlayerController playerController)
    {
        _playerController = playerController;
    }   

    public void Enter()
    {
        _playerController.Animator.SetFloat("Speed", 0.5f);
    }

    public void Exit()
    {
        
    }

    public void FixedTick()
    {
        Vector3 targetVelocity = 
            _playerController.transform.forward * _playerController.moveDir.z +
            _playerController.transform.right * _playerController.moveDir.x;
        targetVelocity.Normalize();

        _playerController.Rigidbody.linearVelocity = new Vector3(
            targetVelocity.x * _playerController._walkSpeed,
            _playerController.Rigidbody.linearVelocity.y,
            targetVelocity.z * _playerController._walkSpeed
        );
    }

    public void Tick()
    {
        if(_playerController.moveDir.magnitude <= 0f)
        {
            _playerController.ChangeState(_playerController._idleState);
        }
    }
}
