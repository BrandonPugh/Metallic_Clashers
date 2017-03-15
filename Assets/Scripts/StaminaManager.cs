using System;
using Library;

using UnityEngine;

[Serializable]
public class StaminaInformation
{
    public uint value;
    public string timeLastPlayed;
    public uint maxValue;
}

public class StaminaManager : MonoSingleton<StaminaManager>
{     
    public uint value;
    public uint maxValue;
    public float staminaRate;

    private float m_Timer;

	public void Start()
	{
	    var playerStaminaInfo = GameManager.self.playerData.staminaInformation; // Get the Stamina Info

	    value = playerStaminaInfo.value;    // Set value to StaminaInfo value
	    maxValue = playerStaminaInfo.maxValue;

        var timeLastPlayed = DateTime.Parse(playerStaminaInfo.timeLastPlayed);   // Get Last Time the app was open 
        var ts = DateTime.Now - timeLastPlayed; // Calculate the time span
        var secondsPassed = 0;

        // Convert it all to Seconds
        secondsPassed += ts.Days * 86164;
        secondsPassed += ts.Hours * 3600;
        secondsPassed += ts.Minutes * 60;
        secondsPassed += ts.Seconds;

        // Add the time that was passed
        value += (uint)(secondsPassed / staminaRate);

        // Limit the value
	    if (value > maxValue)
	    {
	        value = maxValue;
	    }
    }	

	private void Update()
	{
        // If value is at maxValue, don't allow to add time
        if (value >= maxValue)
	    {
            m_Timer = staminaRate;
            return;
        }

        m_Timer -= Time.deltaTime;
	    if (!(m_Timer <= 0))  // As long as timer isn't less than or equal to 0, stop process here
            return;

	    value++;
        m_Timer = staminaRate;
	}
}
