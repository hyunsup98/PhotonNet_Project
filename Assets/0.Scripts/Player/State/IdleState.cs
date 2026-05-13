using Photon.Pun.Demo.SlotRacer;
using UnityEngine;

public class IdleState : IState
{
    private PlayerController _playerController;

    public IdleState(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void Enter()
    {
        _playerController.Animator.SetFloat("Speed", 0f);
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
        if(_playerController.moveDir.sqrMagnitude > 0.1f)
        {
            _playerController.ChangeState(_playerController._walkState);
            Debug.Log(_playerController._currentState);
        }
    }

    public void FixedTick()
    {
        
    }
}
