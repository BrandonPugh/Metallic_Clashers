/* Script Info - Script Name: ItemManager.cs, Created by: Brock Barlow, This script is used to handle items. */

namespace Items
{
    using System.Collections.Generic;

    using Combat;

    public class ItemManager
    {
        private List<BaseItem> m_ActiveList; //list of item(s) being used
        public List<BaseItem> inventory;     //list of item(s)
        public TurnBased item;

        public List<BaseItem> activeList { get { return m_ActiveList; } }

        public void Update()
        {
            activeList.RemoveAll(i => i.Alive == false);
        }

        public void SetItemActive()
        {
            //1) Check if the item is in the inventory.
                // put the item on the active list.
                // remove the item from the inventory list.
            //2) Use the item.
            //3) Determine the item type (attack or defense?)
            //4) Add listeners.

            if (inventory.Contains(item)) { activeList.Add(item); inventory.Remove(item); } else { return; }

            item.UseItem();

            //currently in progress...
            var itemType = item.GetType();

            if (itemType == typeof(TurnBased))
            {
                // Check Type of Item. Goal: Same as on outside of if statement.
                CombatManager.self.onPlayerTurn.AddListener(item.UpdateSelf);
            }

            else if(itemType == typeof(TimeBased))
            {
                CombatManager.self.onCombatUpdate.AddListener(item.UpdateSelf);
            }
        }
    }
}