namespace Combat.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class CombatMenuButton : MonoBehaviour
    {
        [SerializeField]
        private Button m_Button;

        [SerializeField]
        private Image m_BackgroundImage;

        [SerializeField]
        private GameObject m_IconParent;

        // Use this for initialization
        private void Awake()
        {
            if (m_Button == null)
                m_Button = GetComponent<Button>();

            if (m_Button != null)
                m_Button.onClick.AddListener(OnClick);

            OnCombatPauseChange(CombatManager.self.isPaused);

            CombatManager.self.onCombatPauseChange.AddListener(OnCombatPauseChange);
        }

        private static void OnClick()
        {
            CombatManager.self.isPaused = !CombatManager.self.isPaused;
        }

        private void OnCombatPauseChange(bool isPaused)
        {
            if (m_BackgroundImage != null)
                m_BackgroundImage.enabled = isPaused;

            if (m_IconParent != null)
                m_IconParent.SetActive(isPaused);
        }
    }
}
