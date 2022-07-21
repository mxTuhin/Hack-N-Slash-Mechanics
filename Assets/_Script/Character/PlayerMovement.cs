using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;
     
    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private bool _isGrounded = true;
    private Transform _groundChecker;

    public PlayerControlAction playerControl;

    private InputAction move;
    private InputAction attack;
    private InputAction jump;
    private InputAction dash;

    private void Awake()
    {
        playerControl = new PlayerControlAction();
    }

    private void OnEnable()
    {
        move = playerControl.Player.Move;
        move.Enable();

        attack = playerControl.Player.Attack;
        attack.Enable();
        attack.performed += Attack;

        jump = playerControl.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        dash = playerControl.Player.Dash;
        dash.Enable();
        dash.performed += Dash;
    }

    private void OnDisable()
    {
        move.Disable();
        attack.Disable();
        jump.Disable();
    }

    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _groundChecker = transform.GetChild(0);
    }
     
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
     
     
        _inputs = Vector3.zero;
        _inputs.x = move.ReadValue<Vector2>().x;
        _inputs.z = move.ReadValue<Vector2>().y;
        if (_inputs != Vector3.zero)
            transform.forward = _inputs;
    }
     
     
    void FixedUpdate()
    {
        _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        print("Attack Triggered");
    }
    
    private void Jump(InputAction.CallbackContext context)
    {
        _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
    }
    
    private void Dash(InputAction.CallbackContext context)
    {
        Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime)));
        _body.AddForce(dashVelocity, ForceMode.VelocityChange);
    }
}
