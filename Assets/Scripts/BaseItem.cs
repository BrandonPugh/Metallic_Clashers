/* Script Info - Script Name: BaseItem.cs, Created by: Brock Barlow, This script is used to handle items. */

using System;

[Serializable]
public class BaseItem
{
    protected float m_Age; //current durabilities max value.
    
    public float durability; //how long the item will last.
    public float modifier; //value that will "modify" player attribute(s).

    public virtual void UseItem() { } //no defines due to overrides.
}

[Serializable]
public class InstantItem : BaseItem 
{
    //instant item idea: heal up item. will restore the player's "missing health" value. effect will be instant, no lasting/timed effect.
    //tiers of item: minor, standard, major.

    public float minorHeal = .1f; //10%
    public float standardHeal = .25f; //25%
    public float majorHeal = .5f; //50%
    public int healStrength = 3;
 
    public override void UseItem()
    {
        switch (healStrength)
        {
            case 3: //minor heal case.
                modifier = GameManager.self.playerData.health.coefficient * minorHeal;
                GameManager.self.playerData.health.modifier += modifier;
                if (GameManager.self.playerData.health.modifier > 0) { GameManager.self.playerData.health.modifier = 0; }
                break;
            case 2: //standard heal case.
                modifier = GameManager.self.playerData.health.coefficient * standardHeal;
                GameManager.self.playerData.health.modifier += modifier;
                if (GameManager.self.playerData.health.modifier > 0) { GameManager.self.playerData.health.modifier = 0; }
                break;
            case 1: //major heal case.
                modifier = GameManager.self.playerData.health.coefficient * majorHeal;
                GameManager.self.playerData.health.modifier += modifier;
                if (GameManager.self.playerData.health.modifier > 0) { GameManager.self.playerData.health.modifier = 0; }
                break;
            default:
                break;
        }
    }
}

[Serializable]
public class TurnBased : BaseItem //these items are based on the amount of turns the player makes during play.
{   
    public virtual void UpdateSelf() { m_Age++; } //increment age.
}

[Serializable]
public class TurnBuff : TurnBased
{
    //turn item idea: attack increase item. will raise the player's attack value.
    //effect will have a lasting/timed effect (based off turns).
    //
    //turn item idea: defense increase item. will raise the player's defense value.
    //effect will have a lasting/timed effect (based off turns).

    public float buffValue = .1f; //10%
    public float amountOfTurns = 10;
    public int itemStatType = 2;

    public override void UseItem()
    {
        switch (itemStatType)
        {
            case 2: //this case is for the attack stat buff.
                durability = amountOfTurns;
                modifier = GameManager.self.playerData.attack.coefficient * buffValue;
                GameManager.self.playerData.attack.modifier += modifier;
                if (m_Age > durability) { GameManager.self.playerData.attack.modifier -= modifier; }
                break;
            case 1: //this case is for the defense stat buff.
                durability = amountOfTurns;
                modifier = GameManager.self.playerData.defense.coefficient * buffValue;
                GameManager.self.playerData.defense.coefficient += modifier;
                if (m_Age > durability) { GameManager.self.playerData.attack.modifier -= modifier; }
                break;
            default:
                break;
        }
    }

    public override void UpdateSelf()
    {
        base.UpdateSelf();
        if (m_Age >= durability)
        {
            //DO stuff. Get rid of whatever was changed.
            //Destroy
        }
    }
}

[Serializable]
public class TimeBased : BaseItem //these items are based on a timer when activated by the player during play.
{
    public void UpdateSelf(float value) { m_Age += value; } //modify age with float value.
}

[Serializable]
public class TimeItem : TimeBased
{
    //time item idea: attack increase item. will raise the player's attack value.
    //effect will have a lasting/timed effect (based off time).
    //
    //time item idea: defense increase item. will raise the player's defense value.
    //effect will have a lasting/timed effect (based off time).

    public int itemStatType = 2;

    public override void UseItem()
    {
        var tempAttackValue = GameManager.self.playerData.attack.coefficient;
        var tempDefenseValue = GameManager.self.playerData.defense.coefficient;

        switch (itemStatType)
        {
            case 2: //this case is for the attack stat.
                durability = 10;
                modifier = 10 / 100; //10%
                GameManager.self.playerData.attack.coefficient *= modifier;

                if (m_Age >= durability) //Destroy/Remove attack buff.
                {
                    //TODO. Revert attack buff.
                    GameManager.self.playerData.attack.coefficient = tempAttackValue;
                }
                break;

            case 1: //this case is for the defense stat.
                durability = 10;
                modifier = 10 / 100; //10%
                GameManager.self.playerData.defense.coefficient *= modifier;

                if (m_Age >= durability) //Destroy/Remove defense buff.
                {
                    //TODO. Revert defense buff.
                    GameManager.self.playerData.defense.coefficient = tempDefenseValue;
                }
                break;

            default:
                break;
        }
    }
}