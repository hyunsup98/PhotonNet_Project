using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviourPun
{
    private PlayerInputHandler _playerInput;
    
    #region 상태
    private IState _idleState;
    private IState _walkState;
    #endregion

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInputHandler>();

        _idleState = new IdleState(this);
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
