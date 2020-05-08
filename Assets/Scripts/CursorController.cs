using UnityEngine;

public class CursorController : MonoBehaviour
{
    private static readonly Vector2 HotSpot = new Vector2(10, 5);

    public static CursorController Main;

    public Texture2D hover;
    public Texture2D normal;

    private void Awake()
    {
        Main = this;
        Cursor.SetCursor(normal, HotSpot, CursorMode.ForceSoftware);
    }

    public void SetHover(bool isHover)
    {
        Cursor.SetCursor(isHover ? hover : normal, HotSpot, CursorMode.ForceSoftware);
    }
}