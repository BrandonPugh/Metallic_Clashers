/* Script Info - Script Name: BaseItem.cs, Created by: Brock Barlow, This script is used to handle items. */

using UnityEngine;
using System;
using System.Collections;

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
    public int healStrengthType = 3;
 
    public override void UseItem()
    {
        switch (healStrengthType)
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
    public float amountOfTurns = 10; //total turns the player can make before losing buff
    public int itemStatType = 2;

    public override void UseItem()
    {
        switch (itemStatType)
        {
            case 2: //this case is for the attack stat buff.
                durability = amountOfTurns;
                modifier = GameManager.self.playerData.attack.coefficient * buffValue;
                GameManager.self.playerData.attack.modifier += modifier;
                break;
            case 1: //this case is for the defense stat buff.
                durability = amountOfTurns;
                modifier = GameManager.self.playerData.defense.coefficient * buffValue;
                GameManager.self.playerData.defense.coefficient += modifier;
                break;
            default:
                break;
        }
    }

    public override void UpdateSelf()
    {
        base.UpdateSelf();
        if (m_Age > durability) //Get rid of modified value and destroy item.
        {
            GameManager.self.playerData.attack.modifier -= modifier;
        }
    }
}

[Serializable]
public class TimeBased : BaseItem //these items are based on a timer when activated by the player during play.
{
    public virtual void UpdateSelf(float value) { m_Age += value; } //modify age with float value.
}

[Serializable]
public class TimeBuff : TimeBased
{
    //time item idea: attack increase item. will raise the player's attack value.
    //effect will have a lasting/timed effect (based off time).
    //
    //time item idea: defense increase item. will raise the player's defense value.
    //effect will have a lasting/timed effect (based off time).

    public float buffValue = .15f; //15%
    public float amountOfTime = 10; //how much time the player has before losing buff
    public int itemStatType = 2;
    public float counter = 10; //used as timer for buff item

    public override void UseItem()
    {
        switch (itemStatType)
        {
            case 2: //this case is for the attack stat buff.
                durability = amountOfTime;
                modifier = GameManager.self.playerData.attack.coefficient * buffValue;
                GameManager.self.playerData.attack.modifier += modifier;
                counter -= Time.deltaTime;
                break;
            case 1: //this case is for the defense stat buff.
                durability = amountOfTime;
                modifier = GameManager.self.playerData.defense.coefficient * buffValue;
                GameManager.self.playerData.defense.coefficient += modifier;
                counter -= Time.deltaTime;
                break;
            default:
                break;
        }
    }

    public override void UpdateSelf(float counter)
    {
        base.UpdateSelf(counter);
        if (m_Age > durability) //Get rid of modified value and destroy item.
        {
            GameManager.self.playerData.attack.modifier -= modifier;
        }
        if (counter < durability) //reset timer/counter
        {
            counter = 10;
        }
    }
}