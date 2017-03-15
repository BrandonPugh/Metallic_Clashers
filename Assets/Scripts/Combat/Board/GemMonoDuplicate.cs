namespace Combat.Board
{
    using UnityEngine;

    public class GemMonoDuplicate : GemMono
    {
        private void Awake() { }
        private void Start()
        {
            // Now that everything should be initialized, we can show the gem visually
            m_BackgroundImage.enabled = true;
            m_MidgroundImage.enabled = true;
            m_ForegroundImage.enabled = true;
        }

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
