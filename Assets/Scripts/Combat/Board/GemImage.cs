namespace Combat.Board
{
    using System;

    using Information;

    using UnityEngine;
    using UnityEngine.UI;

    public class GemImage : Image, IComponent
    {
        public enum ImageType
        {
            Background,
            Midground,
            Foreground
        }

        [SerializeField]
        private ImageType m_ImageType;
        [SerializeField]
        private Gem m_Gem;

        public ImageType imageType { get { return m_ImageType; } }
        public Gem gem { get { return m_Gem; } set { m_Gem = value; } }

        protected override void Start()
        {
            base.Start();

            gem.onTypeChange.AddListener(OnTypeChange);

            CombatManager.self.onCombatModeChange.AddListener(UpdateSprite);
            CombatManager.self.combatUiInformation.onUseAlternativeColorsChange.AddListener(
                OnUseAlternativeColorsChange);

            UpdateSprite();
            SetColor(CombatManager.self.combatUiInformation.gemColors[(int)gem.gemType]);
        }

        private void SetColor(Color newColor)
        {
            if (m_ImageType == ImageType.Background)
                return;

            color = newColor;
        }

        private void UpdateSprite()
        {
            var currentUiInformation = CombatManager.self.combatUiInformation.currentModeUiInformation;
            switch (m_ImageType)
            {
                case ImageType.Background:
                    sprite = currentUiInformation.backgroundImage;
                    break;
                case ImageType.Midground:
                    sprite = currentUiInformation.midgroundImage;
                    break;
                case ImageType.Foreground:
                    sprite = currentUiInformation.foregroundImage;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnTypeChange(TypeChangeInformation typeChangeInformation)
        {
            SetColor(CombatManager.self.combatUiInformation.gemColors[(int)typeChangeInformation.newType]);
        }

        private void OnUseAlternativeColorsChange(bool newValue)
        {
            SetColor(CombatManager.self.combatUiInformation.gemColors[(int)gem.gemType]);
        }
    }
}
