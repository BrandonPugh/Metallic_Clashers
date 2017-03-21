namespace CustomInput
{
    using System;

    using Information;

    using Library;

    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class TouchEvent : UnityEvent<TouchInformation> { }

    [Serializable]
    public class DragEvent : UnityEvent<DragInformation> { }

    public class InputManager : MonoSingleton<InputManager>
    {
        [SerializeField]
        private float m_DragDeadzone;

        [SerializeField, Space]
        private TouchEvent m_OnPress = new TouchEvent();
        [SerializeField]
        private TouchEvent m_OnRelease = new TouchEvent();
        [SerializeField]
        private TouchEvent m_OnHold = new TouchEvent();

        [SerializeField]
        private DragEvent m_OnBeginDrag = new DragEvent();
        [SerializeField]
        private DragEvent m_OnDrag = new DragEvent();
        [SerializeField]
        private DragEvent m_OnEndDrag = new DragEvent();

        private Vector2 m_PreviousPosition;

        private float m_CurrentHoldDuration;
        private float m_CurrentTotalDragDistance;
        private Vector2 m_PressPosition;

        private bool m_Dragging;

        public float dragDeadzone { get { return m_DragDeadzone; } }

        public TouchEvent onPress { get { return m_OnPress; } }
        public TouchEvent onRelease { get { return m_OnRelease; } }
        public TouchEvent onHold { get { return m_OnHold; } }

        public DragEvent onBeginDrag { get { return m_OnBeginDrag; } }
        public DragEvent onDrag { get { return m_OnDrag; } }
        public DragEvent onEndDrag { get { return m_OnEndDrag; } }

        protected override void OnAwake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Press");

                m_PressPosition = Input.mousePosition;

                m_OnPress.Invoke(
                    new TouchInformation { duration = 0f, position = m_PressPosition });

                m_CurrentHoldDuration = 0f;
            }
            else if (Input.GetMouseButton(0))
            {
                if (!m_Dragging &&
                    Vector2.Distance(m_PressPosition, Input.mousePosition) >= m_DragDeadzone)
                {
                    //Debug.Log("Begin Drag");

                    m_CurrentTotalDragDistance = Vector2.Distance(m_PressPosition, Input.mousePosition);

                    m_OnBeginDrag.Invoke(
                        new DragInformation
                        {
                            origin = m_PressPosition,
                            end = Input.mousePosition,

                            duration = 0f,

                            delta = (Vector2)Input.mousePosition - m_PressPosition,

                            totalDelta = (Vector2)Input.mousePosition - m_PressPosition,
                            totalDistance = Vector2.Distance(m_PressPosition, Input.mousePosition),
                        });

                    m_Dragging = true;
                }
                else if (m_Dragging)
                {
                    //Debug.Log("Drag");

                    m_CurrentTotalDragDistance += Vector2.Distance(m_PreviousPosition, Input.mousePosition);

                    m_OnDrag.Invoke(
                        new DragInformation
                        {
                            origin = m_PressPosition,
                            end = Input.mousePosition,

                            duration = m_CurrentHoldDuration,

                            delta = (Vector2)Input.mousePosition - m_PreviousPosition,

                            totalDelta = (Vector2)Input.mousePosition - m_PressPosition,
                            totalDistance = m_CurrentTotalDragDistance
                        });
                }
                else
                {
                    //Debug.Log("Hold");

                    m_OnHold.Invoke(
                        new TouchInformation
                        {
                            duration = m_CurrentHoldDuration,
                            position = Input.mousePosition
                        });
                }

                m_CurrentHoldDuration += Time.deltaTime;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!m_Dragging)
                {
                    //Debug.Log("Release");

                    m_OnRelease.Invoke(
                        new TouchInformation
                        {
                            duration = m_CurrentHoldDuration,
                            position = m_PressPosition
                        });
                }
                else
                {
                    //Debug.Log("End Drag");

                    m_OnEndDrag.Invoke(
                        new DragInformation
                        {
                            origin = m_PressPosition,
                            end = Input.mousePosition,

                            duration = m_CurrentHoldDuration,

                            delta = (Vector2)Input.mousePosition - m_PreviousPosition,

                            totalDelta = (Vector2)Input.mousePosition - m_PressPosition,
                            totalDistance = m_CurrentTotalDragDistance
                        });
                }

                m_CurrentTotalDragDistance = 0f;
                m_CurrentHoldDuration = 0f;
                m_Dragging = false;
            }

            m_PreviousPosition = Input.mousePosition;
        }
    }
}