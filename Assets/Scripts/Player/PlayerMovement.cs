using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
//using UnityStandardAssets.Utility;
using System.Collections;

public class PlayerMovement : NetworkBehaviour {

	private Vector3 warpReset = new Vector3(9999, 9999, 9999);
    
    private MouseLook mouseLook;

    private Camera camera;

    public float topVelocity = 0.0f;
	public float jumpLength = 0.0f;
    public float currentVelocity;

    public float moveSpeed = 7.0f;  // Ground move speed
	public float slowDownSpeed = 10.0f; // De acceleration speed when we hit something
    public float runAcceleration = 14;   // Ground accel
    public float runDeacceleration = 10;   // Deacceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;  // Air accel
    public float airDeacceleration = 2.0f;    // Deacceleration experienced when opposite strafing
    public float airControl = 0.3f;  // How precise air control is
    public float sideStrafeAcceleration = 50;   // How fast acceleration occurs to get up to sideStrafeSpeed when side strafing
    public float sideStrafeSpeed = 1;    // What the max speed to generate when side strafing
    public float jumpSpeed = 8.0f;  // The speed at which the character's up axis gains when hitting jump
	public float moveScale = 1.0f;
	public float currentJumpLength = 0.0f;

    /* Frame occuring factors */
    public float gravity = 20.0f;
    public float friction = 6;  

    private bool wishJump;
	private bool sidesTouching;

    [HideInInspector]
    public Vector3 velocity = new Vector3();

	private Vector3 warpPosition;

    private CollisionFlags collisionFlags;
    private CharacterController controller;
    private PlayerHandler playerHander;
    private Animator animator;

    public void Start() {
		warpPosition = warpReset;
        controller = GetComponent<CharacterController>();
        playerHander = GetComponent<PlayerHandler>();
        mouseLook = GetComponent<MouseLook>();
        camera = GetComponentInChildren<Camera>();
        animator = GetComponent<Animator>();

        mouseLook.Init(transform, camera.transform);
    }

    private void QueueJump() {
        if(CrossPlatformInputManager.GetButtonDown("Jump") && !wishJump)
            wishJump = true;
        if(CrossPlatformInputManager.GetButtonUp("Jump"))
            wishJump = false;
    }

    private void RotateView() {
        mouseLook.LookRotation(transform, camera.transform);
    }

	public void Update() {
		RotateView();

		if(playerHander.IsDead) 
			return;

		sidesTouching = false;
		QueueJump();

		if (controller.isGrounded) {
			GroundMove ();
		} else if (!controller.isGrounded) {
			AirMove();
			currentJumpLength += velocity.magnitude;
		}

        collisionFlags = controller.Move(velocity * Time.deltaTime);

        /* Calculate top velocity */
        var udp = velocity;
        udp.y = 0.0f;
        currentVelocity = velocity.magnitude;
        if(currentVelocity > topVelocity)
            topVelocity = currentVelocity;


		if(controller.isGrounded) {
			if(currentJumpLength != 0) {
				jumpLength = currentJumpLength;
				currentJumpLength = 0;
			}
		}
			
    }
    
    private void AirMove() {
        Vector3 wishdir;
        float wishvel = airAcceleration;
        float accel;

        var scale = CmdScale();

        GetInput();

        wishdir = new Vector3(horizontal, 0, vertical);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.y = 0;
        wishdir.Normalize();

        var wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        wishdir.Normalize();
        wishspeed *= scale;

        // CPM: Aircontrol
        var wishspeed2 = wishspeed;
        if(Vector3.Dot(velocity, wishdir) < 0)
            accel = airDeacceleration;
        else
            accel = airAcceleration;
        // If the player is ONLY strafing left or right
        if(vertical == 0 && horizontal != 0) {
            if(wishspeed > sideStrafeSpeed)
                wishspeed = sideStrafeSpeed;
            accel = sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel);
        if(airControl > 0)
            AirControl(wishdir, wishspeed2);
		
		// Apply gravity
		velocity.y -= gravity * Time.deltaTime;
    }

