using UnityEngine;

public class Slide
{
    //External Vars
    private Transform _playerOrientation;
    private Transform _playerTransform;
    private Rigidbody _rigidbody;
    private float _maxSlideTime;
    private float _slideForce;
    private float _slideYScale;
    private float _startYScale;
    private float _slideImpulse = 5f;
    private float _slideParameter = -0.1f;

    //Internal Vars
    private float _slideTimer;
    private bool _isSliding;

    public bool isSliding { get { return _isSliding; } }

    public Slide(Transform playerOrientation, Transform playerTransform, Rigidbody rigidbody, float maxSlideTime, float slideForce, float slideYScale,float startYScale, float slideImpulse, float slideParameter)
    {
        _playerOrientation = playerOrientation;
        _playerTransform = playerTransform;
        _rigidbody = rigidbody;
        _maxSlideTime = maxSlideTime;
        _slideForce = slideForce;
        _slideYScale = slideYScale;
        _startYScale = startYScale;
        _slideImpulse = slideImpulse;
        _slideParameter = slideParameter;
    }

    public void SlideStart()
    {
        _isSliding = true;
        _playerTransform.localScale = new Vector3(_playerTransform.localScale.x, _slideYScale, _playerTransform.localScale.z);
        _rigidbody.AddForce(Vector3.down * _slideImpulse, ForceMode.Impulse);
        _slideTimer = _maxSlideTime;
    }

    public void SlidingMovement(Vector3 inputDirection, bool slope, Vector3 slopeDirection)
    {
        if(!slope || _rigidbody.velocity.y > _slideParameter)
        {
            _rigidbody.AddForce(inputDirection.normalized * _slideForce, ForceMode.Force);
            _slideTimer -= Time.deltaTime;
        }
        else
        {
            _rigidbody.AddForce(slopeDirection * _slideForce, ForceMode.Force);
        }

        if(_slideTimer <= 0)
        {
            SlideStop();
        }
    }

    public void SlideStop()
    {
        _isSliding = false;
        //_playerTransform.localScale = new Vector3(_playerTransform.localScale.x, _startYScale, _playerTransform.localScale.z);
    }

    public Vector3 InputDirection (float horizontal,  float vertical)
    {
        return _playerOrientation.forward * vertical + _playerOrientation.right * horizontal;
    }
}
