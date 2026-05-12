using UnityEngine;

public class RunState : IState
{
    private PlayerController _playerController;

    public RunState(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void Enter()
    {
        _playerController.Animator.SetFloat("Speed", 1f);
    }

    public void Exit()
    {
        
    }

    public void FixedTick()
    {
    }

    public void Tick()
    {
    }
}
