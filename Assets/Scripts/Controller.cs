using System.Collections;
using UnityEngine;

public class Controller
{
    //Movement
    private Model _model;
    private KeyCode _sprintKey, _jumpKey, _crouchKey, _interactKey, _shootKey, _reloadKey;
    private float _horizontalInput, _verticalInput;
    private float _walkSpeed, _sprintSpeed, _crouchSpeed;

    //Slide
    private float _slideCooldown;
    private bool _canSlide;

    //Gun
    private bool _shooting, _allowButtonHold;

    public Controller(Model model, KeyCode sprintKey, KeyCode jumpKey, KeyCode crouchKey, KeyCode interactKey, KeyCode shootKey, KeyCode reloadKey, float walkSpeed, float sprintSpeed, float crouchSpeed, float slideCooldown, bool allowButtonHold)
    {
        _model = model;
        _sprintKey = sprintKey;
        _jumpKey = jumpKey;
        _crouchKey = crouchKey;
        _interactKey = interactKey;
        _shootKey = shootKey;
        _reloadKey = reloadKey;
        _walkSpeed = walkSpeed;
        _sprintSpeed = sprintSpeed;
        _crouchSpeed = crouchSpeed;
        _slideCooldown = slideCooldown;
        _allowButtonHold = allowButtonHold;
    }

    public void ArtificialUpdate()
    {
        MyInput();
        SpeedHandler();
        _model.GroundedValue();
        _model.DragValue();
        _model.GunText();
        _model.GunOut();
    }

    public void ArtificialFixedUpdate()
    {
        _model.MovePlayer(_horizontalInput, _verticalInput);
        _model.SpeedControl();
        _model.Sliding(_horizontalInput, _verticalInput);
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(_jumpKey))
        {
            _model.JumpStart();
        }

        if(Input.GetKeyDown(_crouchKey))
        {
            if((_horizontalInput != 0 ||  _verticalInput != 0) && _canSlide)
            {
                _model.SlideValue(true);
            }

            _model.Crouch();
        }

        if(Input.GetKeyUp(_crouchKey))
        {
            if(_model.isSliding)
            {
                _model.SlideValue(false);
            }

            _model.NoCrouch();
        }

        if(Input.GetKeyDown(_interactKey))
        {
            _model.InteractionRay();
        }

        if(_allowButtonHold)
        {
            _shooting = Input.GetKey(_shootKey);
        }
        else
        {
            _shooting = Input.GetKeyDown(_shootKey);
        }

        if(_shooting)
        {
            _model.CanShootOrRelaod(true);
        }

        if(Input.GetKeyDown (_reloadKey))
        {
            _model.CanShootOrRelaod(false);
        }
    }

    private void SpeedHandler()
    {
        if(Input.GetKey(_crouchKey))
        {
            _model.SpeedChange(_crouchSpeed);
        }

        if(_model.isGrounded && Input.GetKey(_sprintKey) && !_model.isCrouching)
        {
            _model.SpeedChange(_sprintSpeed);
            _model.mb.StartCoroutine(TimeToSlide());
        }
        else if (_model.isGrounded && !_model.isCrouching)
        {
            _model.SpeedChange(_walkSpeed);
        }
    }

    private IEnumerator TimeToSlide()
    {
        _canSlide = true;
        yield return new WaitForSeconds(_slideCooldown);
        _canSlide = false;
    }
}
