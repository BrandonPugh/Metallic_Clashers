/* Script Info - Script Name: BaseItem.cs, Created by: Brock Barlow, This script is used to handle items. */

namespace Items
{
    using System;
    using UnityEngine;

    [Serializable]
    public class BaseItem
    {
        protected bool m_Alive;  //states if the item can be destroied or not
        protected float m_Age;   //current durabilities max value.
        public float durability; //how long the item will last.
        public float modifier;   //value that will "modify" player attribute(s).
        public bool Alive { get { return m_Alive; } }

        public virtual void UseItem() {}
        public virtual void UpdateSelf() {}
    }

    [Serializable]
    public class InstantItem : BaseItem
    {
        public InstantItem(float mod) { modifier = mod; }

        public override void UseItem()
        {
            var healValue = GameManager.self.playerData.health.value * modifier;
            GameManager.self.playerData.health.modifier += healValue;
            if (GameManager.self.playerData.health.modifier > 0) { GameManager.self.playerData.health.modifier = 0; }
            m_Alive = false;
        }
    }
    //for instant items, just remove them. do not destroy them.

    [Serializable]
    public class TurnBased : BaseItem { public override void UpdateSelf() { m_Age++; } }

    [Serializable]
    public class TurnBuff : TurnBased
    {
        public bool itemStatType;

        public TurnBuff(float dur, float mod, bool type) { durability = dur; modifier = mod; itemStatType = type; }

        public override void UseItem()
        {
            switch (itemStatType)
            {
            case true:  //attack buff case
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

            if (m_Age < durability) { m_Alive = true; }
                
            switch (itemStatType)
            {
            case true:  //attack buff case
                GameManager.self.playerData.attack.modifier -= GameManager.self.playerData.attack.value * modifier;
                m_Alive = false;
                break;
            case false: //defense buff case
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
    public class TimeBased : BaseItem { public override void UpdateSelf() { m_Age += Time.deltaTime; } }

    [Serializable]
    public class TimeBuff : TimeBased
    {
        public bool itemStatType;

        public TimeBuff(float dur, float mod, bool type) { durability = dur; modifier = mod; itemStatType = type; }

        public override void UseItem()
        {
            switch (itemStatType)
            {
            case true:  //attack buff case
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

            if (m_Age < durability) { m_Alive = true; }
                
            switch (itemStatType)
            {
            case true:  //attack buff case
                GameManager.self.playerData.attack.modifier -= GameManager.self.playerData.attack.value * modifier;
                m_Alive = false;
                break;
            case false: //defense buff case
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