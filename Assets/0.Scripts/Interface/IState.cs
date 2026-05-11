public interface IState
{
    public void Enter();
    public void Exit();
    public void Tick();         // Update
    public void FixedTick();    // FixedUpdate
}