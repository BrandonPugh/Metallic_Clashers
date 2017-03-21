/* Script Info - Script Name: BaseItem.cs, Created by: Brock Barlow, This script is used to handle items. */

namespace Items
{
    using System;

    [Serializable]
    public class BaseItem
    {
        protected bool m_Alive;
        protected float m_Age; //current durabilities max value.
        public float durability; //how long the item will last.
        public float modifier; //value that will "modify" player attribute(s).

        public bool Alive
        {
            get { return m_Alive; }
        }
        public virtual void UseItem() {}
    }

    [Serializable]
    public class InstantItem : BaseItem //just remove, not destroy
    {
        public InstantItem(float mod)
        {
            modifier = mod;
        }

        public override void UseItem()
        {
            var healValue = GameManager.self.playerData.health.value * modifier;
            GameManager.self.playerData.health.modifier += healValue;
            if (GameManager.self.playerData.health.modifier > 0)
            {
                GameManager.self.playerData.health.modifier = 0;
            }
            m_Alive = false;
        }
    }

    [Serializable]
    public class TurnBased : BaseItem //possible void type needed for event
    {
        public virtual void UpdateSelf()
        {
            m_Age++;
        }
    }

    [Serializable]
    public class TurnBuff : TurnBased
    {
        public bool itemStatType;

        public TurnBuff(float dur, float mod, bool type)
        {
            durability = dur;
            modifier = mod;
            itemStatType = type;
        }

        public override void UseItem()
        {
            switch (itemStatType)
            {
            case true: //attack buff case
                var attackValue = GameManager.self.playerData.attack.value * modifier;
                GameManager.self.playerData.attack.modifier += attackValue;
                break;
            case false: //defense buff case
                var defenseValue = GameManager.self.playerData.defense.value * modifier;
                GameManager.self.playerData.defense.modifier += defenseValue;
                break;
            }
        }

        public override void UpdateSelf()
        {
            base.UpdateSelf();

            if (m_Age < durability)
                m_Alive = true;

            switch (itemStatType)
            {
            case true:
                GameManager.self.playerData.attack.modifier -= GameManager.self.playerData.attack.value * modifier;
                m_Alive = false;
                break;

            case false:
                GameManager.self.playerData.defense.modifier -= GameManager.self.playerData.defense.value * modifier;
                m_Alive = false;
                break;

            default:
                m_Alive = false;
                break;
            }
        }
    }

    [Serializable]
    public class TimeBased : BaseItem //possible void type needed for event
    {
        public virtual void UpdateSelf(float value)
        {
            m_Age += value;
        }
    }

    [Serializable]
    public class TimeBuff : TimeBased
    {
        public bool itemStatType;

        public TimeBuff(float dur, float mod, bool type)
        {
            durability = dur;
            modifier = mod;
            itemStatType = type;
        }

        public override void UseItem()
        {
            switch (itemStatType)
            {
            case true: //attack buff case
                var attackValue = GameManager.self.playerData.attack.value * modifier;
                GameManager.self.playerData.attack.modifier += attackValue;
                break;
            case false: //defense buff case
                var defenseValue = GameManager.self.playerData.defense.value * modifier;
                GameManager.self.playerData.defense.modifier += defenseValue;
                break;
            }
        }

        public override void UpdateSelf(float deltaTime)
        {
            base.UpdateSelf(deltaTime);
            if (m_Age < durability)
                m_Alive = true;

            switch (itemStatType)
            {
            case true:
                GameManager.self.playerData.attack.modifier -= GameManager.self.playerData.attack.value * modifier;
                m_Alive = false;
                break;

            case false:
                GameManager.self.playerData.defense.modifier -= GameManager.self.playerData.defense.value * modifier;
                m_Alive = false;
                break;

            default:
                m_Alive = false;
                break;
            }
        }
    }
}