/* Script Info - Script Name: ItemManager.cs, Created by: Brock Barlow, This script is used to handle items. */

namespace Items
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Combat;

    [Serializable]
    public class ItemManager
    {
        private List<BaseItem> m_Inventory;   //list of item(s)
        private List<BaseItem> m_ActiveList;  //list of item(s) being used
        private List<BaseItem> m_CombatInventory; //list of item(s) being taken in to combat
        public List<BaseItem> inventory { get { return m_Inventory; } }
        [XmlIgnore] public List<BaseItem> activeList { get { return m_ActiveList; } }
        [XmlIgnore] public List<BaseItem> combatInventory { get { return m_CombatInventory; } }

        public void ItemUpdate() { activeList.RemoveAll(i => i.Alive == false); } //needs to be called after using an item.

        public void AddItem(BaseItem item)
        {
            if (inventory.Contains(item)) { combatInventory.Add(item); inventory.Remove(item); }
        }

        public void SetItemActive(BaseItem item)
        {
            if (combatInventory.Contains(item)) { activeList.Add(item); combatInventory.Remove(item); } else { return; }

            item.UseItem();

            var itemType = item.GetType();

            if (itemType == typeof(TurnBased)) { CombatManager.self.onPlayerTurn.AddListener(item.UpdateSelf); }
            else if (itemType == typeof(TimeBased)) { CombatManager.self.onCombatUpdate.AddListener(item.UpdateSelf); }
        }

        public void SetItemNonActive(BaseItem item)
        {
            if (activeList.Contains(item)) { combatInventory.Add(item); activeList.Remove(item); }
            if (combatInventory.Contains(item)) { inventory.Add(item); combatInventory.Remove(item); }
        }
    }
}