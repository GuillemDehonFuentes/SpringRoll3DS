using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform target;

    [Header("Offsets")]
    public Vector3 positionOffset;

    void Start()
    {
        if (target == null) return;

        // Calcula el offset SOLO en X y Z
        Vector3 flatOffset = transform.position - target.position;
        flatOffset.y = 0f;

        positionOffset = Quaternion.Inverse(target.rotation) * flatOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calcula la posición objetivo solo en X y Z
        Vector3 targetFlatPos = target.position + target.rotation * positionOffset;

        // Mantiene la Y actual del objeto
        transform.position = new Vector3(
            targetFlatPos.x,
            transform.position.y,
            targetFlatPos.z
        );
    }
}
