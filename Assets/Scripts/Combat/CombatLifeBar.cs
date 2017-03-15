namespace Combat
{
    using UnityEngine;
    using UnityEngine.UI;

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
            CombatManager.self.onCombatUpdate.AddListener(OnCombatUpdate);
        }

        private void OnCombatModeChange()
        {
            m_Image.color = CombatManager.self.combatUiInformation.currentModeUiInformation.modeColor;
        }

        private void OnCombatUpdate()
        {
            var playerHeath = GameManager.self.playerData.health;
            m_HealthText.text = playerHeath.totalValue + "/" + playerHeath.value;
        }
    }
}
