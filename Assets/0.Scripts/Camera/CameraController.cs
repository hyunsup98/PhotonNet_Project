using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviourPun
{
    [SerializeField] private PlayerInput _playerInput;  // 마우스 입력을 받아올 인풋 액션
    [SerializeField] private Transform _cameraTransform;

    private InputAction _lookAction;

    [Header("카메라 감도 및 회전 제한")]
    [SerializeField] private float _mouseSensitivity = 1.5f;

    [SerializeField] private float _minVerticalAngle = -45f;
    [SerializeField] private float _maxVerticalAngle = 0f;

    private Vector2 lookInput;

    private float yRot = 0f;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            _cameraTransform.gameObject.SetActive(false);
        }

        _lookAction = _playerInput.actions["Look"];
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        
        float x = lookInput.x * _mouseSensitivity * Time.deltaTime;
        float y = lookInput.y * _mouseSensitivity * Time.deltaTime;

        yRot += y;

        yRot = Mathf.Clamp(yRot, _minVerticalAngle, _maxVerticalAngle);

        transform.Rotate(Vector3.up * x);
        _cameraTransform.localEulerAngles = new Vector3(yRot, 0, 0);
    }

    private void Look(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            lookInput = ctx.ReadValue<Vector2>();
        else if (ctx.canceled)
            lookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        if (!photonView.IsMine) return;

        _lookAction.performed += Look;
        _lookAction.canceled += Look;
    }

    private void OnDisable()
    {
        if (!photonView.IsMine) return;

        _lookAction.performed -= Look;
        _lookAction.canceled -= Look;
    }
}
