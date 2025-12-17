using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CustomOcclusionCulling : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask cullingLayer;

    [Header("Distancia")]
    [Tooltip("Distancia máxima a la que se renderizan los objetos")]
    public float maxRenderDistance = 30f;

    [Header("Vista")]
    [Tooltip("Margen extra para evitar popping")]
    [Range(0f, 0.3f)]
    public float viewportMargin = 0.05f;

    [Tooltip("Frecuencia de actualización")]
    public float updateInterval = 0.15f;

    private Camera cam;
    private float timer;
    private List<Renderer> renderers = new List<Renderer>();

    void Start()
    {
        cam = GetComponent<Camera>();
        CacheRenderers();
    }

    void CacheRenderers()
    {
        renderers.Clear();
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();

        foreach (Renderer r in allRenderers)
        {
            if (((1 << r.gameObject.layer) & cullingLayer) != 0)
                renderers.Add(r);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            PerformCulling();
        }
    }

    void PerformCulling()
    {
        Vector3 camPos = cam.transform.position;

        foreach (Renderer r in renderers)
        {
            if (r == null) continue;

            bool visible = IsWithinDistance(r, camPos) && IsInCameraView(r);

            if (r.enabled != visible)
                r.enabled = visible;
        }
    }

    bool IsWithinDistance(Renderer r, Vector3 camPos)
    {
        float distance = Vector3.Distance(camPos, r.bounds.center);
        return distance <= maxRenderDistance;
    }

    bool IsInCameraView(Renderer r)
    {
        Bounds b = r.bounds;

        Vector3[] points =
        {
            b.center,
            b.min,
            b.max,
            new Vector3(b.min.x, b.min.y, b.max.z),
            new Vector3(b.max.x, b.max.y, b.min.z)
        };

        foreach (Vector3 p in points)
        {
            Vector3 vp = cam.WorldToViewportPoint(p);

            // Debe estar delante de la cámara
            if (vp.z <= 0) continue;

            if (vp.x >= -viewportMargin && vp.x <= 1 + viewportMargin &&
                vp.y >= -viewportMargin && vp.y <= 1 + viewportMargin)
            {
                return true;
            }
        }

        return false;
    }
}
