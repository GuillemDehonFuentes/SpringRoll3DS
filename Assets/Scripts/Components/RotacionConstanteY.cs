using UnityEngine;

public class RotacionConstanteY : MonoBehaviour
{
    public float velocidad = 90f; // grados por segundo

    void Update()
    {
        transform.Rotate(0f, velocidad * Time.deltaTime, 0f);
    }
}
