namespace CustomInput
{
    using CustomParticleSystem;

    using UnityEngine;

    public class InputEffects : MonoBehaviour
    {
        [SerializeField]
        private Canvas m_OverlayCanvs;
        [SerializeField]
        private ParticleSystem2D m_ParticleSystem2DPrefab;

        private ParticleSystem2D m_ParticleSystem2D;

        private void Awake()
        {
            m_ParticleSystem2D = Instantiate(m_ParticleSystem2DPrefab);
            m_ParticleSystem2D.transform.SetParent(m_OverlayCanvs.transform, false);
        }

        private void LateUpdate()
        {
            //var touches = UnityEngine.Input.touches;
            //if (!touches.Any())
            //    return;

            //var newPosition = Camera.main.ScreenToWorldPoint(touches.First().position);

            m_ParticleSystem2D.transform.position = Input.mousePosition;
        }
    }
}
