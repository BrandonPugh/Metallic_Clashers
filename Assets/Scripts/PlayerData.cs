using System;
using System.Collections.Generic;

using Combat.Board;

using UnityEngine;

using Tree = StageSelection.Tree;

[Serializable]
public class PlayerData
{
    public Attribute health;

    public Attribute attack;

    public Attribute defense;

    public float decayRate;

    public List<GemType> resistances;
    public List<GemType> weaknesses;

    public List<GemSkill> gemSkills = new List<GemSkill>();

    public List<Tree> worldData = new List<Tree>();
    public StaminaInformation staminaInformation;

    public LevelSystem playerLevelSystem;

    //public List<BaseItems> itemInventory = new List<BaseItems>();

    public void TakeDamage(float damage, GemType gemType)
    {
        var finalDamage = damage - defense.totalValue;
        defense.modifier -= damage;
        if (defense.modifier < -defense.value)
        {
            defense.modifier = -defense.value;
        }

        if (finalDamage <= 0)
            return;

        if (resistances.Contains(gemType))
        {
            finalDamage *= .75f;
        }

        else if (weaknesses.Contains(gemType))
        {
            finalDamage *= 1.25f;
        }

        health.modifier -= finalDamage;
    }

    public void DecaySheild()
    {
        if (defense.modifier > 0)
        {
            defense.modifier -= decayRate * Time.deltaTime;
            if (defense.modifier < 0)
            {
                defense.modifier = 0;
            }
        }

        if (defense.modifier > defense.value*20 - defense.value)
        {
            defense.modifier = defense.value * 20 - defense.value;
        }
    }
}
