using UnityEngine;

public class Door : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    public Sprite closeSprite;
    public Sprite openSprite;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (MainController.Main == null)
        {
            return;
        }

        MainController.Main.BarIsOpen = !MainController.Main.BarIsOpen;
        _spriteRenderer.sprite = MainController.Main.BarIsOpen ? openSprite : closeSprite;
    }
}