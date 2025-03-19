using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawCircle : MonoBehaviour
{
    public float radius = 2f; // Rayon du cercle
    public int segments = 50; // Nombre de segments (plus il y en a, plus le cercle sera lisse)

    private LineRenderer lineRenderer;
    public Enemy enemy;

    public Color circleColor = Color.red; // Couleur du cercle
    public float lineWidth = 0.1f; // Épaisseur de la ligne
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1; // Un point supplémentaire pour fermer le cercle
        lineRenderer.loop = true; // Pour que le cercle soit fermé
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = circleColor;
        lineRenderer.endColor = circleColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        
        DrawCircleShape();
    }

    void Update()
    {
        if (enemy != null)
        {
            float angle = 0f;
            for (int i = 0; i <= segments; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius + enemy.transform.position.x;
                float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius + enemy.transform.position.y;
                lineRenderer.SetPosition(i, new Vector3(x, y, enemy.transform.position.z));
                angle += 360f / segments;
            }
        }
    }

    void DrawCircleShape()
    {
        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
            angle += 360f / segments;
        }
    }
}
