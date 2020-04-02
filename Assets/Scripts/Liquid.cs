using System.Collections;
using UnityEngine;

public class Liquid : MonoBehaviour
{
    public float flowSpeed = 2f;

    private LineRenderer lineRenderer;
    private Vector2 targetPosition;
    private Coroutine currentRoutine;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    public void Begin()
    {
        currentRoutine = StartCoroutine(BeginFlow());
    }

    private IEnumerator BeginFlow()
    {
        while (gameObject.activeSelf)
        {
            targetPosition = FindEndPoint();
            MoveToPosition(0, transform.position);
            AnimateToPosition(1, targetPosition);
            yield return null;
        }
    }

    public void End()
    {
        StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(EndFlow());
    }

    private IEnumerator EndFlow()
    {
        while (!HasReachedPostion(0, targetPosition))
        {
            AnimateToPosition(0, targetPosition);
            AnimateToPosition(1, targetPosition);
            yield return null;
        }
        Destroy(gameObject);
    }

    private Vector2 FindEndPoint()
    {
        Ray2D r = new Ray2D(transform.position, Vector2.down);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
        Vector2 endPoint = hit.collider ? hit.point : r.GetPoint(10f);
        return endPoint;
    }

    private void MoveToPosition(int index, Vector2 targetPosition)
    {
        lineRenderer.SetPosition(index, targetPosition);
    }

    private void AnimateToPosition(int index, Vector2 targetPosition)
    {
        Vector2 currentPosition = lineRenderer.GetPosition(index);
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, Time.deltaTime * flowSpeed);
        lineRenderer.SetPosition(index, newPosition);
    }

    private bool HasReachedPostion(int index, Vector2 targetPosition)
    {
        Vector2 currentPosition = lineRenderer.GetPosition(index);
        return currentPosition == targetPosition;
    }
}
