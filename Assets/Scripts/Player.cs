using TMPro;
using UnityEngine;

public class Player : Entity
{
    [Header("Movement")]
    public Rigidbody rigidBody;
    public Transform orientation;
    public Camera mainCamera;
    [SerializeField] private CapsuleCollider _playerColl;
    public float walkSpeed, sprintSpeed, moveImpulse = 10;

    [Header("GroundCheck")]
    private float playerHeight;
    public float groundDrag, groundDistance = 0.2f;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown, airMultiplier;

    [Header("Crouch")]
    public float crouchYScale;
    public float crouchSpeed, crouchImpulse = 5;

    [Header("Slope")]
    public float maxSlopeAngle;
    public float slopeDistance = 0.3f, slopeForce = 20, slopeResistance = 4;

    [Header("Interaction")]
    public Transform interactorSource;
    public float interactionRange;

    [Header("Keys")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space, crouchKey = KeyCode.LeftControl, interactKey = KeyCode.E, shootKey, reloadKey = KeyCode.R;

    [Header("Slide")]
    public float maxSlideTime;
    public float slideForce, slideYScale, slideImpulse = 5, slideParameter = -0.1f, slideCooldown;

    [Header("Gun")]
    public GameObject bulletPrefab;
    public Transform attackPoint;
    public TextMeshProUGUI text;
    public LayerMask layerMask;
    public float damage, timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    private Model _model;
    private Controller _controller;
    private Slide _slide;
    private Gun _gun;

    private void Start()
    {
        playerHeight = _playerColl.height;
        shootKey = KeyCode.Mouse0;
        _slide = new Slide(orientation, transform, rigidBody, maxSlideTime, slideForce, slideYScale, transform.localScale.y, slideImpulse, slideParameter);
        _gun = new Gun(this, mainCamera, attackPoint, text, layerMask, damage, timeBetweenShooting, spread, range, reloadTime, timeBetweenShots, magazineSize, bulletsPerTap, true, Vector3.zero, bulletPrefab);
        _model = new Model(this, rigidBody, transform, orientation, moveImpulse, airMultiplier, jumpCooldown, jumpForce, playerHeight, groundDrag, groundDistance, crouchYScale, crouchImpulse, maxSlopeAngle, slopeDistance, slopeForce, slopeResistance, interactorSource, interactionRange, _slide, _gun);
        _controller = new Controller(_model, sprintKey, jumpKey, crouchKey, interactKey, shootKey, reloadKey, walkSpeed, sprintSpeed, crouchSpeed, slideCooldown, allowButtonHold);
        _model.ModelStart();
    }

    private void Update()
    {
        _controller.ArtificialUpdate();
    }

    private void FixedUpdate()
    {
        _controller.ArtificialFixedUpdate();
    }

    public override void TakeDamage(float damage)
    {
        Debug.Log("WARNING: PLAYER RECEIVED DAMAGE");
    }

    public override void Death()
    {
        Debug.Log("GAME OVER: PLAYER DIED");
    }

    
}
