using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviourPun
{
    public PlayerInputHandler PlayerInput { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    #region 상태
    private IState _currentState;

    private IState _idleState;
    private IState _walkState;
    #endregion

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInputHandler>();
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();

        _idleState = new IdleState(this);
        _walkState = new WalkState(this);
    }

    private void Update()
    {
        if(!photonView.IsMine) return;

        _currentState?.Tick();
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine) return;

        _currentState?.FixedTick();
    }
}
