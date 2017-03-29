namespace Combat.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class ToggleEnableObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_ToggleObject;

        private Button m_Button;

        private void Awake()
        {
            m_Button = GetComponent<Button>();
            m_Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (m_ToggleObject != null)
                m_ToggleObject.SetActive(!m_ToggleObject.activeInHierarchy);
        }
    }
}
