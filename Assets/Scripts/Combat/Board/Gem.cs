namespace Combat.Board
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Information;

    using JetBrains.Annotations;

    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class PositionChangeEvent : UnityEvent<PositionChangeInformation> { }
    [Serializable]
    public class TypeChangeEvent : UnityEvent<TypeChangeInformation> { }
    [Serializable]
    public class CreateGemEvent : UnityEvent<Gem> { }

    public enum GemType
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Purple,
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    [Serializable]
    public class Gem : IAttachable
    {
        #region SERIALIZED_FIELDS

        [SerializeField]
        private GemType m_GemType;

        [SerializeField]
        private Vector2 m_Position;

        [SerializeField]
        private TypeChangeEvent m_OnTypeChange = new TypeChangeEvent();
        [SerializeField]
        private PositionChangeEvent m_OnPositionChange = new PositionChangeEvent();

        #endregion

        private readonly List<IComponent> m_Components = new List<IComponent>();

        public static readonly CreateGemEvent onCreate = new CreateGemEvent();

        #region PUBLIC_PROPERTIES

        public Grid grid { get; private set; }

        public GemType gemType
        {
            get { return m_GemType; }
            set
            {
                m_GemType = value;

                m_OnTypeChange.Invoke(new TypeChangeInformation { gem = this, newType = value });
            }
        }

        public Vector2 position
        {
            get { return m_Position; }
            set
            {
                m_Position = value;

                m_OnPositionChange.Invoke(new PositionChangeInformation { gem = this, newPosition = value });
            }
        }

        public TypeChangeEvent onTypeChange { get { return m_OnTypeChange; } }
        public PositionChangeEvent onPositionChange { get { return m_OnPositionChange; } }

        public List<IComponent> components { get { return m_Components; } }

        public Row row { get { return grid.rows[(int)position.y]; } }
        public Column column { get { return grid.columns[(int)position.x]; } }

        public Gem up { get { return GetNeighbor(Direction.Up); } }
        public Gem down { get { return GetNeighbor(Direction.Down); } }
        public Gem left { get { return GetNeighbor(Direction.Left); } }
        public Gem right { get { return GetNeighbor(Direction.Right); } }

        #endregion

        private Gem() { }

        public Gem(Grid newGrid, Vector2 newPosition, GemType newGemType) : this()
        {
            grid = newGrid;

            m_Position = newPosition;
            m_GemType = newGemType;

            onCreate.Invoke(this);
        }

        [CanBeNull]
        public Gem GetNeighbor(Direction direction, bool clamp = false)
        {
            var nextPosition = m_Position;

            switch (direction)
            {
            case Direction.Up:
                nextPosition += Vector2.up;
                break;
            case Direction.Down:
                nextPosition += Vector2.down;
                break;
            case Direction.Left:
                nextPosition += Vector2.left;
                break;
            case Direction.Right:
                nextPosition += Vector2.right;
                break;

            default:
                throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            if (clamp)
            {
                nextPosition = grid.ClampPosition(nextPosition);

                return grid[(int)nextPosition.y][(int)nextPosition.x];
            }

            return
                nextPosition == grid.ClampPosition(nextPosition) ?
                grid[(int)nextPosition.y][(int)nextPosition.x] : null;
        }
    }

    public static class DirectionExtentions
    {
        public static Direction Reverse(this Direction direction)
        {
            switch (direction)
            {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            default:
                throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }
    }
}
