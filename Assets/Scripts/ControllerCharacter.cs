using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControllerCharacter : MonoBehaviour {
	[SerializeField] float Speed;
	[SerializeField] float TurnRate;
	[SerializeField] float JumpHeight;
	[SerializeField] float DoubleJumpHeight;
	[SerializeField] float HitForce;
	[SerializeField, Range(1, 5)] float FallRateMultiplier;
	[SerializeField, Range(1, 5)] float lowJumpRateMultiplier;
	[Header("Ground")]
	[SerializeField] Transform GroundTransform;
	[SerializeField] LayerMask GroundLayerMask;

	CharacterController CharacterController;
	Vector3 Velocity = Vector3.zero;

	void Start() {
		CharacterController = GetComponent<CharacterController>();
	}

	void Update() {
		// Check if player is on ground
		bool OnGround = Physics.CheckSphere(GroundTransform.position, 0.2f, GroundLayerMask, QueryTriggerInteraction.Ignore);

		// get direction input
		Vector3 direction = Vector3.zero;
		direction.x = Input.GetAxis("Horizontal");
		direction.z = Input.GetAxis("Vertical");

        Velocity.x = direction.x * Speed;
        Velocity.z = direction.z * Speed;

        // set velocity
        if (OnGround) {
			if (Velocity.y < 0) Velocity.y = 0;
			if (Input.GetButtonDown("Jump")) {
				Velocity.y += Mathf.Sqrt(JumpHeight * -2 * Physics.gravity.y);
				StartCoroutine(DoubleJump());
			}
		}

		// Adjust gravity for jump 
		float GravityMultiplier = 1;
		if (!OnGround && Velocity.y < 0) GravityMultiplier = FallRateMultiplier;
		if (!OnGround && Velocity.y > 0 && !Input.GetButton("Jump")) GravityMultiplier = lowJumpRateMultiplier;

		Velocity.y += Physics.gravity.y * GravityMultiplier * Time.deltaTime;

		// move character
		CharacterController.Move(Velocity * Time.deltaTime);

		// Rotate character to face direction of movement
		Vector3 Face = new Vector3(Velocity.x, 0, Velocity.z);
		if (Face.magnitude > 0) {
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Face), Time.deltaTime * TurnRate);
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Rigidbody body = hit.collider.attachedRigidbody;

		// No rigidbody
		if (body == null || body.isKinematic) {
			return;
		}

		// We dont want to push objects below us
		if (hit.moveDirection.y < -0.3)	{
			return;
		}

		// Calculate push direction from move direction,
		// we only push objects to the sides never up and down
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

		// If you know how fast your character is trying to move,
		// then you can also multiply the push velocity by that.

		// Apply the push
		body.velocity = pushDir * HitForce;
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
}