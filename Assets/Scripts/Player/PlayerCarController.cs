using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarController : MonoBehaviour
{
    public float acceleration = 22f;
    public float reverseAcceleration = 14f;
    public float turnSpeed = 70f;
    public float hopForce = 4f;

    public float airControlMultiplier = 0.35f;
    public float airSteerStrength = 8f;

    // Self-righting
    public float selfRightSpeed = 5f;
    public float selfRightMinAngle = 3f;

    private Rigidbody rb;
    private Collider carCollider;

    private bool hopPressed;
    private int hopsRemaining = 2;

    private bool hasGroundContact = false;
    private Vector3 groundNormal = Vector3.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        carCollider = GetComponent<Collider>();

        rb.centerOfMass = new Vector3(0f, -0.4f, 0f);
    }

    void Update()
    {
        bool jumpPressed = false;

        // Keyboard jump
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }

        // Controller jump - Xbox A / PlayStation Cross
        if (Gamepad.current != null &&
            Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            jumpPressed = true;
        }

        if (jumpPressed)
            hopPressed = true;
    }

    void FixedUpdate()
    {
        float move = 0f;
        float steer = 0f;

        // -------------------------
        // KEYBOARD INPUT
        // -------------------------

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                move = 1f;

            if (Keyboard.current.sKey.isPressed)
                move = -1f;

            if (Keyboard.current.aKey.isPressed)
                steer = -1f;

            if (Keyboard.current.dKey.isPressed)
                steer = 1f;
        }

        // -------------------------
        // GAMEPAD INPUT
        // -------------------------

        if (Gamepad.current != null)
        {
            float controllerMove =
                Gamepad.current.rightTrigger.ReadValue()
                - Gamepad.current.leftTrigger.ReadValue();

            float controllerSteer =
                Gamepad.current.leftStick.x.ReadValue();

            // Small steering deadzone
            if (Mathf.Abs(controllerSteer) < 0.15f)
                controllerSteer = 0f;

            // Use whichever device currently has stronger input
            if (Mathf.Abs(controllerMove) > Mathf.Abs(move))
                move = controllerMove;

            if (Mathf.Abs(controllerSteer) > Mathf.Abs(steer))
                steer = controllerSteer;
        }

        bool grounded = hasGroundContact;

        // -------------------------
        // GROUND MOVEMENT
        // -------------------------

        if (grounded)
        {
            float force =
                move >= 0f ? acceleration : reverseAcceleration;

            rb.AddForce(
                transform.forward * move * force,
                ForceMode.Acceleration
            );

            Quaternion targetRotation = rb.rotation;

            // Steering
            if (rb.linearVelocity.magnitude > 0.5f &&
                Mathf.Abs(steer) > 0.01f)
            {
                float direction =
                    move < 0f ? -1f : 1f;

                float turnAmount =
                    steer *
                    turnSpeed *
                    direction *
                    Time.fixedDeltaTime;

                targetRotation =
                    Quaternion.AngleAxis(
                        turnAmount,
                        groundNormal
                    ) * targetRotation;
            }

            // -------------------------
            // SELF-RIGHTING
            // -------------------------

            float tiltAngle =
                Vector3.Angle(
                    targetRotation * Vector3.up,
                    groundNormal
                );

            if (tiltAngle > selfRightMinAngle)
            {
                Vector3 forward =
                    targetRotation * Vector3.forward;

                Vector3 flatForward =
                    Vector3.ProjectOnPlane(
                        forward,
                        groundNormal
                    ).normalized;

                if (flatForward.sqrMagnitude > 0.01f)
                {
                    Quaternion uprightRotation =
                        Quaternion.LookRotation(
                            flatForward,
                            groundNormal
                        );

                    targetRotation =
                        Quaternion.Slerp(
                            targetRotation,
                            uprightRotation,
                            selfRightSpeed *
                            Time.fixedDeltaTime
                        );
                }
            }

            rb.MoveRotation(targetRotation);

            if (rb.linearVelocity.y <= 0.1f)
                hopsRemaining = 2;
        }

        // -------------------------
        // AIR MOVEMENT
        // -------------------------

        else
        {
            Vector3 horizontalForward =
                Vector3.ProjectOnPlane(
                    transform.forward,
                    Vector3.up
                ).normalized;

            float force =
                move >= 0f ? acceleration : reverseAcceleration;

            rb.AddForce(
                horizontalForward *
                move *
                force *
                airControlMultiplier,
                ForceMode.Acceleration
            );

            rb.AddTorque(
                Vector3.up *
                steer *
                airSteerStrength,
                ForceMode.Acceleration
            );
        }

        // -------------------------
        // JUMP / DOUBLE JUMP
        // -------------------------

        if (hopPressed && hopsRemaining > 0)
        {
            rb.AddForce(
                Vector3.up * hopForce,
                ForceMode.Impulse
            );

            hopsRemaining--;
        }

        hopPressed = false;

        // Ground contact gets refreshed every physics step
        hasGroundContact = false;
        groundNormal = Vector3.up;
    }

    private void OnCollisionStay(Collision collision)
    {
        float bestGroundNormal = 0f;

        foreach (ContactPoint contact in collision.contacts)
        {
            float upwardAmount =
                Vector3.Dot(
                    contact.normal,
                    Vector3.up
                );

            // Ignore walls and vertical obstacles
            if (upwardAmount > 0.45f &&
                upwardAmount > bestGroundNormal)
            {
                bestGroundNormal = upwardAmount;
                groundNormal = contact.normal;
                hasGroundContact = true;
            }
        }
    }
}