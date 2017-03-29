namespace Combat.UI
{
    using System;

    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class CombatModeButton : MonoBehaviour
    {
        [SerializeField]
        private Button m_Button;
        [SerializeField]
        private Image m_Image;
        [SerializeField]
        private Text m_ModeText;

        // Use this for initialization
        private void Awake()
        {
            m_Button = GetComponent<Button>();
            if (m_Image == null)
                m_Image = GetComponent<Image>();

            m_ModeText = GetComponentInChildren<Text>();

            OnCombatModeChange();

            CombatManager.self.onCombatModeChange.AddListener(OnCombatModeChange);

            m_Button.onClick.AddListener(OnClick);
        }

        private static void OnClick()
        {
            CombatManager.self.ToggleCombatMode();
        }

        private void OnCombatModeChange()
        {
            m_Image.color = CombatManager.self.combatUiInformation.currentModeUiInformation.modeColor;
            m_ModeText.color = CombatManager.self.combatUiInformation.currentModeUiInformation.modeColor;

            switch (CombatManager.self.combatMode)
            {
            case CombatManager.CombatMode.Attack:
                m_ModeText.text = "ATK";
                break;
            case CombatManager.CombatMode.Defense:
                m_ModeText.text = "DEF";
                break;

            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
