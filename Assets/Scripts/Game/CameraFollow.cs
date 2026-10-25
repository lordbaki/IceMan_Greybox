using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float distance = 10f;
    public float height = 6f;
    public float followSpeed = 5f;
    public float lookAhead = 5f;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition =
            target.position
            - target.forward * distance
            + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        Vector3 lookTarget =
            target.position
            + target.forward * lookAhead
            + Vector3.up * 0.5f;

        transform.LookAt(lookTarget);
    }
}