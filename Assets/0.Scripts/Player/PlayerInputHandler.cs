using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviourPun
{
    private PlayerController _playerController;
    private PlayerInput _playerInput;
    
    private InputAction _moveAction;    // 이동 입력 액션
    private InputAction _jumpAction;    // 점프 입력 액션

    private void Awake()
    {
        if(!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        _playerController = GetComponent<PlayerController>();
        _playerInput = GetComponent<PlayerInput>();
    }

    // 이동 로직
    private void Move(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
    }
    
    // 점프 로직
    private void Jump(InputAction.CallbackContext context)
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
