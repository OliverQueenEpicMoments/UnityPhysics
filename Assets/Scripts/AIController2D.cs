using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody2D))]
public class AIController2D : MonoBehaviour, IDamagable {
	[SerializeField] Animator animator;
	[SerializeField] SpriteRenderer spriterenderer;
	[SerializeField] Health EnemyHealth;
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
    [SerializeField] Transform PlayerLocation;
    [SerializeField] Transform RaycastLocation;
    [SerializeField] Transform[] Waypoints;
    [SerializeField] float RayDistance = 1;
    [SerializeField] string EnemyTag;
    [SerializeField] LayerMask RaycastLayerMask;
    [SerializeField] bool HasFall = true;
    [SerializeField] float EnemyDamage = 1;

	public float Health = 100;
	private bool AttackSwap = true;

	Rigidbody2D RB;

    Vector2 Velocity = Vector2.zero;
	bool FaceRight = true;
	float GroundAngle = 0;
	Transform TargetWaypoint = null;
	GameObject Enemy = null;

	enum State { 
		IDLE,
		PATROL,
		CHASE,
		ATTACK,
		// Hit state where stunned and goes to chase?
		DEATH
	}

	State state = State.IDLE;
	float StateTimer = 1;

    void Start() {
		RB = GetComponent<Rigidbody2D>();
		spriterenderer = GetComponent<SpriteRenderer>();
		EnemyHealth = GetComponent<Health>();
	}

	void Update() {
        // Update AI
		CheckEnemySeen();
		if (EnemyHealth.CurrentHealth <= 1) state = State.DEATH;

        Vector2 Direction = Vector2.zero;
        switch (state) {
			case State.IDLE:
				if (Enemy != null) state = State.CHASE;

                StateTimer -= Time.deltaTime;
				if (StateTimer <= 0) {
                    SetNewWaypointTarget();
                    state = State.PATROL;
                }
				break;
			case State.PATROL:
				{
                    if (Enemy != null) state = State.CHASE;

                    Direction.x = Mathf.Sign(TargetWaypoint.position.x - transform.position.x);
                    float DX = Mathf.Abs(TargetWaypoint.position.x - transform.position.x);
                    if (DX <= 0.25f) {
                        state = State.IDLE;
                        StateTimer = 1;
                    }
                }
				break;
            case State.CHASE:
				{
                    if (Enemy == null) {
                        state = State.IDLE;
                        StateTimer = 1;
                        break;
                    }

                    float DX = Mathf.Abs(Enemy.transform.position.x - transform.position.x);
                    if (DX <= 0.9f) {
						state = State.ATTACK;
						if (AttackSwap) {
							StartCoroutine(HitStun(AttackSwap));
							break;
                        } else if (AttackSwap == false) {
							StartCoroutine(HitStun(AttackSwap));
                            break;
                        }
					} else {
                        Direction.x = Mathf.Sign(Enemy.transform.position.x - transform.position.x);
                    }
                }
				break;
            case State.ATTACK:
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0)) {
                    state = State.CHASE;
                }
                break;
            case State.DEATH:
				StartCoroutine(Death());
				break;
            default:
				break;
		}

		// Check if player is on ground
		bool OnGround = UpdateGroundCheck();

        // get direction input

        // Transform direction to slope space
        Direction = Quaternion.AngleAxis(GroundAngle, Vector3.forward) * Direction;
        Debug.DrawRay(transform.position, Direction, Color.green);

        Velocity.x = Direction.x * Speed;

        // set velocity
        if (OnGround) {
			if (Velocity.y < 0) Velocity.y = 0;
			//if (Input.GetButtonDown("Jump")) {
			//	Velocity.y += Mathf.Sqrt(JumpHeight * -2 * Physics.gravity.y);
			//	StartCoroutine(DoubleJump());
			//	animator.SetTrigger("Jump");
			//}
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
		if (HasFall == true) animator.SetBool("Fall", !OnGround && Velocity.y < -0.1f);
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

    private void CheckEnemySeen() {
        Enemy = null;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, ((FaceRight) ? Vector2.right : Vector2.left), RayDistance, RaycastLayerMask);
        if (raycastHit.collider != null && raycastHit.collider.gameObject.CompareTag(EnemyTag)) {
            Enemy = raycastHit.collider.gameObject;
            Debug.DrawRay(RaycastLocation.position, ((FaceRight) ? Vector2.right : Vector2.left) * RayDistance, Color.red);
        }
    }

	public void Damage(int damage) { 
		Health -= damage;
		print(Health);
	}

	IEnumerator HitStun(bool chain) {
		if (chain) {
			animator.SetTrigger("Attack1");
            yield return new WaitForSeconds(0.75f);
            Enemy.GetComponent<Health>().TakeDamage(EnemyDamage);
            Debug.Log("Player health: " + Enemy.GetComponent<Health>().CurrentHealth);
            AttackSwap = false;
        } else {
            animator.SetTrigger("Attack2");
            yield return new WaitForSeconds(0.75f);
            Enemy.GetComponent<Health>().TakeDamage(EnemyDamage);
			Debug.Log("Player health: " + Enemy.GetComponent<Health>().CurrentHealth);
            AttackSwap = true;
        }
    }

	IEnumerator Death() {
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }
}