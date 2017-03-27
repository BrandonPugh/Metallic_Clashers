namespace Combat
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;

    [RequireComponent(typeof(Image))]
    public class CombatLifeBar : MonoBehaviour
    {
        [SerializeField]
        private Image m_Image;
        [SerializeField]
        private Text m_HealthText;

        // Use this for initialization
        private void Awake()
        {
            m_Image = GetComponent<Image>();
            OnCombatModeChange();

            m_HealthText = GetComponentInChildren<Text>();
            m_HealthText.text =
                GameManager.self.playerData.health.totalValue + "/" +
                GameManager.self.playerData.health.value;

            CombatManager.self.onCombatModeChange.AddListener(OnCombatModeChange);
            GameManager.self.playerData.health.onTotalValueChanged.AddListener(OnHealthModifierChanged);
        }

        private void OnCombatModeChange()
        {
            m_Image.color = CombatManager.self.combatUiInformation.currentModeUiInformation.modeColor;
        }

        //private void OnCombatUpdate()
        //{
        //    var playerHeath = GameManager.self.playerData.health;
        //    m_HealthText.text = Math.Ceiling(playerHeath.totalValue) + "/" + playerHeath.value;
        //}

        private void OnHealthModifierChanged()
        {
            var playerHeath = GameManager.self.playerData.health;
            m_HealthText.text = Math.Ceiling(playerHeath.totalValue) + "/" + playerHeath.value;
        }
    }
}
