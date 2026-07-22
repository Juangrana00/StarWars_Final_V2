using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]

    public float walkSpeed;
    public float sprintSpeed;
    public Transform orientation;

    [Header("Ground Check")]

    public float playerHeight;
    public float groundDrag;
    public LayerMask groundLayer;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchYScale;

    [Header("Slope")]
    public float maxSlopeAngle;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Interaction")]
    public Transform interactorSource;
    public float interactRange;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private float _moveSpeed;
    private float _horizontalInput;
    private float _verticalInput;
    private float _startYScale;
    private bool _grounded;
    private bool _readyJump;
    private bool _crouching;
    private bool _exitingSlope;

    private Vector3 _moveDirection;
    private Rigidbody _rb;
    private RaycastHit _slopeHit;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _readyJump = true;
        _startYScale = transform.localScale.y;
    }

    private void Update()
    {
        MyInput();
        StateHandler();
        Interactor();

        _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        if(_grounded)
        {
            _rb.drag = groundDrag;
        }
        else
        {
            _rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        SpeedControl();
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(jumpKey) && _readyJump && _grounded)
        {
            _readyJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            _crouching = true;
        }

        if(Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
            _crouching = false;
        }
    }

    private void MovePlayer()
    {
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (OnSlope() && !_exitingSlope)
        {
            _rb.AddForce(GetSlopeMoveDirection(_moveDirection) * _moveSpeed * 20f, ForceMode.Force);

            if (_rb.velocity.y > 0)
            {
                _rb.AddForce(Vector3.down * 1, ForceMode.Force);
            }
        }
        else if (_grounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        }
        else if(!_grounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        _rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if(OnSlope() && !_exitingSlope)
        {
            if(_rb.velocity.magnitude > _moveSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _moveSpeed;
            }
        }
        else
        {
            Vector3 flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            if (flatVelocity.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVelocity.normalized * _moveSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        _exitingSlope = true;
    }

    private void ResetJump()
    {
        _readyJump = true;
        _exitingSlope = false;
    }

    private void StateHandler()
    {
        if(Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            _moveSpeed = crouchSpeed;
        }

        if (_grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            _moveSpeed = sprintSpeed;
        }
        else if(_grounded && !_crouching)
        {
            state = MovementState.walking;
            _moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

    private void Interactor()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(interactorSource.position, interactorSource.forward);

            if(Physics.Raycast(ray, out RaycastHit hitInfo, interactRange))
            {
                if(hitInfo.collider.gameObject.TryGetComponent(out IInteract interactable))
                {
                    interactable.Interact();
                }
            }
        }
    }
}
