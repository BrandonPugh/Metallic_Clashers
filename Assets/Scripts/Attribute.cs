using System;

using UnityEngine.Events;

[Serializable]
public class Attribute
{
    private float m_Modifier;
    private float m_Coefficient;
    private UnityEvent m_OnTotalValueChanged = new UnityEvent();

    public float value;

    public float coefficient
    {
        get { return m_Coefficient; }
        set
        {
            m_Coefficient = value;
            m_OnTotalValueChanged.Invoke();
        }
    }

    public float modifier
    {
        get { return m_Modifier; }
        set
        {
            m_Modifier = value;
            m_OnTotalValueChanged.Invoke();
        }
    }
    public UnityEvent onTotalValueChanged { get { return m_OnTotalValueChanged; } }

    public float totalValue { get { return value * m_Coefficient + m_Modifier; } }
}
