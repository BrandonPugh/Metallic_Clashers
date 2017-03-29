using System;
using System.Collections.Generic;
using Combat.Board;
using UnityEngine;

namespace Combat
{
    [Serializable]
    public class Enemy : IAttachable
    {
        [SerializeField]
        private Attribute m_Health = new Attribute { value = 10f };
        [SerializeField]
        private Attribute m_Attack = new Attribute { value = 10f };
        [SerializeField]
        private Attribute m_Defense = new Attribute { value = 10f };

        public Attribute health { get { return m_Health; } }
        public Attribute attack { get { return m_Attack; } }
        public Attribute defense {get { return m_Defense; } }

        public float attackSpeed;public int movesUntilAttack;
        public GemType damageType;

        public List<GemType> resistances;
        public List<GemType> weaknesses;

        private int movesCounter = 0;
        private float attackCountdown;

        private readonly List<IComponent> m_Components = new List<IComponent>();

        public List<IComponent> components { get { return m_Components; } }

        public Enemy()
        {
            health.value = 10;
            health.coefficient = 1;

            attack.value = 10;
            attack.coefficient = 1;

            defense.value = 10;
            defense.coefficient = 1;

            attackSpeed = 5;
            attackCountdown = attackSpeed;

            movesUntilAttack = 3;
        }

        public Enemy(
            float newHealth,
            float newAttack,
            float newDefense,
            float newAttackSpeed,
            int newMovesUntilAttack)
        {
            health.value = newHealth;

            attack.value = newAttack;

            defense.value = newDefense;

            attackSpeed = newAttackSpeed;
            attackCountdown = attackSpeed;

            movesUntilAttack = newMovesUntilAttack;

        }

        public void OnCombatBegin()
        {
            CombatManager.self.onPlayerTurn.AddListener(OnPlayerTurn);
            //CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
        }

        private void OnCombatUpdate()
        {
            attackCountdown -= Time.deltaTime;

            if (attackCountdown > 0) return;
            Attack();
            attackCountdown = attackSpeed;
        }

        private void OnPlayerTurn()
        {
            movesCounter++;
            if (movesCounter != 0 && movesCounter % movesUntilAttack == 0)
            {
                Attack();
            }
        }

        private void Attack()
        {
            GameManager.self.playerData.TakeDamage(attack.totalValue, damageType);
        }

        public void TakeDamage(float damage, GemType gemType)
        {
            var percentage = damage / defense.totalValue;
            var finalDamage = damage * Mathf.Clamp(percentage, 0f, 1f);

            if (resistances != null && resistances.Contains(gemType))
            {
                finalDamage *= .75f;
            }

            else if (weaknesses != null && weaknesses.Contains(gemType))
            {
                finalDamage *= 1.25f;
            }

            health.modifier -= finalDamage;
        }
    }
}
