/* Script Info - Script Name: LevelSystem.cs, Created By: Brock Barlow, This script is used to level up the player. */

using System;

[Serializable]
public class LevelSystem
{
    [Serializable]
    public struct LevelInfo
    {
        public uint level; //player's level
        public uint currentExperience; //player's current exp amount
        public uint experienceRequired; //the exp amount the player needs to level up
        public uint experienceNeeded //experienceRequired - currentExperience
        {
            get { return experienceRequired - currentExperience; }
        } 
    }

    public LevelInfo playerLevelInfo;

    private LevelInfo CalculateLevel(uint exp)
    {
        //base_xp * (level_to_get ^ factor);

        //base_xp = constant, how much xp needed to levelup.
        //level_to_get = level aiming for.
        //factor = constant, how much of an increase of xp needed for levelup.

        var tempExperience = exp;

        const uint c_baseExperience = 200; //base_xp //testing value from research results
        uint tempLevel = 1; //level_to_get //player needs to start at level one
        const uint c_factor = 2; //factor //testing value from research results

        var experienceRequiredFormula = c_baseExperience * ((tempLevel + 1) ^ c_factor);

        for (tempLevel = 1; tempExperience > experienceRequiredFormula; tempLevel++)
        {
            tempExperience -= experienceRequiredFormula;
            experienceRequiredFormula = c_baseExperience * ((tempLevel + 2) ^ c_factor); //recalculate formula value
        }

        var levelInfo = new LevelInfo
        {
            level = tempLevel, //player's level
            currentExperience = tempExperience, //player's current exp amout
            experienceRequired = experienceRequiredFormula, //the exp amount the player needs to level up
        };

        return levelInfo;
    }
                             //fight exp
    public void IsLeveledUp(uint modifier)
    {
        var tempCurrentExperience = CalculateLevel(playerLevelInfo.currentExperience);
        var finalTotal = CalculateLevel(playerLevelInfo.currentExperience + modifier);

        if (finalTotal.level != tempCurrentExperience.level)
        {
            uint differenceInLevel;

            for (differenceInLevel = finalTotal.level - tempCurrentExperience.level; differenceInLevel > 0; differenceInLevel--)
            {
                const int c_percentageValue = 10 / 100; //10%

                //health stat change
                GameManager.self.playerData.health.value *= c_percentageValue;

                //attack stat change
                GameManager.self.playerData.attack.value *= c_percentageValue;

                //defense stat change
                GameManager.self.playerData.defense.value *= c_percentageValue;

                //stamina stat change
                StaminaManager.self.maxValue *= c_percentageValue;
            }
        }

        playerLevelInfo = finalTotal;
    }
}