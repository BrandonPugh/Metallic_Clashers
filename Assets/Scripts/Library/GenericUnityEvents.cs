namespace Library
{
    using System;

    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class UnityBoolEvent : UnityEvent<bool> { }

    [Serializable]
    public class UnityIntEvent : UnityEvent<int> { }
    [Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }

    [Serializable]
    public class UnityVector2Event : UnityEvent<Vector2> { }
    [Serializable]
    public class UnityVector3Event : UnityEvent<Vector3> { }
}
