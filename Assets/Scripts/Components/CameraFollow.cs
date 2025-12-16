using UnityEngine;
using UnityEngine.N3DS;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform target;

    [Header("Offsets")]
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    [Header("Suavizado")]
    public float smoothSpeed = 10f;

    [Header("Rotación alrededor del objetivo")]
    public float rotationSpeed = 50f;

    void Update()
    {
        if (target == null) return;

        if (Input.GetKey(KeyCode.E) || GamePad.GetButtonHold(N3dsButton.R))
            offset = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f) * offset;

        if (Input.GetKey(KeyCode.Q) || GamePad.GetButtonHold(N3dsButton.L))
            offset = Quaternion.Euler(0f, -rotationSpeed * Time.deltaTime, 0f) * offset;
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        transform.LookAt(target);
    }
}
