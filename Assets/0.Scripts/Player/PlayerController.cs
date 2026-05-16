using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviourPun
{
    public PlayerInputHandler PlayerInput { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public CameraController CameraController { get; private set; }

    #region 상태
    public IState _currentState;

    public IState _idleState;
    public IState _walkState;
    #endregion

    #region 변수
    [Header("이동 관련 속도")]
    public bool _isSprint;
    public float _walkSpeed = 3f;

    public Vector3 moveDir = Vector3.zero;
    #endregion

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInputHandler>();
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        CameraController = GetComponent<CameraController>();

        _idleState = new IdleState(this);
        _walkState = new WalkState(this);

        ChangeState(_idleState);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        _currentState?.Tick();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        _currentState?.FixedTick();
    }

    public void ChangeState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
}
