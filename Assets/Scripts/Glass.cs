using UnityEngine;

public class Glass : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private LineRenderer lineRenderer;
    private ParticleSystem fullParticleSystem;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        fullParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        float zAngle = transform.localEulerAngles.z;
        bool shouldFlow = zAngle > 80 && zAngle < 280;

        if (shouldFlow)
        {
            Vector3 currentPosition = lineRenderer.GetPosition(0);
            Vector3 stepPosition = Vector3.up * 0.1f;
            Vector3 newPosition = currentPosition - stepPosition;

            if (IsInConstrainsts(newPosition))
            {
                lineRenderer.SetPosition(0, newPosition);
            }
        }
    }

    private void OnParticleCollision(GameObject origin)
    {
        ParticleSystem particleSystem = origin.GetComponent<ParticleSystem>();
        Color color = particleSystem.main.startColor.color;

        //lineRenderer.startColor = new Color(color.r, color.g, color.b, 0.60f);
        //lineRenderer.endColor = new Color(color.r, color.g, color.b, 0.70f);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;


        Vector3 currentPosition = lineRenderer.GetPosition(0);
        Vector3 stepPosition = Vector3.up * 0.1f;
        Vector3 newPosition = currentPosition + stepPosition;

        if (IsInConstrainsts(newPosition))
        {
            lineRenderer.SetPosition(0, newPosition);
        }
        else
        {
            if (!fullParticleSystem.isPlaying)
            {
                var main = fullParticleSystem.main;
                main.startColor = color;
                fullParticleSystem.Play();
            }
        }
    }

    private bool IsInConstrainsts(Vector3 position)
    {
        return position.y >= 0 && position.y <= boxCollider.size.y;
    }
}
