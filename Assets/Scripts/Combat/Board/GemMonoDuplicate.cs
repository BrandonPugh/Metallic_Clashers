namespace Combat.Board
{
    using UnityEngine;

    public class GemMonoDuplicate : GemMono
    {
        protected override void UpdateTransformPosition()
        {
            var spacing = gridMono.CalculateSpacing();

            var nextPosition =
                grid.ClampPosition(
                    position + rowMono.currentDirection + columnMono.currentDirection);

            var offsetPosition = nextPosition - rowMono.currentDirection - columnMono.currentDirection;
            offsetPosition =
                new Vector2(offsetPosition.x * spacing.x, offsetPosition.y * spacing.y);

            m_RectTransform.anchoredPosition = 
                offsetPosition + rowMono.positionOffset + columnMono.positionOffset;

            m_PositionIsDirty = false;
        }

        public new static void Init()
        {
            Gem.onCreate.AddListener(OnCreateGem);
        }

        private static void OnCreateGem(Gem newGem)
        {
            var newGemMonoDuplicate = CreateBaseGameObject<GemMonoDuplicate>(newGem);

            SubscribeToEvents(newGemMonoDuplicate);

            newGemMonoDuplicate.gameObject.SetActive(false);
        }
    }
}
