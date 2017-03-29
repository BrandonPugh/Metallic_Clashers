
namespace Combat.UI
{
    using System;
    using System.Collections.Generic;

    using Library;

    using UnityEngine;

    [CreateAssetMenu]
    public class CombatUiInformation : ScriptableObject
    {
        [Serializable]
        public class ModeUiInformation
        {
            public Sprite backgroundImage;
            public Sprite midgroundImage;
            public Sprite foregroundImage;

            public Color modeColor;
        }

        [SerializeField]
        private GameObject m_GemGameObjectPrefab;

        [SerializeField]
        private ModeUiInformation m_AttackModeUiInformation;
        [SerializeField]
        private ModeUiInformation m_DefenseModeUiInformation;

        [Space]
        public List<Color> standardGemColors = new List<Color>();
        public List<Color> alternativeGemColors = new List<Color>();

        [Space, SerializeField]
        private bool m_UseAlternativeColors;

        [Space, SerializeField]
        private UnityBoolEvent m_OnUseAlternativeColorsChange = new UnityBoolEvent();

        public GameObject gemGameObjectPrefab { get { return m_GemGameObjectPrefab; } }

        public List<Color> gemColors
        {
            get { return m_UseAlternativeColors ? alternativeGemColors : standardGemColors; }
        }

        public ModeUiInformation currentModeUiInformation
        {
            get
            {
                if (CombatManager.self == null)
                    return null;

                switch (CombatManager.self.combatMode)
                {
                    case CombatManager.CombatMode.Attack:
                        return m_AttackModeUiInformation;
                    case CombatManager.CombatMode.Defense:
                        return m_DefenseModeUiInformation;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool useAlternativeColors
        {
            get { return m_UseAlternativeColors; }
            set { m_UseAlternativeColors = value; m_OnUseAlternativeColorsChange.Invoke(value); }
        }

        public UnityBoolEvent onUseAlternativeColorsChange
        {
            get { return m_OnUseAlternativeColorsChange; }
        }
    }
}
