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
        var main = MagicBag.Bag.main;
        main.BarIsOpen = !main.BarIsOpen;
        _spriteRenderer.sprite = main.BarIsOpen ? openSprite : closeSprite;
    }
}