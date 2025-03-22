using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInputAction;
    private InputAction _selectAction;
    private InputAction _fireAction;

    public event Action Fire;

    private void Awake()
    {
        if(_playerInputAction == null)
        {
            _playerInputAction = GetComponent<PlayerInput>();
        }
        _selectAction = _playerInputAction.actions["Select"];
        _fireAction = _playerInputAction.actions["Fire"];
    }

    public Vector2 GetSelectedPosition()
    {
        return _selectAction.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Fire?.Invoke();
        }
    }

}
