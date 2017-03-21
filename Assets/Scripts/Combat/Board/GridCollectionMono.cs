namespace Combat.Board
{
    using System.Collections;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class GridCollectionMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private Image m_Image;

        [SerializeField]
        private GridCollection m_GridCollection;

        private static bool s_Initialized;

        public GridCollection gridCollection { get { return m_GridCollection; } }

        private Vector2 m_PositionOffset;
        private Vector2 m_CurrentDirection;
        private float m_ReducePositionOffsetTime = 1f;
        private IEnumerator m_ReducePositionOffsetCoroutine;

        public Grid grid { get { return m_GridCollection.grid; } }
        public GridMono gridMono { get { return grid.GetComponent<GridMono>(); } }

        public Vector2 positionOffset
        {
            get { return m_PositionOffset; }
            set
            {
                m_PositionOffset = value;
                m_CurrentDirection =
                    Mathf.Abs(m_PositionOffset.x) > Mathf.Abs(m_PositionOffset.y)
                    ? m_PositionOffset.x > 0f
                        ? Vector2.right : Vector2.left
                    : m_PositionOffset.y > 0f
                        ? Vector2.up : Vector2.down;

                m_ReducePositionOffsetCoroutine = ReducePositionOffset();
            }
        }
        public Vector2 currentDirection { get { return m_CurrentDirection; } }

        private void OnCombatUpdate()
        {
            if (m_ReducePositionOffsetCoroutine != null && m_ReducePositionOffsetCoroutine.MoveNext()) ;
            else { m_ReducePositionOffsetCoroutine = null; }
        }

        private void CheckForSlide()
        {
            var spacing = gridMono.CalculateSpacing();
            if (Mathf.Abs(m_PositionOffset.x) <= spacing.x / 2f &&
                Mathf.Abs(m_PositionOffset.y) <= spacing.y / 2f)
                return;

            var gemMonos = gridCollection.gems.Select(rowGem => rowGem.GetComponent<GemMono>()).ToList();

            foreach (var gemMono in gemMonos)
            {
                var newPosition = gemMono.CalculatePosition(gemMono.position + m_CurrentDirection);
                gemMono.currentPosition = newPosition;
            }

            gridCollection.Slide(
                m_CurrentDirection == Vector2.right || m_CurrentDirection == Vector2.up
                    ? SlideDirection.Backward : SlideDirection.Forward);

            m_PositionOffset -=
                gridCollection is Row
                    ? new Vector2(spacing.x * m_CurrentDirection.x, 0f)
                    : new Vector2(0f, spacing.y * m_CurrentDirection.y);

            m_CurrentDirection = Vector2.zero;
        }
        private IEnumerator ReducePositionOffset()
        {
            CheckForSlide();

            var gemMonos =
                gridCollection.gems.Where(
                    gem => gem != null).SelectMany(
                        gem => gem.GetComponents<GemMono>()).ToList();

            foreach (var gem in gemMonos)
                gem.positionIsDirty = true;

            yield return null;

            var deltaTime = 0f;
            while (deltaTime < m_ReducePositionOffsetTime)
            {
                var spacing = gridMono.CalculateSpacing();

                if (Mathf.Abs(m_PositionOffset.x) > spacing.x / 2f ||
                    Mathf.Abs(m_PositionOffset.y) > spacing.y / 2f)
                {
                    var newOffset =
                        new Vector2(
                            spacing.x * m_CurrentDirection.x,
                            spacing.y * m_CurrentDirection.y);
                    m_PositionOffset =
                        Vector2.Lerp(m_PositionOffset, newOffset, deltaTime / m_ReducePositionOffsetTime);
                }
                else
                {
                    m_PositionOffset =
                        Vector2.Lerp(m_PositionOffset, Vector2.zero, deltaTime / m_ReducePositionOffsetTime);
                }

                CheckForSlide();

                gemMonos =
                    gridCollection.gems.Where(
                        gem => gem != null).SelectMany(
                            gem => gem.GetComponents<GemMono>()).ToList();

                foreach (var gem in gemMonos)
                    gem.positionIsDirty = true;

                deltaTime += Time.deltaTime;

                yield return null;
            }

            m_PositionOffset = Vector2.zero;
            m_CurrentDirection = Vector2.zero;

            gemMonos =
                gridCollection.gems.Where(
                    gem => gem != null).SelectMany(
                        gem => gem.GetComponents<GemMono>()).ToList();

            foreach (var gem in gemMonos)
                gem.positionIsDirty = true;
        }

        public static void Init()
        {
            if (s_Initialized)
                return;

            GridCollection.onCreate.AddListener(OnCreateGridCollection);
            s_Initialized = true;
        }

        private static void OnCreateGridCollection(GridCollection newGridCollection)
        {
            var gridMono =
                newGridCollection.grid.components.First(component => component is GridMono) as GridMono;

            if (gridMono == null)
                return;

            var newGameObject = new GameObject(newGridCollection.GetType() + " " + newGridCollection.index);
            newGameObject.transform.SetParent(
                newGridCollection is Row ?
                    CombatManager.self.rowParent.transform : CombatManager.self.columnParent.transform,
                false);

            if (newGridCollection is Row)
                newGameObject.transform.SetAsFirstSibling();

            var newGridCollectionMono = newGameObject.AddComponent<GridCollectionMono>();

            newGridCollectionMono.m_GridCollection = newGridCollection;
            newGridCollection.components.Add(newGridCollectionMono);

            newGridCollectionMono.m_Image = newGameObject.GetComponent<Image>();
            newGridCollectionMono.m_Image.color = new Color(0f, 0f, 0f, 0f);

            CombatManager.self.onCombatUpdate.AddListener(newGridCollectionMono.OnCombatUpdate);
        }
    }
}
