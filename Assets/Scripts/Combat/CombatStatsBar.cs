namespace Combat
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;

    [RequireComponent(typeof(Image))]
    public class CombatStatsBar : MonoBehaviour
    {
        private Image m_Image;
        [SerializeField]
        private Text m_DefenseText;
        [SerializeField]
        private Text m_HealthText;

        // Use this for initialization
        private void Awake()
        {
            m_Image = GetComponent<Image>();
            OnCombatModeChange();

            CombatManager.self.onCombatModeChange.AddListener(OnCombatModeChange);
            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
        }

        private void OnCombatModeChange()
        {
            var color = CombatManager.self.combatUiInformation.currentModeUiInformation.modeColor;
            m_Image.color = color;
        }

        private void OnCombatUpdate()
        {
            var playerStats = GameManager.self.playerData;

            m_HealthText.text = "HP: " + Math.Ceiling(playerStats.health.totalValue)
                + "/" + playerStats.health.value;

            m_DefenseText.text = "S:" + Math.Ceiling(playerStats.defense.totalValue)
                + "/" + (playerStats.defense.value * 20);
        }
    }
}
