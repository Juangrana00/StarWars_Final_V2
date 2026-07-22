using System.Collections;
using UnityEngine;

public class Model
{
    //References
    public MonoBehaviour mb;

    //Movement
    private Rigidbody _rb;
    private Transform _playerTransform;
    private Transform _playerOrientation;
    private Vector3 _moveDirection;
    private float _moveSpeed;
    private float _moveImpulse = 10f;

    //Jump
    private bool _readyToJump;
    private bool _isGrounded;
    private float _airMultiplier;
    private float _jumpCooldown;
    private float _jumpForce;
    private float _playerHeight;
    private float _groundDistance = 0.2f;
    private float _groundDrag;

    //Crouch
    private bool _isCrouching;
    private float _startYScale;
    private float _crouchYScale;
    private float _crouchImpulse = 5f;

    //Slope
    private RaycastHit _slopeHit;
    private bool _exitingSlope;
    private float _slopeDistance = 0.3f;
    private float _maxSlopeAngle;
    private float _slopeForce = 20f;
    private float _slopeResistance = 2f;

    //Interaction
    private Transform _interactorSource;
    private float _interactorRange;

    //Slide
    private Slide _slide;

    //Gun
    private Gun _gun;

    public bool isGrounded { get { return _isGrounded; } }

    public bool isCrouching { get { return _isCrouching; } }

    public bool isSliding { get { return _slide.isSliding; } }

    public Model(MonoBehaviour mb, Rigidbody rb, Transform transform, Transform orientation, float moveImpulse, float airMultiplier, float jumpCooldown, float jumpForce, float playerHeight, float groundDrag, float groundDistance, float crouchYScale, float crouchImpulse, float maxSlopeAngle, float slopeDistance, float slopeForce, float slopeResistance, Transform interactorSource, float interactorRange, Slide slide, Gun gun)
    {
        this.mb = mb;
        _rb = rb;
        _playerTransform = transform;
        _playerOrientation = orientation;
        _moveImpulse = moveImpulse;
        _airMultiplier = airMultiplier;
        _jumpCooldown = jumpCooldown;
        _jumpForce = jumpForce;
        _playerHeight = playerHeight;
        _groundDrag = groundDrag;
        _groundDistance = groundDistance;
        _crouchYScale = crouchYScale;
        _maxSlopeAngle = maxSlopeAngle;
        _slopeDistance = slopeDistance;
        _slopeForce = slopeForce;
        _slopeResistance = slopeResistance;
        _interactorSource = interactorSource;
        _interactorRange = interactorRange;
        _slide = slide;
        _gun = gun;
    }

    public void ModelStart()
    {
        _rb.freezeRotation = true;
        _readyToJump = true;
        _startYScale = _playerTransform.localScale.y;
        _gun.GunStart();
    }

    public void GroundedValue()
    {
        _isGrounded = Physics.Raycast(_playerTransform.position, Vector3.down, _playerHeight * 0.5f + _groundDistance);
    }

    public void DragValue()
    {
        if(_isGrounded)
        {
            _rb.drag = _groundDrag;
        }
        else
        {
            _rb.drag = 0;
        }
    }

    public void JumpStart()
    {
        if (_readyToJump && _isGrounded)
        {
            _readyToJump = false;
            Jump();
            mb.StartCoroutine(ResetJump());
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(_playerTransform.up * _jumpForce, ForceMode.Impulse);
        _exitingSlope = true;
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(_jumpCooldown);
        _readyToJump = true;
        _exitingSlope = false;
    }

    public void Crouch()
    {
        _playerTransform.localScale = new Vector3(_playerTransform.localScale.x, _crouchYScale, _playerTransform.localScale.z);
        _rb.AddForce(Vector3.down * _crouchImpulse, ForceMode.Impulse);
        _isCrouching = true;
    }

    public void NoCrouch()
    {
        _playerTransform.localScale = new Vector3(_playerTransform.localScale.x, _startYScale, _playerTransform.localScale.z);
        _isCrouching = false;
    }

    public void MovePlayer(float horizontal,  float vertical)
    {
        _moveDirection = _playerOrientation.forward * vertical + _playerOrientation.right * horizontal;

        if(OnSlope() && !_exitingSlope)
        {
            _rb.AddForce(GetSlopeMoveDirection(_moveDirection) * _moveSpeed * _slopeForce, ForceMode.Force);

            if(_rb.velocity.y > 0)
            {
                _rb.AddForce(Vector3.down * _slopeResistance, ForceMode.Force);
            }
        }
        else if(_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * _moveImpulse, ForceMode.Force);
        }
        else if(!_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * _moveImpulse * _airMultiplier, ForceMode.Force);
        }

        _rb.useGravity = !OnSlope();
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(_playerTransform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + _slopeDistance))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

    public void SpeedControl()
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

            if(flatVelocity.magnitude > _moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * _moveSpeed;
                _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);
            }
        }
    }

    public void SpeedChange(float newSpeed)
    {
        _moveSpeed = newSpeed;
    }

    public void InteractionRay()
    {
        Ray ray = new Ray(_interactorSource.position, _interactorSource.forward);

        if(Physics.Raycast(ray, out RaycastHit hitInfo, _interactorRange))
        {
            if(hitInfo.collider.gameObject.TryGetComponent(out IInteract interactable))
            {
                interactable.Interact();
            }
        }
    }

    public void Sliding(float horizontal, float vertical)
    {
        if(isSliding)
        {
            Vector3 inputDirection = _slide.InputDirection(horizontal, vertical);
            _slide.SlidingMovement(inputDirection, OnSlope(), GetSlopeMoveDirection(inputDirection));
        }
    }

    public void SlideValue(bool value)
    {
        if(value)
        {
            _slide.SlideStart();
        }
        else
        {
            _slide.SlideStop();
        }
    }

    public void CanShootOrRelaod(bool value)
    {
        if(value)
        {
            _gun.CanShoot();
        }
        else
        {
            _gun.CanReload();
        }
    }

    public void GunText()
    {
        _gun.SetText();
    }

    public void GunOut()
    {
        _gun.OutOfBullets();
    }
}
