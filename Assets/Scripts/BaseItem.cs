/* Script Info - Script Name: BaseItem.cs, Created by: Brock Barlow, This script is used to handle items. */

using System;

[Serializable]
public class BaseItem
{
    protected float m_Age;   //current durabilities max value.
    public float durability; //how long the item will last.
    public float modifier;   //value that will "modify" player attribute(s).

    public virtual void UseItem() { } 
}

[Serializable]
public class InstantItem : BaseItem 
{
    public InstantItem() { modifier = .15f; }

    public override void UseItem()
    {
        var healValue = GameManager.self.playerData.health.value * modifier;
        GameManager.self.playerData.health.modifier += healValue;
        if (GameManager.self.playerData.health.modifier > 0) { GameManager.self.playerData.health.modifier = 0; }
        //TODO. Destroy health item.
    }
}

[Serializable]
public class TurnBased : BaseItem 
{   
    public virtual bool UpdateSelf() { m_Age++; return true; } 
}

[Serializable]
public class TurnBuff : TurnBased  //TODO. Get feedback here.
{
    public int itemStatType = 2;
    public bool caseTwo, caseOne;

    public TurnBuff() { durability = 10; modifier = .1f; } 

    public override void UseItem()
    {
        switch (itemStatType)
        {
            case 2: //attack buff case
                caseTwo = true;
                var attackValue = GameManager.self.playerData.attack.value * modifier;
                GameManager.self.playerData.attack.modifier += attackValue;
                break;
            case 1: //defense buff case
                caseOne = true;
                var defenseValue = GameManager.self.playerData.defense.value * modifier;
                GameManager.self.playerData.defense.modifier += defenseValue;
                break;
            default:
                break;
        }
    }

    public override bool UpdateSelf()
    {
        base.UpdateSelf();
        if (m_Age > durability & caseTwo == true) 
        {
            GameManager.self.playerData.attack.modifier -= modifier;
            //TODO. Destroy health item.
            return false;
        }
        if (m_Age > durability & caseOne == true)
        {
            GameManager.self.playerData.defense.modifier -= modifier;
            //TODO. Destroy health item.
            return false;
        }
        return true;
    }
}

[Serializable]
public class TimeBased : BaseItem 
{
    public virtual bool UpdateSelf(float value) { m_Age += value; return true; } 
}

[Serializable]
public class TimeBuff : TimeBased  //TODO. Get feedback here.
{
    public int itemStatType = 2;
    public bool caseTwo, caseOne;
    public float counter = 10; 

    public TimeBuff() { durability = 10; modifier = .1f; }

    public override void UseItem()
    {
        switch (itemStatType)
        {
            case 2: //attack buff case
                caseTwo = true;
                var attackValue = GameManager.self.playerData.attack.value * modifier;
                GameManager.self.playerData.attack.modifier += attackValue;
                break;
            case 1: //defense buff case
                caseOne = true;
                var defenseValue = GameManager.self.playerData.defense.value * modifier;
                GameManager.self.playerData.defense.modifier += defenseValue;
                break;
            default:
                break;
        }
    }

    public override bool UpdateSelf(float counter)
    {
        base.UpdateSelf(counter);
        if (m_Age > durability & caseTwo == true)
        {
            GameManager.self.playerData.attack.modifier -= modifier;
            //TODO. Destroy health item.
            return false;
        }
        if (m_Age > durability & caseOne == true)
        {
            GameManager.self.playerData.defense.modifier -= modifier;
            //TODO. Destroy health item.
            return false;
        }
        return true;
    }
}