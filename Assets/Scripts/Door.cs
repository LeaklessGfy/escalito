using UnityEngine;

public class Door : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private Sprite openSprite = default;
    [SerializeField] private Sprite closeSprite = default;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (Controller.Main == null)
        {
            return;
        }
        
        var isOpen = Controller.Main.ToggleBar();
        _spriteRenderer.sprite = isOpen ? openSprite : closeSprite;
    }
}