    private void AirControl(Vector3 wishdir, float wishspeed) {
        float zspeed;
        float speed;
        float dot;
        float k;
        int i;

        // Can't control movement if not moving forward or backward
        if(vertical == 0 || wishspeed == 0)
            return;

        zspeed = velocity.y;
        velocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        speed = velocity.magnitude;
        velocity.Normalize();

        dot = Vector3.Dot(velocity, wishdir);
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if(dot > 0) {
            velocity.x = velocity.x * speed + wishdir.x * k;
            velocity.y = velocity.y * speed + wishdir.y * k;
            velocity.z = velocity.z * speed + wishdir.z * k;

            velocity.Normalize();
        }

        velocity.x *= speed;
        velocity.y = zspeed; // Note this line
        velocity.z *= speed;

    }

    private void GroundMove() {
        Vector3 wishdir;
        Vector3 wishvel;

        // Do not apply friction if the player is queueing up the next jump
        if(!wishJump)
            ApplyFriction(1.0f);
        else
            ApplyFriction(0);

        var scale = CmdScale();

        GetInput();

        wishdir = new Vector3(horizontal, 0, vertical);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.y = 0;
        wishdir.Normalize();

        var wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        Accelerate(wishdir, wishspeed, runAcceleration);

        // Jumping
        if(wishJump) {
            velocity.y = jumpSpeed;
            wishJump = false;
        }
    }

    private void ApplyFriction(float t) {
        Vector3 vec = velocity;
        float vel;
        float speed;
        float newspeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        /* Only if the player is on the ground then apply friction */
        if(controller.isGrounded) {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * friction * Time.deltaTime * t;
        }

        newspeed = speed - drop;
        if(newspeed < 0)
            newspeed = 0;
        if(speed > 0)
            newspeed /= speed;

        velocity.x *= newspeed;
        // playerVelocity.y *= newspeed;
        velocity.z *= newspeed;
    }

    private void Accelerate(Vector3 wishdir, float wishspeed, float accel) {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot(velocity, wishdir);
        addspeed = wishspeed - currentspeed;
        if(addspeed <= 0)
            return;
        accelspeed = accel * Time.deltaTime * wishspeed;
        if(accelspeed > addspeed)
            accelspeed = addspeed;

        velocity.x += accelspeed * wishdir.x;
        velocity.z += accelspeed * wishdir.z;
    }

    private float CmdScale() {
        int max = 0;
        float total;
        float scale;

        max = (int)Mathf.Abs(vertical);
        if(Mathf.Abs(horizontal) > max)
            max = (int)Mathf.Abs(horizontal);
        if(max == 0)
            return 0;

        total = Mathf.Sqrt(vertical * vertical + horizontal * horizontal);
        scale = moveSpeed * max / (moveScale * total);

        return scale;
    }
    
    [HideInInspector]
    public float horizontal = 0;
    [HideInInspector]
    public float vertical = 0;

    private void GetInput() {
        horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");
    }

	public void KnockBack(Vector3 direction, float force) {
		controller.Move(direction * force);
	}

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;

        //dont move the rigidbody if the character is on top of it
        if (collisionFlags == CollisionFlags.Below) {
			return;
		}

		if (collisionFlags == CollisionFlags.Above) {
			velocity = new Vector3(velocity.x, -(gravity/10), velocity.z);
		}

		if (collisionFlags == CollisionFlags.Sides) {
			float accel;
			Vector3 wishdir = hit.normal;

			var wishspeed = wishdir.magnitude;
			wishspeed *= slowDownSpeed;
			
			wishdir.Normalize();
			
			// CPM: Aircontrol
			var wishspeed2 = wishspeed;
			if(Vector3.Dot(velocity, wishdir) < 0)
				accel = airDeacceleration;
			else
				accel = airAcceleration;

			// If the player is ONLY strafing left or right
			if(vertical == 0 && horizontal != 0) {
				if(wishspeed > sideStrafeSpeed)
					wishspeed = sideStrafeSpeed;
				accel = sideStrafeAcceleration;
			}
			
			Accelerate(wishdir, wishspeed, accel);
		}

        if(body == null || body.isKinematic) {
            return;
        }


        body.AddForceAtPosition(controller.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }

}