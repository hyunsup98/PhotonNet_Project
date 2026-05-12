
public class WalkState : IState
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
        
    }

    public void Tick()
    {
        
    }
}
