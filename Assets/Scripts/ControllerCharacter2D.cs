using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ControllerCharacter2D : MonoBehaviour, IDamagable {
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
	[Header("Attack")]
	[SerializeField] Transform AttackTransform;
	[SerializeField] float AttackRadius;

	Rigidbody2D RB;

    Vector2 Velocity = Vector2.zero;
	bool FaceRight = true;
    float GroundAngle = 0;

    void Start() {
		RB = GetComponent<Rigidbody2D>();
		spriterenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		// Check if player is on ground
		bool OnGround = UpdateGroundCheck();

		// get direction input
		Vector2 Direction = Vector2.zero;
		Direction.x = Input.GetAxis("Horizontal");

        // Transform direction to slope space 
        Direction = Quaternion.AngleAxis(GroundAngle, Vector3.forward) * Direction;
        Debug.DrawRay(transform.position, Direction, Color.green);

        Velocity.x = Direction.x * Speed;

        // set velocity
        if (OnGround) {
			if (Velocity.y < 0) Velocity.y = 0;
			if (Input.GetButtonDown("Jump")) {
				Velocity.y += Mathf.Sqrt(JumpHeight * -2 * Physics.gravity.y);
				StartCoroutine(DoubleJump());
				animator.SetTrigger("Jump");
			}

			if (Input.GetMouseButton(0)) {
				animator.SetTrigger("Attack");
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
	}

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
		Gizmos.DrawSphere(GroundTransform.position, GroundRadius);
    }

    private void CheckAttack() {
        Collider2D[] Colliders = Physics2D.OverlapCircleAll(AttackTransform.position, AttackRadius);
        foreach (Collider2D Collider in Colliders) {
            if (Collider.gameObject == gameObject) continue;

            if (Collider.gameObject.TryGetComponent<IDamagable>(out var Damagable)) {
                Damagable.Damage(10);
            }
        }
    }

    public void Damage(int damage) {
        throw new System.NotImplementedException();
    }
}