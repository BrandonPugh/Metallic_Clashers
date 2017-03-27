using System;
using System.Collections.Generic;

using Combat.Board;

using UnityEngine;

using Tree = StageSelection.Tree;

[Serializable]
public class PlayerData
{
    public PlayerData()
    {
        health = new Attribute();
        attack = new Attribute();
        defense = new Attribute();

        health.value = 200;
        attack.value = 10;
        defense.value = 10;

        health.coefficient = 1;
        defense.coefficient = 1;
        attack.coefficient = 1;

        resistances = new List<GemType>();
        weaknesses = new List<GemType>();
    }

    public PlayerData(float hel, float att, float def)
    {
        health = new Attribute();
        attack = new Attribute();
        defense = new Attribute();

        health.value = hel;
        attack.value = att;
        defense.value = def;

        health.coefficient = 1;
        defense.coefficient = 1;
        attack.coefficient = 1;

        resistances = new List<GemType>();
        weaknesses = new List<GemType>();
    }

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

    public void DecayShield()
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
