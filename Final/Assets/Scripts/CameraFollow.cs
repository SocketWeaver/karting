using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// The target.
    /// </summary>
    public Transform target;

    /// <summary>
    /// The offset position.
    /// </summary>
    public Vector3 offsetPosition;

    public float damp = 0.2f;

    public void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 newPosition = target.TransformPoint(offsetPosition);
        transform.position = Vector3.Lerp(transform.position, newPosition, damp);
        transform.LookAt(target);
    }
}
