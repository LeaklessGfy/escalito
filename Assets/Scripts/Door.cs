using UnityEngine;

public class Door : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private Sprite closeSprite;
    [SerializeField] private Sprite openSprite;

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