using UnityEngine;

public class PlayerMovementSlide : MonoBehaviour
{
    [Header("PMS References")]
    public Transform orientation;
    public Transform playerObj;

    [Header("Slide")]
    public float maxSlideTime;
    public float slideForce;
    public float slideYScale;
    public KeyCode slideKey = KeyCode.LeftControl;

    private Rigidbody _rb;
    private PlayerMovement _pm;
    private float _slideTimer;
    private float _startYScale;
    private float _horizontalInput;
    private float _verticalInput;
    private bool _sliding;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _pm = GetComponent<PlayerMovement>();

        _startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (_horizontalInput != 0 || _verticalInput != 0))
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && _sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (_sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        _sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        _slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if(!_pm.OnSlope() || _rb.velocity.y > -0.1)
        {
            _rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            _slideTimer -= Time.deltaTime;
        }
        else
        {
            _rb.AddForce(_pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (_slideTimer <= 0)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        _sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, _startYScale, playerObj.localScale.z);
    }
}
