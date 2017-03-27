namespace Combat
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;

    public class CombatStatusBar : MonoBehaviour
    {
        [SerializeField]
        private Image m_HealthImage;
        [SerializeField]
        private Image m_HealthBackgroundImage;
        [SerializeField]
        private Text m_HealthText;

        [SerializeField]
        private Image m_ShieldImage;
        [SerializeField]
        private Text m_ShieldText;

        // Use this for initialization
        private void Awake()
        {
            OnCombatModeChange();
        }

        private void Start()
        {
            OnHealthModifierChanged();
            OnDefenseModifierChanged();

            CombatManager.self.onCombatModeChange.AddListener(OnCombatModeChange);

            GameManager.self.playerData.health.onTotalValueChanged.AddListener(OnHealthModifierChanged);
            GameManager.self.playerData.defense.onTotalValueChanged.AddListener(OnDefenseModifierChanged);
        }

        private void OnCombatModeChange()
        {
            if (m_HealthImage == null)
                return;

            m_HealthImage.color =
                CombatManager.self.combatUiInformation.currentModeUiInformation.modeColor;

            if (m_HealthBackgroundImage == null)
                return;

            float h, s, v;
            Color.RGBToHSV(m_HealthImage.color, out h, out s, out v);

            m_HealthBackgroundImage.color = Color.HSVToRGB(h, s - 0.2f, v);
        }

        private void OnHealthModifierChanged()
        {
            if (m_HealthText != null)
            {
                var playerHeath = GameManager.self.playerData.health;
                m_HealthText.text =
                    Math.Ceiling(playerHeath.totalValue) + "/" + Mathf.Ceil(playerHeath.value);
            }

            OnStatusModifierChanged();
        }

        private void OnDefenseModifierChanged()
        {
            if (m_ShieldText != null)
            {
                var playerDefense = GameManager.self.playerData.defense;
                m_ShieldText.text = Math.Ceiling(playerDefense.totalValue).ToString();
            }

            OnStatusModifierChanged();
        }

        private void OnStatusModifierChanged()
        {
            if (m_HealthImage == null || m_ShieldImage == null)
                return;

            var totalValue =
                GameManager.self.playerData.health.totalValue
                + GameManager.self.playerData.defense.totalValue;

            var maxValue = GameManager.self.playerData.health.value;

            var imageBounds = m_HealthImage.rectTransform.rect.max;

            if (totalValue < maxValue)
            {
                m_HealthImage.fillAmount =
                    GameManager.self.playerData.health.totalValue / GameManager.self.playerData.health.value;

                m_ShieldImage.rectTransform.anchorMin =
                    new Vector2(
                        m_HealthImage.fillAmount,
                        m_ShieldImage.rectTransform.anchorMin.y);

                m_ShieldImage.fillAmount =
                    GameManager.self.playerData.defense.totalValue
                    / (GameManager.self.playerData.health.value
                        - GameManager.self.playerData.health.totalValue);
            }
            else
            {
                m_HealthImage.fillAmount =
                    GameManager.self.playerData.health.totalValue / totalValue;

                m_ShieldImage.rectTransform.anchorMin =
                    new Vector2(
                        m_HealthImage.fillAmount,
                        m_ShieldImage.rectTransform.anchorMin.y);

                m_ShieldImage.fillAmount = 1f;
            }

            m_HealthText.rectTransform.anchorMax =
                    new Vector2(
                        m_HealthImage.fillAmount,
                        m_HealthText.rectTransform.anchorMax.y);
        }
    }
}
