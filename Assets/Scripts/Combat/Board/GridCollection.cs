namespace Combat.Board
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Information;

    using UnityEngine;
    using UnityEngine.Events;

    public enum SlideDirection
    {
        Forward,
        Backward,
    }

    [Serializable]
    public class CreateGridCollectionEvent : UnityEvent<GridCollection> { }

    [Serializable]
    public abstract class GridCollection
    {
        [SerializeField]
        protected int m_Index;

        protected readonly List<IComponent> m_Components = new List<IComponent>();

        public static readonly CreateGridCollectionEvent onCreate = new CreateGridCollectionEvent();

        public Grid grid { get; protected set; }
        public int index { get { return m_Index; } }

        public List<IComponent> components { get { return m_Components; } }

        public abstract IEnumerable<Gem> gems { get; }

        protected GridCollection() { }
        protected GridCollection(Grid newGrid, int newIndex) : this()
        {
            grid = newGrid;

            m_Index = newIndex;

            onCreate.Invoke(this);
        }

        protected abstract bool CopyAt(List<List<Gem>> tempList, int fromIndex, int toIndex);

        public bool Slide(SlideDirection direction)
        {
            var tempList =
                grid.gemLists.Select(gemList => gemList.gems.ToList()).ToList();

            var listCount = gems.Count();
            for (var i = 0; i < listCount; i++)
            {
                int nextIndex;
                switch (direction)
                {
                case SlideDirection.Forward:
                    nextIndex = i - 1;
                    if (nextIndex < 0)
                        nextIndex = listCount - 1;
                    break;
                case SlideDirection.Backward:
                    nextIndex = i + 1;
                    if (nextIndex > listCount - 1)
                        nextIndex = 0;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
                }

                var result = CopyAt(tempList, i, nextIndex);
                if (!result)
                    return false;
            }

            grid.onSlide.Invoke(new SlideInformation { gridCollection = this });
            return true;
        }

        public T GetComponent<T>() where T : IComponent
        {
            return (T)m_Components.FirstOrDefault(component => component is T);
        }
    }

    [Serializable]
    public class Column : GridCollection
    {
        private Column() { }
        public Column(Grid newGrid, int newIndex) : base(newGrid, newIndex) { }

        public override IEnumerable<Gem> gems
        {
            get { return grid.gemLists.Select(gemList => gemList[index]); }
        }

        protected override bool CopyAt(List<List<Gem>> tempList, int fromIndex, int toIndex)
        {
            var listCount = grid.gemLists[0].gems.Count;
            if (fromIndex >= listCount || fromIndex < 0 ||
                toIndex >= listCount || toIndex < 0)
                return false;

            grid.gemLists[toIndex][index] = tempList[fromIndex][index];

            grid.gemLists[toIndex][index].position = new Vector2(index, toIndex);
            return true;
        }
    }

    [Serializable]
    public class Row : GridCollection
    {
        private Row() { }
        public Row(Grid newGrid, int newIndex) : base(newGrid, newIndex) { }

        public override IEnumerable<Gem> gems
        {
            get { return grid.gemLists[index].gems; }
        }

        protected override bool CopyAt(List<List<Gem>> tempList, int fromIndex, int toIndex)
        {
            var listCount = grid.gemLists.Count;
            if (fromIndex >= listCount || fromIndex < 0 ||
                toIndex >= listCount || toIndex < 0)
                return false;

            grid.gemLists[index][toIndex] = tempList[index][fromIndex];

            grid.gemLists[index][toIndex].position = new Vector2(toIndex, index);
            return true;
        }
    }
}
