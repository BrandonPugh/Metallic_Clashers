namespace Combat.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class QuitCombatEarly : MonoBehaviour
    {
        private Button m_Button;

        private void Awake()
        {
            m_Button = GetComponent<Button>();
            m_Button.onClick.AddListener(OnClick);
        }

        private static void OnClick()
        {
            GameManager.self.LoadScene(1);
        }
    }
}
