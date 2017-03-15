namespace Combat.Board
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Information;

    using UnityEngine;
    using UnityEngine.Events;

    public class GridResizeEvent : UnityEvent<GridResizeInformation> { }

    [RequireComponent(typeof(RectTransform))]
    public class GridMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private Grid m_Grid;

        [SerializeField]
        private RectTransform m_RectTransform;
        [SerializeField]
        private RectTransform m_ParentRectTransform;

        [SerializeField]
        private GridResizeEvent m_OnGridResize = new GridResizeEvent();

        private Vector2 m_PreviousRectSize;

        private bool m_GemsAreAnimating;
        private List<IEnumerator> m_GemMatchAnimations = new List<IEnumerator>();

        private bool m_GemsAreMatching;
        private IEnumerator m_WaitToCheckForMatch;

        public Grid grid { get { return m_Grid; } }

        public RectTransform rectTransform { get { return m_RectTransform; } }
        public RectTransform parentRectTransform { get { return m_ParentRectTransform; } }

        public GridResizeEvent onGridResize { get { return m_OnGridResize; } }

        public bool gemsAreAnimating { get { return m_GemsAreAnimating; } }
        public List<IEnumerator> gemMatchAnimations { get { return m_GemMatchAnimations; } }

        private void LateUpdate()
        {
            var currentRectSize = new Vector2(m_RectTransform.rect.width, m_RectTransform.rect.height);
            if (currentRectSize == m_PreviousRectSize)
                return;

            m_OnGridResize.Invoke(new GridResizeInformation { newRect = m_RectTransform.rect });

            m_PreviousRectSize = currentRectSize;
        }

        private void OnCombatUpdate()
        {
            if (m_GemMatchAnimations.Count != 0)
            {
                m_GemsAreAnimating = true;
                m_GemsAreMatching = true;

                var tempList = m_GemMatchAnimations.ToList();
                foreach (var gemMatchAnimation in tempList)
                    if (!gemMatchAnimation.MoveNext())
                        m_GemMatchAnimations.Remove(gemMatchAnimation);
            }
            else if (m_GemsAreMatching)
            {
                m_Grid.ApplyGravity();
                m_Grid.Fill();

                m_WaitToCheckForMatch = WaitToCheckForMatch();

                m_GemsAreMatching = false;
            }
            else if (m_WaitToCheckForMatch != null)
                m_WaitToCheckForMatch.MoveNext();
            else
                m_GemsAreAnimating = false;
        }

        private void OnPlayerTurn()
        {
            m_Grid.CheckMatch();
        }

        public Vector2 CalculateSpacing()
        {
            return
                new Vector2(
                    m_RectTransform.rect.width / (grid.size.x - 1),
                    m_RectTransform.rect.height / (grid.size.y - 1));
        }

        private IEnumerator WaitToCheckForMatch()
        {
            while (
                grid.gemLists.Any(
                    gemList => gemList.gems.Where(
                        gem => gem != null).Any(
                        gem => gem.GetComponent<GemMono>().moveToPositionCoroutine != null)))
            {
                yield return null;
            }

            m_Grid.CheckMatch();

            m_WaitToCheckForMatch = null;
        }

        public static void Init()
        {
            Grid.onCreate.AddListener(OnGridCreate);
        }

        private static void OnGridCreate(Grid newGrid)
        {
            var newGameObject = new GameObject();
            newGameObject.transform.SetParent(CombatManager.self.gridParentRectTransform, false);

            var newGridMono = newGameObject.AddComponent<GridMono>();

            newGridMono.m_Grid = newGrid;
            newGrid.components.Add(newGridMono);

            newGridMono.m_RectTransform = newGameObject.GetComponent<RectTransform>();
            newGridMono.m_ParentRectTransform =
                newGameObject.GetComponentsInParent<RectTransform>().First(
                    rectTransform => rectTransform != newGridMono.m_RectTransform);

            newGridMono.m_RectTransform.anchorMin = new Vector2(0.1f, 0.1f);
            newGridMono.m_RectTransform.anchorMax = new Vector2(0.9f, 0.9f);
            newGridMono.m_RectTransform.sizeDelta = Vector2.zero;

            newGridMono.m_RectTransform.anchoredPosition = Vector2.zero;

            CombatManager.self.onCombatUpdate.AddListener(newGridMono.OnCombatUpdate);
            CombatManager.self.onPlayerTurn.AddListener(newGridMono.OnPlayerTurn);
        }
    }
}
