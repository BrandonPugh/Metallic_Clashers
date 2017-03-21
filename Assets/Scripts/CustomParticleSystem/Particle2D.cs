namespace CustomParticleSystem
{
    using System.Collections;

    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class Particle2D : MonoBehaviour
    {
        private RectTransform m_RectTransform;
        private Image m_Image;

        public Vector2 velocity;
        public Vector2 friction;

        public float duration;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_Image = GetComponent<Image>();
        }

        private void Start()
        {
            m_RectTransform.anchoredPosition += velocity;
            StartCoroutine(DestroyRoutine());
        }

        private void FixedUpdate()
        {
            m_RectTransform.anchoredPosition += velocity;

            Mathf.MoveTowards(velocity.x, 0f, friction.x);
            Mathf.MoveTowards(velocity.y, 0f, friction.y);
        }

        private IEnumerator DestroyRoutine()
        {
            var deltaTime = 0f;
            while (deltaTime < duration)
            {
                m_Image.color =
                    new Color(
                        m_Image.color.r,
                        m_Image.color.g,
                        m_Image.color.b,
                        1f - deltaTime / duration);

                deltaTime += Time.deltaTime;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
