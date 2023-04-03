using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {
    [System.Serializable]
    public class Wheel {
        public WheelCollider Collider;
        public Transform transform;
    }

    [System.Serializable]
    public class Axel {
        public Wheel LeftWheel;
        public Wheel RightWheel;
        public bool IsMotor;
        public bool IsSteering;
    }

    [SerializeField] Axel[] Axels;
    [SerializeField] float MaxMotorTorque;
    [SerializeField] float MaxSteeringAngle;

    void FixedUpdate() {
        float Motor = MaxMotorTorque * Input.GetAxis("Vertical");
        float Steering = MaxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (Axel axel in Axels) {
            if (axel.IsSteering) {
                axel.LeftWheel.Collider.steerAngle = Steering;
                axel.RightWheel.Collider.steerAngle = Steering;
            }

            if (axel.IsMotor) {
                axel.LeftWheel.Collider.motorTorque = Motor;
                axel.RightWheel.Collider.motorTorque = Motor;
            }

            UpdateWheelTransform(axel.LeftWheel);
            UpdateWheelTransform(axel.RightWheel);
        }
    }

    public void UpdateWheelTransform(Wheel wheel) {
        wheel.Collider.GetWorldPose(out Vector3 Position, out Quaternion Rotation);

        wheel.transform.position = Position;
        wheel.transform.rotation = Rotation;
    }
}