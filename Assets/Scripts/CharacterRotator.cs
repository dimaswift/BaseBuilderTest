using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotator : MonoBehaviour
{
    public float sensitivity = 5;
    public float smoothTime = 1;
    public float minX = -360F;
    public float maxX = 360F;

    public float minY = -60F;
    public float maxY = 60F;

    Vector2 rotationAxis;

    Quaternion originalRotation;

    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        originalRotation = transform.localRotation;
    }

    /// <summary>
    /// Rotates rigidbody using delta. Delta is a pointer position delta on screen. Should be called from FixedUpdate
    /// </summary>
    /// <param name="delta"></param>
    public void Rotate(Vector2 delta)
    {
        rotationAxis += delta * sensitivity;

        rotationAxis.y = ClampAngle(rotationAxis.y, minY, maxY);
        rotationAxis.x = ClampAngle(rotationAxis.x, minX, maxX);

        Quaternion yQuaternion = Quaternion.AngleAxis(rotationAxis.y, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationAxis.x, Vector3.up);
        var finalRot = Quaternion.Lerp(body.rotation, originalRotation * xQuaternion * yQuaternion, smoothTime * Time.fixedDeltaTime);
        body.MoveRotation(finalRot);
        body.angularVelocity = Vector3.zero;
    }

    float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}