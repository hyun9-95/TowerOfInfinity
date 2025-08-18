using UnityEngine;

public class WireSphereRenderer : MonoBehaviour
{
    public float radius = 1f;
    public Color color = Color.white;
    private LineRenderer lineRenderer;
    
    void Start()
    {
        CreateWireSphere();
    }
    
    private void CreateWireSphere()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = false;
        
        int segments = 32;
        lineRenderer.positionCount = segments + 1;
        
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 2f * Mathf.PI / segments;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            lineRenderer.SetPosition(i, pos);
        }
    }
}