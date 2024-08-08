using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/Relative FPS Movement")]
public class RelativeMovement : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _moveSpeed = 6.0f;
    [SerializeField] float _rotSpeed = 15.0f;
    [SerializeField] float _jumpSpeed = 15.0f;
    [SerializeField] float _gravity = -9.8f;
    [SerializeField] float _terminalVelocity = -20.0f;
    [SerializeField] float _minFall = -1.5f;
    [SerializeField] float _coyoteTime = 0.1f; // Coyote time duration
    [Header("Audio")]
    [SerializeField] private AudioSource bounceSound; // Assign this in the inspector
    [SerializeField] private AudioSource popSound; // Assign this in the inspector
    [SerializeField] private AudioSource jumpSound; // Assign this in the inspector

    private float _vertSpeed;
    private float _groundCheckDistance;
    private ControllerColliderHit _contact;
    private Vector3 _knockbackVelocity; // Store knockback velocity

    private CharacterController _charController;
    private Animator _anim;
    private bool _isJumping = false; // Track if the character is jumping
    private float _originalAnimSpeed; // Store the original animation speed
    private float _originalMoveSpeed; // Store the original move speed

    private float _coyoteTimeCounter; // Coyote time counter
    private bool _isGroundedLastFrame; // Track if the player was grounded in the previous frame

    private Coroutine _speedBoostCoroutine; // Reference to the active speed boost coroutine

    private void Start()
    {
        _vertSpeed = _minFall;
        _charController = GetComponent<CharacterController>();
        _charController.radius = 0.4f;
        _groundCheckDistance = (_charController.height + _charController.radius) / _charController.height * 0.11f;
        _anim = gameObject.GetComponentInChildren<Animator>();
        _originalAnimSpeed = _anim.speed; // Store the initial animation speed
        _originalMoveSpeed = _moveSpeed; // Store the initial move speed
    }

    private void Update()
    {
        Vector3 movement = Vector3.zero;
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        if (horInput != 0 || vertInput != 0)
        {
            Vector3 right = _target.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up);
            movement = (right * horInput) + (forward * vertInput);
            movement *= _moveSpeed;
            movement = Vector3.ClampMagnitude(movement, _moveSpeed);

            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, _rotSpeed * Time.deltaTime);
        }

        bool hitGround = false;
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
            hitGround = hit.distance <= _groundCheckDistance;

        // Update coyote time counter
        if (hitGround)
        {
            _coyoteTimeCounter = _coyoteTime;
            if (Input.GetButtonDown("Jump"))
            {
                if (!_isJumping) // Trigger jump only if not already jumping
                {
                    _vertSpeed = _jumpSpeed;
                    _anim.SetTrigger("JumpTrigger"); // Trigger the jump start animation
                    _isJumping = true;

                    // Play the jump sound when the player jumps
                    if (jumpSound != null)
                    {
                        jumpSound.PlayOneShot(jumpSound.clip);
                    }
                }
            }
            else
            {
                _vertSpeed = _minFall;
            }

            if (_isJumping && _charController.isGrounded) // Check if the character is grounded
            {
                _isJumping = false; // Reset jump state
            }
        }
        else
        {
            // Apply coyote time
            if (!_isGroundedLastFrame)
            {
                _coyoteTimeCounter -= Time.deltaTime;
            }

            if (_coyoteTimeCounter > 0 && Input.GetButtonDown("Jump"))
            {
                _vertSpeed = _jumpSpeed;
                _anim.SetTrigger("JumpTrigger"); // Trigger the jump start animation
                _coyoteTimeCounter = 0; // Reset coyote time counter after jumping

                // Play the jump sound when the player jumps during coyote time
                if (jumpSound != null)
                {
                    jumpSound.PlayOneShot(jumpSound.clip);
                }
            }
            
            _vertSpeed += _gravity * 5 * Time.deltaTime;

            if (_vertSpeed < _terminalVelocity)
                _vertSpeed = _terminalVelocity;

            if (_charController.isGrounded)
            {
                if (Vector3.Dot(movement, _contact.normal) < 0)
                    movement = _contact.normal * _moveSpeed;
                else
                    movement += _contact.normal * _moveSpeed;
            }
        }

        movement.y = _vertSpeed;
        movement += _knockbackVelocity; // Apply knockback if any
        _charController.Move(movement * Time.deltaTime);

        // Reduce knockback effect over time
        _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, 0.1f);

        // Set animation parameters
        if (_charController.isGrounded)
        {
            _anim.SetBool("IsJumping", false); // Set IsJumping to false when grounded
        }
        else
        {
            _anim.SetBool("IsJumping", true); // Set IsJumping to true when in the air
        }

        if (horInput != 0 || vertInput != 0)
        {
            _anim.SetInteger("AnimationPar", 1);
        }
        else
        {
            _anim.SetInteger("AnimationPar", 0);
        }

        _isGroundedLastFrame = _charController.isGrounded; // Update grounded status
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
        
        // Check if the player is colliding with a bounce platform
        BounceScript bounceScript = hit.gameObject.GetComponent<BounceScript>();
        if (bounceScript != null)
        {
            ApplyBounce(bounceScript.bounceForce);
            
            // Play the bounce sound with a random pitch
            if (bounceSound != null)
            {
                bounceSound.pitch = Random.Range(0.78f, 1.4f); // Set random pitch
                bounceSound.PlayOneShot(bounceSound.clip);
            }
        }

        BreakingPlatform breakingPlatform = hit.gameObject.GetComponent<BreakingPlatform>();
        if (breakingPlatform != null)
        {
            // Start the coroutine to deactivate and reactivate the platform
            StartCoroutine(breakingPlatform.DeactivateAndReactivateCoroutine());

            // Play the pop sound when colliding with the breaking platform
            if (popSound != null)
            {
                popSound.PlayOneShot(popSound.clip);
            }
        }
    }

    public void ApplyBounce(float bounceForce)
    {
        _vertSpeed = bounceForce; // Apply the bounce force
    }

    public IEnumerator SpeedBoost(float boostAmount, float duration)
    {
        _moveSpeed += boostAmount;
        _anim.speed += boostAmount / _originalMoveSpeed; // Adjust animation speed
        yield return new WaitForSeconds(duration);
        ResetSpeedBoost();
    }

    public void ApplyKnockback(Vector3 knockbackForce)
    {
        // Apply vertical component of knockback
        _vertSpeed = knockbackForce.y;

        // Apply horizontal knockback
        _knockbackVelocity += new Vector3(knockbackForce.x, 0, knockbackForce.z);
    }

    public void ResetSpeedBoost()
    {
        _moveSpeed = _originalMoveSpeed; // Reset move speed to original
        _anim.speed = _originalAnimSpeed; // Reset animation speed to original
        if (_speedBoostCoroutine != null)
        {
            StopCoroutine(_speedBoostCoroutine); // Stop the active speed boost coroutine
            _speedBoostCoroutine = null;
        }
    }

    public void StartSpeedBoost(float boostAmount, float duration)
    {
        if (_speedBoostCoroutine != null)
        {
            StopCoroutine(_speedBoostCoroutine); // Stop the active speed boost coroutine if any
        }
        _speedBoostCoroutine = StartCoroutine(SpeedBoost(boostAmount, duration));
    }
}
