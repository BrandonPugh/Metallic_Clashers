//////////////////////
//   MonoSingleton  //
//////////////////////

using UnityEngine;

namespace Library
{
    using System.Linq;

    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool s_IsQuitting;

        private static T s_Self;

        public static T self
        {
            get
            {
                if (s_IsQuitting)
                    return null;

                if (s_Self == null)
                    s_Self = FindObjectOfType<T>();

                return s_Self;
            }
        }

        protected MonoSingleton() { }

        private void Awake()
        {
            if (s_Self == null)
                s_Self = this as T;
            else if (s_Self != this)
            {
                Destroy(gameObject);
                return;
            }

            s_IsQuitting = false;

            OnAwake();
        }

        protected virtual void OnApplicationQuit() { s_IsQuitting = true; }

        protected virtual void OnAwake() { }
    }
}