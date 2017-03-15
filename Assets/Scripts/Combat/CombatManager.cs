namespace Combat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Input.Information;

    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using Board;
    using Board.Information;

    using UnityEngine.Serialization;

    public class CombatManager : SubManager<CombatManager>
    {
        public enum CombatMode
        {
            Attack,
            Defense,
        }

        [Serializable]
        public class UnityBoolEvent : UnityEvent<bool> { }

        [Serializable]
        public class CombatUiInformation
        {
            [Serializable]
            public class ModeUiInformation
            {
                public Sprite backgroundImage;
                public Sprite midgroundImage;
                public Sprite foregroundImage;

                public Color modeColor;
            }

            public ModeUiInformation m_AttackModeUiInformation;
            public ModeUiInformation m_DefenseModeUiInformation;

            [Space]
            public List<Color> standardGemColors = new List<Color>();
            public List<Color> alternativeGemColors = new List<Color>();

            [Space, SerializeField]
            private bool m_UseAlternativeColors;

            [Space, SerializeField]
            private UnityBoolEvent m_OnUseAlternativeColorsChange = new UnityBoolEvent();

            public List<Color> gemColors
            {
                get { return m_UseAlternativeColors ? alternativeGemColors : standardGemColors; }
            }

            public ModeUiInformation currentModeUiInformation
            {
                get
                {
                    switch (self.combatMode)
                    {
                    case CombatMode.Attack:
                        return m_AttackModeUiInformation;
                    case CombatMode.Defense:
                        return m_DefenseModeUiInformation;

                    default:
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public bool useAlternativeColors
            {
                get { return m_UseAlternativeColors; }
                set { m_UseAlternativeColors = value; m_OnUseAlternativeColorsChange.Invoke(value); }
            }

            public UnityBoolEvent onUseAlternativeColorsChange
            {
                get { return m_OnUseAlternativeColorsChange; }
            }
        }

        [SerializeField]
        private Canvas m_Canvas;
        [SerializeField]
        private RectTransform m_GridParentRectTransform;
        [SerializeField]
        private VerticalLayoutGroup m_RowParent;
        [SerializeField]
        private HorizontalLayoutGroup m_ColumnParent;

        [Space, SerializeField]
        private CombatUiInformation m_CombatUiInformation;

        [SerializeField]
        private CombatMode m_CombatMode;

        [Space, SerializeField]
        private UnityEvent m_OnCombatBegin = new UnityEvent();
        [SerializeField]
        private UnityEvent m_OnCombatUpdate = new UnityEvent();
        [SerializeField]
        private UnityEvent m_OnCombatLateUpdate = new UnityEvent();
        [SerializeField]
        private UnityEvent m_OnCombatEnd = new UnityEvent();

        [Space, SerializeField]
        private UnityEvent m_OnCombatModeChange = new UnityEvent();

        [Space, SerializeField]
        private UnityBoolEvent m_OnCombatPauseChange = new UnityBoolEvent();

        [SerializeField]
        private UnityEvent m_OnPlayerTurn = new UnityEvent();

        [SerializeField]
        private GridMono m_GridMono;

        private GridCollectionMono m_LockedGridCollectionMono;

        private bool m_CombatHasBegun;
        private bool m_IsPaused;

        private bool m_HasSlid;

        public Canvas canvas { get { return m_Canvas; } }
        public RectTransform gridParentRectTransform { get { return m_GridParentRectTransform; } }
        public VerticalLayoutGroup rowParent { get { return m_RowParent; } }
        public HorizontalLayoutGroup columnParent { get { return m_ColumnParent; } }

        public CombatUiInformation combatUiInformation { get { return m_CombatUiInformation; } }

        public CombatMode combatMode { get { return m_CombatMode; } }

        //TODO: public List<Enemy> enemies = new List<>;

        public UnityEvent onCombatBegin { get { return m_OnCombatBegin; } }
        public UnityEvent onCombatUpdate { get { return m_OnCombatUpdate; } }
        public UnityEvent onCombatLateUpdate { get { return m_OnCombatLateUpdate; } }
        public UnityEvent onCombatEnd { get { return m_OnCombatEnd; } }

        public UnityEvent onCombatModeChange { get { return m_OnCombatModeChange; } }

        public bool isPaused
        {
            get { return m_IsPaused; }
            set { m_IsPaused = value; m_OnCombatPauseChange.Invoke(value); }
        }
        public UnityBoolEvent onCombatPauseChange { get { return m_OnCombatPauseChange; } }


        public UnityEvent onPlayerTurn { get { return m_OnPlayerTurn; } }

        public GridMono gridMono { get { return m_GridMono; } }

        protected override void Init()
        {
            if (m_Canvas == null)
                m_Canvas = FindObjectOfType<Canvas>();

            //TODO: Initialize Combat
            if (m_GridParentRectTransform == null)
                m_GridParentRectTransform = m_Canvas.GetComponent<RectTransform>();

            GridMono.Init();
            GridCollectionMono.Init();

            GemMono.Init();
            GemMonoDuplicate.Init();

            var newGrid = new Grid(new Vector2(5f, 5f));

            m_GridMono = newGrid.GetComponent<GridMono>();

            m_GridMono.grid.onSlide.AddListener(OnSlide);
        }

        private void Update()
        {
            if (m_CombatHasBegun == false)
            {
                m_OnCombatBegin.Invoke();

                m_CombatHasBegun = true;
            }

            //if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            //   m_IsPaused = !m_IsPaused;

            if (!m_IsPaused)
                m_OnCombatUpdate.Invoke();

            //if (m_LockedGridCollectionMono == null)
            //    return;

            //foreach (var gem in m_LockedGridCollectionMono.gridCollection.gems)
            //{
            //    var gemMono = gem.GetComponent<GemMono>();

            //    gemMono.positionOffset += new Vector2(0.5f, 0f);
            //}
        }

        private void LateUpdate()
        {
            if (!m_IsPaused)
                m_OnCombatLateUpdate.Invoke();
        }

        public void ToggleCombatMode()
        {
            switch (m_CombatMode)
            {
            case CombatMode.Attack:
                m_CombatMode = CombatMode.Defense;
                break;
            case CombatMode.Defense:
                m_CombatMode = CombatMode.Attack;
                break;

            default:
                throw new ArgumentOutOfRangeException();
            }

            m_OnCombatModeChange.Invoke();
        }

        private void OnSlide(SlideInformation slideInfo)
        {
            m_HasSlid = true;
        }

        protected override void OnBeginDrag(DragInformation dragInfo)
        {
            var hitMonos = RayCastToGridCollectionMono(dragInfo.origin).ToList();

            // If we didn't hit a GemMono first
            if (hitMonos.Count == 0)
            {
                m_LockedGridCollectionMono = null;
                return;
            }

            m_LockedGridCollectionMono =
                hitMonos.First(
                    hitMono =>
                        Mathf.Abs(dragInfo.totalDelta.x) > Mathf.Abs(dragInfo.totalDelta.y)
                            ? hitMono.gridCollection is Row : hitMono.gridCollection is Column);
        }

        protected override void OnDrag(DragInformation dragInfo)
        {
            if (gridMono.gemsAreAnimating)
                return;

            // If we didn't hit a GridCollectionMono at the start of the drag
            if (m_LockedGridCollectionMono == null)
                return;

            var addedPositionOffset =
                new Vector2(
                    m_LockedGridCollectionMono.gridCollection is Row
                        ? dragInfo.delta.x / m_Canvas.scaleFactor : 0f,

                    m_LockedGridCollectionMono.gridCollection is Column
                        ? dragInfo.delta.y / m_Canvas.scaleFactor : 0f);

            m_LockedGridCollectionMono.positionOffset += addedPositionOffset;
        }

        protected override void OnEndDrag(DragInformation dragInfo)
        {
            if (!m_HasSlid)
                return;

            onPlayerTurn.Invoke();

            m_HasSlid = false;
        }

        private static IEnumerable<GridCollectionMono> RayCastToGridCollectionMono(Vector2 origin)
        {
            var pointerEventData =
                new PointerEventData(EventSystem.current) { position = origin };

            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, hits);

            // If nothing was hit
            if (hits.Count <= 0)
                return null;

            // Return the first hit object's GridCollectionMono component
            // Will be null if one was not found on the game object
            return
                hits.
                    Where(
                        hit => hit.gameObject.GetComponent<GridCollectionMono>() != null).
                    Select(
                        hit => hit.gameObject.GetComponent<GridCollectionMono>());
        }

        [CanBeNull]
        private static GemMono RaycastToGemMono(Vector2 origin)
        {
            var pointerEventData =
                new PointerEventData(EventSystem.current) { position = origin };

            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, hits);

            // If nothing was hit
            if (hits.Count <= 0)
                return null;

            var firstHit = hits.First();

            // Return the first hit object's GemMono component
            // Will be null if one was not found on the game object
            return firstHit.gameObject.GetComponent<GemMono>();
        }

        [CanBeNull]
        private static Gem RaycastToGem(Vector2 origin)
        {
            var gemMono = RaycastToGemMono(origin);

            // If we didn't hit a GemMono first
            return gemMono ? gemMono.gem : null;
        }

        [ContextMenu("Test Match")]
        public void TestMatch()
        {
            m_GridMono.grid.CheckMatch();
        }
    }
}
