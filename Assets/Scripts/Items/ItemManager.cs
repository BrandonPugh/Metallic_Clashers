/* Script Info - Script Name: ItemManager.cs, Created by: Brock Barlow, This script is used to handle items. */

namespace Items
{
    using System.Collections.Generic;
    using Combat;

    public class ItemManager
    {
        private List<BaseItem> m_ActiveList; //list of item(s) being used
        public List<BaseItem> activeList { get { return m_ActiveList; } }
        public List<BaseItem> inventory;     //list of item(s)
        public TurnBased item;

        public void SetCapacity() { inventory.Capacity = 15; activeList.Capacity = 6; }
        //sets the capacity of item lists. //15 and 6 are test values that will get changed later.

        public void Update() { activeList.RemoveAll(i => i.Alive == false); }
        //needs to be called after using an item.

        public void SetItemActive()
        {
            if (inventory.Contains(item)) { activeList.Add(item); inventory.Remove(item); } else { return; }

            item.UseItem();

            var itemType = item.GetType();

            if (itemType == typeof(InstantItem)) { Update(); }
            else if (itemType == typeof(TurnBased)) { CombatManager.self.onPlayerTurn.AddListener(item.UpdateSelf); Update(); }
            else if (itemType == typeof(TimeBased)) { CombatManager.self.onCombatUpdate.AddListener(item.UpdateSelf); Update(); }
        }

        public void SetItemNonActive() { if (activeList.Contains(item)) { inventory.Add(item); activeList.Remove(item); } }
        //if active list has any item(s) still after battle, remove them and add them back to the inventory list.
    }
}