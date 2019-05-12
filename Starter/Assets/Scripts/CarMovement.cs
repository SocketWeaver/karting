using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float engineForce = 900f;
    public float steerAngle = 30f;
    public float breakForce = 30000f;

    public WheelCollider frontRightCollider;
    public WheelCollider frontLeftCollider;
    public WheelCollider rearRightCollider;
    public WheelCollider rearLeftCollider;

    public Transform frontRightTransform;
    public Transform frontLeftTransform;
    public Transform rearRightTransform;
    public Transform rearLeftTransform;

    // used for lowering the center of mass of the car
    public Transform centerOfMass;

    GameSceneManager gameSceneManager;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
        gameSceneManager = FindObjectOfType<GameSceneManager>();
    }

    private void Update()
    {
        // update when game is started
        if(gameSceneManager.State == GameSceneManager.GameState.started)
        {
            UpdateWheelTransforms();
            UpdateWheelPhysics();
        }
    }

    /// <summary>
    /// Updates the wheel transforms.
    /// </summary>
    void UpdateWheelTransforms()
    {
        Quaternion rotation;
        Vector3 position;

        frontLeftCollider.GetWorldPose(out position, out rotation);
        frontLeftTransform.position = position;
        frontLeftTransform.rotation = rotation;

        frontRightCollider.GetWorldPose(out position, out rotation);
        frontRightTransform.position = position;
        frontRightTransform.rotation = rotation;

        rearLeftCollider.GetWorldPose(out position, out rotation);
        rearLeftTransform.position = position;
        rearLeftTransform.rotation = rotation;

        rearRightCollider.GetWorldPose(out position, out rotation);
        rearRightTransform.position = position;
        rearRightTransform.rotation = rotation;
    }

    /// <summary>
    /// Updates the wheel physics.
    /// </summary>
    void UpdateWheelPhysics()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        // accelerate
        rearRightCollider.motorTorque = v * engineForce;
        rearLeftCollider.motorTorque = v * engineForce;
        frontRightCollider.motorTorque = v * engineForce;
        frontLeftCollider.motorTorque = v * engineForce;

        // steer
        frontRightCollider.steerAngle = h * steerAngle;
        frontLeftCollider.steerAngle = h * steerAngle;

        // apply brakeTorque
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Break");
            rearRightCollider.brakeTorque = breakForce;
            rearLeftCollider.brakeTorque = breakForce;
            frontRightCollider.brakeTorque = breakForce;
            frontLeftCollider.brakeTorque = breakForce;
        }

        // reset brakeTorque
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Break stop");
            rearRightCollider.brakeTorque = 0;
            rearLeftCollider.brakeTorque = 0;
            frontRightCollider.brakeTorque = 0;
            frontLeftCollider.brakeTorque = 0;
        }
    }
}
