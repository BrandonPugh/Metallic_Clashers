namespace Combat.Board
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Information;

    using UnityEngine;

    [RequireComponent(typeof(RectTransform))]
    public class GemMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        protected Gem m_Gem;

        protected RectTransform m_RectTransform;

        private static bool s_Initialized;

        protected Vector2 m_CurrentPosition;
        private IEnumerator m_MoveToPositionCoroutine;

        private IEnumerator m_MatchAnimationCoroutine;

        protected bool m_PositionIsDirty;

        private GemMonoDuplicate m_GemMonoDuplicate;

        private static List<GemMono> s_GemsCreatedThisFrame = new List<GemMono>();

        public RectTransform rectTransform { get { return m_RectTransform; } }

        public Gem gem { get { return m_Gem; } private set { m_Gem = value; } }

        public Grid grid { get { return m_Gem.grid; } }
        public GridMono gridMono { get { return grid.GetComponent<GridMono>(); } }

        public Vector2 position { get { return m_Gem.position; } set { m_Gem.position = value; } }
        public GemType gemType { get { return m_Gem.gemType; } set { m_Gem.gemType = value; } }

        public Row row { get { return m_Gem.row; } }
        public GridCollectionMono rowMono { get { return row.GetComponent<GridCollectionMono>(); } }

        public Column column { get { return m_Gem.column; } }
        public GridCollectionMono columnMono { get { return column.GetComponent<GridCollectionMono>(); } }

        public Vector2 currentPosition
        {
            get { return m_CurrentPosition; }
            set { m_CurrentPosition = value; }
        }

        public IEnumerator moveToPositionCoroutine { get { return m_MoveToPositionCoroutine; } }

        public IEnumerator matchAnimationCoroutine { get { return m_MatchAnimationCoroutine; } }

        public bool positionIsDirty { get { return m_PositionIsDirty; } set { m_PositionIsDirty = value; } }

        private void Awake()
        {
            s_GemsCreatedThisFrame.Add(this);
        }

        private void Start()
        {
            var stackPosition = 0f;
            foreach (var gemMono in s_GemsCreatedThisFrame)
                if (position.x == gemMono.position.x)
                    if (position.y > gemMono.position.y)
                        stackPosition++;

            m_GemMonoDuplicate = gem.GetComponent<GemMonoDuplicate>();

            m_CurrentPosition =
                new Vector2(
                    m_Gem.position.x
                    * (gridMono.rectTransform.rect.width / (grid.size.x - 1)),

                    CombatManager.self.gridParentRectTransform.rect.height
                    + stackPosition
                    * (gridMono.rectTransform.rect.height / (grid.size.y - 1)));

            m_RectTransform.anchoredPosition = m_CurrentPosition;

            OnPositionChange(new PositionChangeInformation { gem = gem, newPosition = gem.position });
        }

        private void OnCombatUpdate()
        {
            s_GemsCreatedThisFrame.Remove(this);

            if (m_MoveToPositionCoroutine != null)
                if (!m_MoveToPositionCoroutine.MoveNext())
                    m_MoveToPositionCoroutine = null;
        }

        protected void OnCombatLateUpdate()
        {
            if (m_PositionIsDirty)
                UpdateTransformPosition();
        }

        public Vector2 CalculatePosition(Vector2 newPosition)
        {
            newPosition = grid.ClampPosition(newPosition);

            var spacing = gridMono.CalculateSpacing();
            return new Vector2(newPosition.x * spacing.x, newPosition.y * spacing.y);
        }

        private void OnPositionChange(PositionChangeInformation positionChangeInfo)
        {
            name = "Gem " + positionChangeInfo.newPosition;

            var spacing = gridMono.CalculateSpacing();
            var moveToPosition =
                new Vector2(
                    positionChangeInfo.newPosition.x * spacing.x,
                    positionChangeInfo.newPosition.y * spacing.y);

            m_MoveToPositionCoroutine = MoveToPosition(moveToPosition);
        }

        protected void OnMatch(MatchInformation matchInfo)
        {
            if (matchInfo.gems.Any(matchInfoGem => matchInfoGem == m_Gem))
            {
                m_MatchAnimationCoroutine = MatchAnimation();
                gridMono.matchingGemMonos.Add(this);
            }
        }

        private void OnGridResize(GridResizeInformation gridResizeInformation)
        {
            OnPositionChange(
                new PositionChangeInformation
                {
                    gem = gem,
                    newPosition = gem.position
                });
        }

        protected virtual void UpdateTransformPosition()
        {
            var nextPosition =
                grid.ClampPosition(
                    position + rowMono.currentDirection + columnMono.currentDirection);

            if (nextPosition != position + rowMono.currentDirection + columnMono.currentDirection)
            {
                if (!m_GemMonoDuplicate.gameObject.activeSelf)
                {
                    m_GemMonoDuplicate.gameObject.SetActive(true);
                    m_GemMonoDuplicate.UpdateTransformPosition();
                }
            }
            else if (m_GemMonoDuplicate.gameObject.activeSelf)
                m_GemMonoDuplicate.gameObject.SetActive(false);

            m_RectTransform.anchoredPosition =
                m_CurrentPosition + rowMono.positionOffset + columnMono.positionOffset;

            m_PositionIsDirty = false;
        }

        private IEnumerator MoveToPosition(Vector2 newPosition)
        {
            var deltaTime = 0f;
            while (m_CurrentPosition != newPosition)
            {
                m_CurrentPosition =
                    Vector2.MoveTowards(m_CurrentPosition, newPosition, 5f + 50f * deltaTime);

                deltaTime += Time.deltaTime;

                m_PositionIsDirty = true;

                yield return null;
            }

            m_PositionIsDirty = true;

            m_MoveToPositionCoroutine = null;
        }

        protected IEnumerator MatchAnimation()
        {
            var gemImages = gem.GetComponents<GemImage>().ToArray();

            var backgroundImage = gemImages.FirstOrDefault(
                gemImage => gemImage.imageType == GemImage.ImageType.Background);
            var midgroundImage = gemImages.FirstOrDefault(
                gemImage => gemImage.imageType == GemImage.ImageType.Midground);
            var foregroundImage = gemImages.FirstOrDefault(
                gemImage => gemImage.imageType == GemImage.ImageType.Foreground);

            var deltaTime = 0f;
            while (deltaTime < 0.5f)
            {
                if (backgroundImage != null)
                {
                    backgroundImage.color =
                        new Color(
                            backgroundImage.color.r,
                            backgroundImage.color.g,
                            backgroundImage.color.b,
                            1 - deltaTime / 0.5f);
                }

                if (midgroundImage != null)
                {
                    midgroundImage.color =
                        new Color(
                            midgroundImage.color.r,
                            midgroundImage.color.g,
                            midgroundImage.color.b,
                            1 - deltaTime / 0.5f);
                }

                if (foregroundImage)
                {
                    foregroundImage.color =
                        new Color(
                            foregroundImage.color.r,
                            foregroundImage.color.g,
                            foregroundImage.color.b,
                            1 - deltaTime / 0.5f);
                }

                m_RectTransform.Rotate(0f, 0f, -270f * Time.deltaTime);

                m_RectTransform.localScale =
                    new Vector3(
                        1f + 0.5f * (deltaTime / 0.5f),
                        1f + 0.5f * (deltaTime / 0.5f), 1f);

                deltaTime += Time.deltaTime;

                yield return null;
            }

            Destroy(gameObject);
        }

        #region STATIC_FUNCTIONS

        public static void Init()
        {
            if (s_Initialized)
                return;

            Gem.onCreate.AddListener(OnCreateGem);

            s_Initialized = true;
        }

        private static void OnCreateGem(Gem newGem)
        {
            var newGemMono = CreateBaseGameObject<GemMono>(newGem);

            SubscribeToEvents(newGemMono);
        }

        protected static GemMono CreateBaseGameObject<T>(Gem gem) where T : GemMono
        {
            var newGameObject = Instantiate(CombatManager.self.combatUiInformation.gemGameObjectPrefab);
            var newGemMono = newGameObject.AddComponent<T>();

            // Connect Gem to GemMono
            newGemMono.gem = gem;

            newGemMono.m_RectTransform = newGameObject.GetComponent<RectTransform>();

            var gemImages = newGameObject.GetComponentsInChildren<GemImage>();
            var backgroundImage =
                gemImages.FirstOrDefault(gemImage => gemImage.imageType == GemImage.ImageType.Background);
            var midgroundImage =
                gemImages.FirstOrDefault(gemImage => gemImage.imageType == GemImage.ImageType.Midground);
            var foregroundImage =
                gemImages.FirstOrDefault(gemImage => gemImage.imageType == GemImage.ImageType.Foreground);

            if (backgroundImage != null)
            {
                backgroundImage.gem = gem;
                gem.components.Add(backgroundImage);
            }
            if (midgroundImage != null)
            {
                midgroundImage.gem = gem;
                gem.components.Add(midgroundImage);
            }
            if (foregroundImage != null)
            {
                foregroundImage.gem = gem;
                gem.components.Add(foregroundImage);
            }

            gem.components.Add(newGemMono);

            var gridMono = newGemMono.gridMono;
            newGemMono.m_RectTransform.SetParent(gridMono.transform, false);

            return newGemMono;
        }

        protected static void SubscribeToEvents(GemMono gemMono)
        {
            // Subscribe to the relevant events

            gemMono.grid.onMatch.AddListener(gemMono.OnMatch);
            gemMono.gridMono.onGridResize.AddListener(gemMono.OnGridResize);

            CombatManager.self.onCombatUpdate.AddListener(gemMono.OnCombatUpdate);
            CombatManager.self.onCombatLateUpdate.AddListener(gemMono.OnCombatLateUpdate);

            gemMono.gem.onPositionChange.AddListener(gemMono.OnPositionChange);
        }

        #endregion
    }
}
