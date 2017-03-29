namespace Combat.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class CombatAlternativeColorButton : MonoBehaviour
    {
        [SerializeField]
        private Button m_Button;

        // Use this for initialization
        private void Awake()
        {
            if (m_Button == null)
                m_Button = GetComponent<Button>();

            m_Button.onClick.AddListener(OnClick);
        }

        private static void OnClick()
        {
            CombatManager.self.combatUiInformation.useAlternativeColors =
                !CombatManager.self.combatUiInformation.useAlternativeColors;
        }
    }
}
