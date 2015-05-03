using UnityEngine;
using System.Collections;

public class LegacyThirdPersonController : MonoBehaviour
{
    #region AnimationClips
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip runAnimation;
    public AnimationClip jumpPoseAnimation;
    public AnimationClip fallPoseAnimation;
    public AnimationClip attackPoseAnimation;
    private Animation animation;

    public float walkMaxAnimationSpeed = 0.75f;
    public float trotMaxAnimationSpeed = 1.0f;
    public float runMaxAnimationSpeed = 1.0f;
    public float jumpAnimationSpeed = 1.15f;
    public float fallAnimationSpeed = 1.0f;
    public float attackAnimationSpeed = 1.0f;
    #endregion

    public Transform buik;//what

    #region Movement variables
    // The speed when walking
    public float walkSpeed = 2.0f;
    // after trotAfterSeconds of walking we trot with trotSpeed
    public float trotSpeed = 4.0f;
    // when pressing "Fire3" button (cmd) we start running
    public float runSpeed = 6.0f;
    public float inAirControlAcceleration = 3.0f;
    public bool canJump = true;
    // How high do we jump when pressing jump and letting go immediately
    public float jumpHeight = 0.5f;
    // The gravity for the character
    public float gravity = 20.0f;
    // The gravity in controlled descent mode
    public float speedSmoothing = 10.0f;
    public float rotateSpeed = 500.0f;
    public float trotAfterSeconds = 3.0f;
    private float jumpRepeatTime = 0.05f;
    private float jumpTimeout = 0.15f;
    private float groundedTimeout = 0.25f;
    #endregion

    // The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
    private float lockCameraTimer = 0.0f;
    private CharacterMovementState state;
    public bool isControllable = false;

    void Awake()
    {
        state = new CharacterMovementState();
        state.moveDirection = transform.TransformDirection(Vector3.forward);

        animation = GetComponent<Animation>();
        if (!animation)
        {
            Debug.Log("The character you would like to control doesn't have animations.");
        }
        if (!idleAnimation)
        {
            animation = null;
            Debug.Log("No idle animation found. Turning off animations.");
        }
        if (!walkAnimation)
        {
            animation = null;
            Debug.Log("No walk animation found. Turning off animations.");
        }
        if (!runAnimation)
        {
            animation = null;
            Debug.Log("No run animation found. Turning off animations.");
        }
        if (!jumpPoseAnimation && canJump)
        {
            animation = null;
            Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
        }
    }

