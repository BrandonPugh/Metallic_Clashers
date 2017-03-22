namespace Combat
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;

    [RequireComponent(typeof(Image))]
    public class CombatDefenseBar : MonoBehaviour
    {
        [SerializeField]
        private Image m_Image;
        [SerializeField]
        private Text m_DefenseText;

        // Use this for initialization
        private void Awake()
        {
            m_Image = GetComponent<Image>();
            OnCombatModeChange();

            m_DefenseText = GetComponentInChildren<Text>();
            m_DefenseText.text =
                GameManager.self.playerData.health.totalValue + "/" +
                GameManager.self.playerData.health.value;

            CombatManager.self.onCombatModeChange.AddListener(OnCombatModeChange);
            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
        }

        private void OnCombatModeChange()
        {
            //var color = CombatManager.self.combatUiInformation.currentModeUiInformation.modeColor;
            //m_Image.color = (color.r > color.b) ? Color.blue : Color.red;
        }

        private void OnCombatUpdate()
        {
            var playerDefense = GameManager.self.playerData.defense;
            m_DefenseText.text =
                Math.Ceiling(playerDefense.totalValue) + "/" 
                + (playerDefense.value * 20 + playerDefense.value);
        }
    }
}
