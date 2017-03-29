using System;

using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Attribute
{
    [SerializeField]
    private float m_Modifier;
    [SerializeField]
    private float m_Coefficient = 1f;
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
