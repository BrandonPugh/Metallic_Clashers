namespace CustomParticleSystem
{
    using System;
    using System.Linq;

    using CustomParticleSystem;

    using UnityEngine;

    using Random = UnityEngine.Random;

    [RequireComponent(typeof(RectTransform))]
    public class ParticleSystem2D : MonoBehaviour
    {
        [SerializeField]
        private Particle2D m_Particle2DPrefab;

        [Space, SerializeField, Range(0.1f, 3f)]
        private float m_ParticleDuration;

        private GameObject m_ParticleAnchor;

        private void Awake()
        {
            Random.InitState(DateTime.Now.Millisecond);
        }

        private void Start()
        {
            m_ParticleAnchor = new GameObject("Particle Anchor");
            m_ParticleAnchor.transform.SetParent(transform.parent, false);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!Input.GetMouseButton(0))
                return;

            var newParticle2D = Instantiate(m_Particle2DPrefab);
            newParticle2D.transform.SetParent(m_ParticleAnchor.transform, false);
            newParticle2D.transform.position = transform.position;

            var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            newParticle2D.velocity =
                new Vector2(Mathf.Cos(angle) * 7f, Mathf.Sin(angle) * 7f);

            newParticle2D.friction = new Vector2(1f, 1f);
            newParticle2D.duration = m_ParticleDuration;
        }
    }
}
