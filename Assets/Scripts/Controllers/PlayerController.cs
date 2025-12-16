using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.N3DS;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    public float moveForce = 10f;
    public float maxSpeed = 8f;

    [Header("Physics Damping")]
    public float deceleration = 5f; 

    [Header("Camera References")]
    public Transform cameraTransform;

    [Header("Character References")]
    public Animator characterAnimator;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (cameraTransform == null)
        {
            UnityEngine.Debug.LogWarning("PlayerController: No se ha asignado cameraTransform.");
            return;
        }

        float moveX = GamePad.CirclePad.x;
        float moveZ = GamePad.CirclePad.y;

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 movement = (camForward * moveZ) + (camRight * moveX);

#if UNITY_EDITOR
        float editorMoveX = Input.GetAxis("Horizontal");
        float editorMoveZ = Input.GetAxis("Vertical");

        Vector3 movementEditor = (camForward * editorMoveZ) + (camRight * editorMoveX);

        if (rb.velocity.magnitude < maxSpeed)
            rb.AddForce(movementEditor * moveForce, ForceMode.Force);
#endif

        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(movement * moveForce, ForceMode.Force);
        }

        if (moveX == 0 && moveZ == 0)
        {
            Vector3 vel = rb.velocity;
            vel.y = 0;

            Vector3 decel = vel.normalized * deceleration * Time.fixedDeltaTime;

            if (decel.magnitude > vel.magnitude)
                decel = vel;

            rb.velocity -= new Vector3(decel.x, 0, decel.z);
        }

        characterAnimator.SetBool("Moving", IsMoving());
        characterAnimator.SetFloat("MoveSpeed", GetMovementSpeed());
    }

    public bool IsMoving(float threshold = 0.1f)
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        return horizontalVelocity.magnitude > threshold;
    }

    public float GetMovementSpeed()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        float realSpeed = horizontalVelocity.magnitude;
        float animSpeed = Mathf.Clamp((realSpeed / maxSpeed) * 3f, 0f, 3f);
        return animSpeed;
    }
}
