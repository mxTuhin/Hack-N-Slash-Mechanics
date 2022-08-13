using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Control Variables")]
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;
    Vector3 m_EulerAngleVelocity;
     
    [Header("Player Trackers")]
    public GameObject playerGUI;
    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private bool _isGrounded = true;
    private Transform _groundChecker;
    private Vector3 direction;
    

    [Header("Input System")]
    public PlayerControlAction playerControl;

    private InputAction move;
    private InputAction attack;
    private InputAction jump;
    private InputAction dash;
    private InputAction mouseAxis;

    [Header("Camera System")] [SerializeField] private Vector2 turn;
    [SerializeField] private float sensitivity = 0.5f;
    private Vector3 deltaMove;
    [SerializeField] private float speed = 1;
    private Vector3 mousePos;

    private void Awake()
    {
        playerControl = new PlayerControlAction();
    }

    #region Initializers
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

        mouseAxis = playerControl.Player.Look;
        mouseAxis.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        attack.Disable();
        jump.Disable();
        mouseAxis.Disable();
    }

    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _groundChecker = transform.GetChild(0);
        m_EulerAngleVelocity = new Vector3(0, 100, 0);
        
    }
    
    #endregion
    
    #region RuntimeLoops
     
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        MovementInputModule();
        RotationWithMouseInputModule();
        // RotationWithMouseMovementModule();

    }
     
     
    void FixedUpdate()
    {
        
        _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
        
        
    }
    
    #endregion
    
    #region ControlMechanics

    private void MovementInputModule()
    {
        _inputs = Vector3.zero;
        _inputs.x = move.ReadValue<Vector2>().x;
        _inputs.z = move.ReadValue<Vector2>().y;
        if (_inputs != Vector3.zero)
        {
            transform.forward = _inputs;
        }
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
        StartCoroutine(disableCharacter(0.1f));
    }

    IEnumerator disableCharacter(float disableTimer)
    {
        playerGUI.SetActive(false);
        yield return new WaitForSeconds(disableTimer);
        playerGUI.SetActive(true);
    }
    
    #endregion
    
    #region MouseMechanics

    private void RotationWithMouseInputModule()
    {
        mousePos = Mouse.current.delta.ReadValue();
        print(mousePos);

        turn.x = mousePos.x;
        turn.y = mousePos.y;
    }

    public CinemachineFreeLook cineCam;
    private void RotationWithMouseMovementModule()
    {
        // transform.localRotation = Quaternion.Euler(0, turn.x, 0);
        if (mousePos.magnitude >= 0.1f)
        {
            _body.rotation = Quaternion.Euler(_body.rotation.eulerAngles + new Vector3(0f, turn.x, 0f));
        }
        
        // transform.localRotation = quaternion.Euler(-turn.y, 0, 0);
    }
    #endregion
}
