using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ControllerCharacter2D : MonoBehaviour {
	[SerializeField] Animator animator;
	[SerializeField] SpriteRenderer Renderer;
	[SerializeField] float Speed;
	[SerializeField] float TurnRate;
	[SerializeField] float JumpHeight;
	[SerializeField] float DoubleJumpHeight;
	[SerializeField] float HitForce;
	[SerializeField, Range(1, 5)] float FallRateMultiplier;
	[SerializeField, Range(1, 5)] float LowJumpRateMultiplier;
	[Header("Ground")]
	[SerializeField] Transform GroundTransform;
	[SerializeField] LayerMask GroundLayerMask;
	[SerializeField] float GroundRadius;

	Rigidbody2D RB;
	SpriteRenderer spriterenderer;
    Vector2 Velocity = Vector2.zero;
	bool FaceRight = true;

	void Start() {
		RB = GetComponent<Rigidbody2D>();
		spriterenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		// Check if player is on ground
		bool OnGround = Physics2D.OverlapCircle(GroundTransform.position, GroundRadius, GroundLayerMask) != null;

		// get direction input
		Vector2 direction = Vector2.zero;
		direction.x = Input.GetAxis("Horizontal");

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

	private void Flip() {
		Vector3 CurrentScale = gameObject.transform.localScale;
		CurrentScale.x *= -1;
		gameObject.transform.localScale = CurrentScale;

		FaceRight = !FaceRight;

		//FaceRight = false;
		//spriterenderer.flipX = !FaceRight;
	}

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
		Gizmos.DrawSphere(GroundTransform.position, GroundRadius);
    }
}