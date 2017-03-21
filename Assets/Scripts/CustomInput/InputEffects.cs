namespace CustomInput
{
    using System.Linq;

    using CustomParticleSystem;

    using Library;

    using UnityEngine;

    public class InputEffects : MonoSingleton<InputEffects>
    {
        [SerializeField]
        private Canvas m_OverlayCanvas;
        [SerializeField]
        private ParticleSystem2D m_ParticleSystem2DPrefab;

        private ParticleSystem2D m_ParticleSystem2D;

        protected override void OnAwake()
        {
            DontDestroyOnLoad(this);
            GameManager.self.onSceneLoaded.AddListener(Init);

            Init();
        }

        private void LateUpdate()
        {
            //var touches = UnityEngine.Input.touches;
            //if (!touches.Any())
            //    return;

            //var newPosition = Camera.main.ScreenToWorldPoint(touches.First().position);
            if (m_ParticleSystem2D != null)
                m_ParticleSystem2D.transform.position = Input.mousePosition;
        }

        private void Init()
        {
            m_OverlayCanvas =
                    FindObjectsOfType<Canvas>().
                        FirstOrDefault(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay);

            if (m_OverlayCanvas == null)
                return;

            m_ParticleSystem2D = Instantiate(m_ParticleSystem2DPrefab);
            m_ParticleSystem2D.transform.SetParent(m_OverlayCanvas.transform, false);
        }
    }
}