    void UpdateSmoothedMovementDirection()
    {
        Transform cameraTransform = Camera.main.transform;
        bool grounded = IsGrounded();

        // Forward vector relative to the camera along the x-z plane	
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        // Right vector relative to the camera
        // Always orthogonal to the forward vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float v=0;
        float h=0;
        if (isControllable)
        {
            v = Input.GetAxisRaw("Vertical");
            h = Input.GetAxisRaw("Horizontal");
        }

        // Are we moving backwards or looking backwards
        if (v < -0.2)
        {
            state.movingBack = true;
        }
        else
        {
            state.movingBack = false;
        }

        bool wasMoving = state.isMoving;
        state.isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;

        // Target direction relative to the camera
        Vector3 targetDirection = h * right + v * forward;

        // Grounded controls
        if (grounded)
        {
            // Lock camera for short period when transitioning moving & standing still
            lockCameraTimer += Time.deltaTime;
            if (state.isMoving != wasMoving)
                lockCameraTimer = 0.0f;

            // We store speed and direction seperately,
            // so that when the character stands still we still have a valid forward direction
            // moveDirection is always normalized, and we only update it if there is user input.
            if (targetDirection != Vector3.zero)
            {
                // If we are really slow, just snap to the target direction
                if (state.moveSpeed < walkSpeed * 0.9 && grounded)
                {
                    state.moveDirection = targetDirection.normalized;
                }
                // Otherwise smoothly turn towards it
                else
                {
                    state.moveDirection = Vector3.RotateTowards(state.moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);

                    state.moveDirection = state.moveDirection.normalized;
                }
            }

            // Smooth the speed based on the current target direction
            float curSmooth = speedSmoothing * Time.deltaTime;

            // Choose target speed
            //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
            float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

            state.currentState = CharacterState.Idle;

            // Pick speed modifier
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                targetSpeed *= runSpeed;
                state.currentState = CharacterState.Running;
            }
            else if (Time.time - trotAfterSeconds > state.walkTimeStart)
            {
                targetSpeed *= trotSpeed;
                state.currentState = CharacterState.Trotting;
            }
            else
            {
                targetSpeed *= walkSpeed;
                state.currentState = CharacterState.Walking;
            }

            state.moveSpeed = Mathf.Lerp(state.moveSpeed, targetSpeed, curSmooth);

            // Reset walk time start when we slow down
            if (state.moveSpeed < walkSpeed * 0.3f)
                state.walkTimeStart = Time.time;
        }
        // In air controls
        else
        {
            // Lock camera while in air
            if (state.jumping)
            {
                lockCameraTimer = 0.0f;
            }

            if (state.isMoving)
            {
                state.inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
            }
        }
    }

    void ApplyJumping()
    {
        // Prevent jumping too fast after each other
        if (state.lastJumpTime + jumpRepeatTime > Time.time)
        {
            return;
        }

        if (IsGrounded())
        {
            // Jump
            // - Only when pressing the button down
            // - With a timeout so you can press the button slightly before landing		
            if (canJump && Time.time < state.lastJumpButtonTime + jumpTimeout)
            {
                state.verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
                SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void ApplyGravity()
    {
        if (isControllable)	// don't move player at all if not controllable.
        {
            // Apply gravity
            bool jumpButton = Input.GetButton("Jump");

            // When we reach the apex of the jump we send out a message
            if (state.jumping && !state.jumpingReachedApex && state.verticalSpeed <= 0.0)
            {
                state.jumpingReachedApex = true;
                SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
            }

            if (IsGrounded())
            {
                state.verticalSpeed = 0.0f;
            }
            else
            {
                state.verticalSpeed -= gravity * Time.deltaTime;
            }
        }
    }

    float CalculateJumpVerticalSpeed (float targetJumpHeight)
    {
	    // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
	    return Mathf.Sqrt(2 * targetJumpHeight * gravity);
    }

    void DidJump()
    {
        state.jumping = true;
        state.jumpingReachedApex = false;
        state.lastJumpTime = Time.time;
        state.lastJumpStartHeight = transform.position.y;
        state.lastJumpButtonTime = -10;

        state.currentState = CharacterState.Jumping;
    }

    void Update()
    {
        if (isControllable && Input.GetButtonDown("Jump"))
        {
            state.lastJumpButtonTime = Time.time;
        }

        UpdateSmoothedMovementDirection();

        // Apply gravity
        // - extra power jump modifies gravity
        // - controlledDescent mode modifies gravity
        ApplyGravity ();

        // Apply jumping logic
        ApplyJumping ();
	
        // Calculate actual motion
        var movement = state.moveDirection * state.moveSpeed + new Vector3(0, state.verticalSpeed, 0) + state.inAirVelocity;
        movement *= Time.deltaTime;
	
        // Move the controller
        CharacterController controller  = GetComponent<CharacterController>();
        state.collisionFlags = controller.Move(movement);
	
        // ANIMATION sector
        if(animation) 
        {
            if(state.currentState == CharacterState.Jumping) 
            {
                if(!state.jumpingReachedApex) {
                    animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
                    animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
                    animation.CrossFade(jumpPoseAnimation.name);
                } 
                else 
                {
                    animation[jumpPoseAnimation.name].speed = -fallAnimationSpeed;
                    animation[fallPoseAnimation.name].speed = fallAnimationSpeed;
                    animation.CrossFade(fallPoseAnimation.name, 0.1f);		
                }
            } 
            else 
            {
                if (this.isControllable && controller.velocity.sqrMagnitude < 0.5)
                {
                    animation.CrossFade(idleAnimation.name);
                    state.currentState = CharacterState.Idle;
                }
                else
                {
                    switch(state.currentState)
                    {
                        case CharacterState.Idle:
                            animation.CrossFade(idleAnimation.name);
                            break;
                        case CharacterState.Running:
                            animation[runAnimation.name].speed = runMaxAnimationSpeed;
                            if (isControllable)
                            {
                                animation[runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
                            }
                            animation.CrossFade(runAnimation.name);
                            break;
                        case CharacterState.Trotting:
                            animation[runAnimation.name].speed = trotMaxAnimationSpeed;
                            if (isControllable)
                            {
                                animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, trotMaxAnimationSpeed);
                            }
                            animation.CrossFade(walkAnimation.name);
                            break;
                        case CharacterState.Walking:
                            animation[runAnimation.name].speed = walkMaxAnimationSpeed;
                            if (isControllable) 
                            {
                                animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
                            }
                            animation.CrossFade(walkAnimation.name);
                            break;
                    }
                }
                if (isControllable && Input.GetButton("Fire1"))
                {
                    animation[attackPoseAnimation.name].AddMixingTransform(buik);
                    animation.CrossFade(attackPoseAnimation.name,0.2f);
                    animation.CrossFadeQueued(idleAnimation.name,1.0f);
                    //((GetComponent.<Animation>() as Animation)[attackPoseAnimation.name] as AnimationClip).AddMixingTransform(buik);
                    //GetComponent.<Animation>().CrossFade(attackPoseAnimation.name, 0.2);
                    //GetComponent.<Animation>().CrossFadeQueued(idleAnimation.name, 1.0);
                }
            }
            // Set rotation to the move direction
            if (IsGrounded())
            {
                transform.rotation = Quaternion.LookRotation(state.moveDirection);
            }	
            else
            {
                Vector3 xzMove = movement;
                xzMove.y = 0;
                if (xzMove.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(xzMove);
                }
            }	
	
            // We are in jump mode but just became grounded
            if (IsGrounded())
            {
                state.lastGroundedTime = Time.time;
                state.inAirVelocity = Vector3.zero;
                if (state.jumping)
                {
                    state.jumping = false;
                    SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
    void OnControllerColliderHit(ControllerColliderHit hit )
    {
    //	Debug.DrawRay(hit.point, hit.normal);
        if (hit.moveDirection.y > 0.01)
        {
            return;
        }
    }
    bool IsMoving ()
    {
	    return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
    }
    bool IsGrounded()
    {
        return (state.collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

}
public enum CharacterState { Idle = 0, Walking = 1, Trotting = 2, Running = 3, Jumping = 4, }

public class CharacterMovementState
{
    public CharacterState currentState;

    public Vector3 moveDirection = Vector3.zero;
    public float verticalSpeed = 0.0f;
    public float moveSpeed = 0.0f;

    // The last collision flags returned from controller.Move
    public CollisionFlags collisionFlags;

    public bool jumping = false;
    public bool jumpingReachedApex = false;

    public bool movingBack = false;
    public bool isMoving = false;
    public float walkTimeStart = 0.0f;
    public float lastJumpButtonTime = -10.0f;
    public float lastJumpTime = -1.0f;

    public float lastJumpStartHeight = 0.0f;
    public Vector3 inAirVelocity = Vector3.zero;
    public float lastGroundedTime = 0.0f;
}
