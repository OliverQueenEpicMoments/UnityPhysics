using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIController2D : MonoBehaviour {
	[SerializeField] Animator animator;
	[SerializeField] SpriteRenderer spriterenderer;
	[SerializeField] float Speed;
	[SerializeField] float JumpHeight;
	[SerializeField] float DoubleJumpHeight;
	[SerializeField, Range(1, 5)] float FallRateMultiplier;
	[SerializeField, Range(1, 5)] float LowJumpRateMultiplier;
	[Header("Ground")]
	[SerializeField] Transform GroundTransform;
	[SerializeField] LayerMask GroundLayerMask;
	[SerializeField] float GroundRadius;
    [Header("AI")]
    [SerializeField] Transform[] Waypoints;
    [SerializeField] float RayDistance = 1;

	Rigidbody2D RB;

    Vector2 Velocity = Vector2.zero;
	bool FaceRight = true;
	float GroundAngle = 0;
	Transform TargetWaypoint = null;

	enum State { 
		IDLE,
		PATROL,
		CHASE,
		ATTACK
	}

	State state = State.IDLE;
	float StateTimer = 0;

    void Start() {
		RB = GetComponent<Rigidbody2D>();
		//spriterenderer = GetComponent<SpriteRenderer>();
	}

    
	void Update() {
        // Update AI
        Vector2 direction = Vector2.zero;
        switch (state) {
			case State.IDLE:
				//if (CanSeePlayer()) state = State.CHASE;
				StateTimer += Time.deltaTime;
				if (StateTimer >= 0.5f) {
                    SetNewWaypointTarget();
                    state = State.PATROL;
                }
				break;
			case State.PATROL:
				//if (CanSeePlayer()) state = State.CHASE;

				direction.x = Mathf.Sign(TargetWaypoint.position.x - transform.position.x);
				float DX = Mathf.Abs(TargetWaypoint.position.x - transform.position.x);
				if (DX <= 0.25f)	{
					state = State.IDLE;
					StateTimer = 0;
				}
				break;
            case State.CHASE:

                break;
            case State.ATTACK:

				break;
			default:
				break;
		}

		// Check if player is on ground
		bool OnGround = UpdateGroundCheck();

        // get direction input

        // Transform direction to slope space
        direction = Quaternion.AngleAxis(GroundAngle, Vector3.forward) * direction;
        Debug.DrawRay(transform.position, direction, Color.green);

        Velocity.x = direction.x * Speed;

        // set velocity
        if (OnGround) {
			if (Velocity.y < 0) Velocity.y = 0;
			if (Input.GetButtonDown("Jump")) {
				Velocity.y += Mathf.Sqrt(JumpHeight * -2 * Physics.gravity.y);
				StartCoroutine(DoubleJump());
				animator.SetTrigger("Jump");
			}
		}

		// Adjust gravity for jump 
		float GravityMultiplier = 1;
		if (!OnGround && Velocity.y < 0) GravityMultiplier = FallRateMultiplier;
		if (!OnGround && Velocity.y > 0 && !Input.GetButton("Jump")) GravityMultiplier = LowJumpRateMultiplier;

		Velocity.y += Physics.gravity.y * GravityMultiplier * Time.deltaTime;

		// move character
		RB.velocity = Velocity;

		// Rotate character to face direction of movement
		if (Velocity.x > 0 && !FaceRight) Flip();
		if (Velocity.x < 0 && FaceRight) Flip();

		// Update the animator
		animator.SetFloat("Speed", Mathf.Abs(Velocity.x));
		animator.SetBool("Fall", !OnGround && Velocity.y < -0.1f);
	}

    IEnumerator DoubleJump() {
		// Wait a bit after first jump 
		yield return new WaitForSeconds(0.01f);

		// Allow a double jump while moving up 
		while (Velocity.y > 0) {
			// If jump is pressed add jump velocity
			if (Input.GetButtonDown("Jump")) {
                Velocity.y += Mathf.Sqrt(DoubleJumpHeight * -2 * Physics.gravity.y);
				break;
            }
			yield return null;
		}
	}

	private bool UpdateGroundCheck() {
		// Check if character is on ground
        Collider2D Collider = Physics2D.OverlapCircle(GroundTransform.position, GroundRadius, GroundLayerMask);
		if (Collider != null) { 
			RaycastHit2D raycasthit = Physics2D.Raycast(GroundTransform.position, Vector2.down, GroundRadius, GroundLayerMask);
			if (raycasthit.collider != null) {
				// Get the angle of the ground
				GroundAngle = Vector2.SignedAngle(Vector2.up, raycasthit.normal);
                Debug.DrawRay(raycasthit.point, raycasthit.normal, Color.red);
            }
        }
		return Collider != null;
    }

	private void Flip() {
		Vector3 CurrentScale = gameObject.transform.localScale;
		CurrentScale.x *= -1;
		gameObject.transform.localScale = CurrentScale;

		FaceRight = !FaceRight;

		//FaceRight = !FaceRight;
		//spriterenderer.flipX = !FaceRight;
	}

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
		Gizmos.DrawSphere(GroundTransform.position, GroundRadius);
    }

	private void SetNewWaypointTarget() {
        Transform Waypoint = null;
        do {
			Waypoint = Waypoints[Random.Range(0, Waypoints.Length)];
		} while (Waypoint == TargetWaypoint);
        TargetWaypoint = Waypoint;
	}

	private bool CanSeePlayer() {
        RaycastHit2D raycasthit = Physics2D.Raycast(transform.position, ((FaceRight) ? Vector2.right : Vector2.left) * RayDistance);
        Debug.DrawRay(transform.position, ((FaceRight) ? Vector2.right : Vector2.left) * RayDistance);

        return raycasthit.collider != null && raycasthit.collider.gameObject.CompareTag("Player");
	}
}