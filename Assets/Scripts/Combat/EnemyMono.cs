namespace Combat
{
    using UnityEngine;

    using Board;
    public class EnemyMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private Enemy m_Enemy;

        public Enemy enemy { get { return m_Enemy; } }
    }
}
