namespace CustomInput.Information
{
    using System;

    using UnityEngine;

    [Serializable]
    public class DragInformation
    {
        public Vector2 origin;
        public Vector2 end;

        public float duration;

        /// <summary>
        /// The delta in mouse position this frame
        /// </summary>
        public Vector2 delta;

        /// <summary>
        /// The total delta in mouse position from the start of the drag
        /// </summary>
        public Vector2 totalDelta;

        public float totalDistance;
    }
}