using UnityEngine;

public class RotacionConstanteZ : MonoBehaviour
{
    public float velocidad = 90f; // grados por segundo

    void Update()
    {
        transform.Rotate(0f, 0f, velocidad * Time.deltaTime);
    }
}
