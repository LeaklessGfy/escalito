using System.Collections.Generic;
using Ingredients;
using UnityEngine;

namespace Cocktails
{
    public class Glass : MonoBehaviour
    {
        private const float Speed = 0.1f;
        private readonly LinkedList<SpriteRenderer> _sprites = new LinkedList<SpriteRenderer>();

        private IngredientKey _lastIngredient;

        [SerializeField] private Sprite sprite;

        public Cocktail Cocktail { get; } = CocktailBuilder.BuildEmpty();
        public bool Served { get; set; }

        private void OnParticleCollision(GameObject origin)
        {
            var bottle = origin.GetComponentInParent<Bottle>();
            var ingredient = bottle.ingredient;
            if (_sprites.Count < 1 || ingredient.key != _lastIngredient)
            {
                AddSpriteRenderer(ingredient);
            }

            AddIngredient(ingredient);
        }

        // TODO : Think about making it inside drag and drop and not collision
        private void OnCollisionEnter2D(Collision2D collision)
        {
            var go = collision.gameObject;
            var consumable = go.GetComponent<Consumable>();

            if (consumable == null)
            {
                return;
            }

            go.transform.SetParent(transform);
            Destroy(go.GetComponent<Rigidbody2D>());
            Cocktail.AddIngredient(consumable.ingredient.key);
        }

        private void AddSpriteRenderer(Ingredient ingredient)
        {
            var go = new GameObject();
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = new Vector2(0, GetNextY());
            go.transform.localScale = new Vector2(5, 5);
            go.name = ingredient.key + " " + _sprites.Count;

            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = ingredient.color;
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            spriteRenderer.sortingLayerName = "Ingredients";
            spriteRenderer.sortingOrder = _sprites.Count * -1;

            _sprites.AddLast(spriteRenderer);
            _lastIngredient = ingredient.key;
        }

        private void AddIngredient(Ingredient ingredient)
        {
            var last = _sprites.Last.Value.transform;
            var localPosition = last.localPosition;
            last.localPosition = new Vector2(localPosition.x, localPosition.y + Speed);

            Cocktail.AddIngredient(ingredient.key);
        }

        private float GetNextY()
        {
            if (_sprites.Count < 1)
            {
                return -10;
            }

            return _sprites.Last.Value.transform.localPosition.y;
        }
    }
}