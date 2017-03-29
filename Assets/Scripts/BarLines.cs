using UnityEngine;
using UnityEngine.UI;

public class BarLines : Graphic
{
    private enum BarType
    {
        Health,
        Shield,
    }

    [Space, SerializeField]
    private Image m_ParentImage;
    [SerializeField]
    private float m_LineWidth = 1f;
    [SerializeField]
    private int m_SegmentValue = 50;

    [SerializeField]
    private BarType m_BarType;

    protected override void Awake()
    {
        base.Awake();

        if (!Application.isPlaying)
            return;

        GameManager.self.playerData.health.onTotalValueChanged.AddListener(OnTotalValueChanged);
        GameManager.self.playerData.defense.onTotalValueChanged.AddListener(OnTotalValueChanged);

        OnTotalValueChanged();
    }

    private void OnTotalValueChanged()
    {
        rectTransform.anchorMax =
            new Vector2(
                m_ParentImage.fillAmount,
                rectTransform.anchorMax.y);

        SetVerticesDirty();
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (!Application.isPlaying || m_ParentImage == null || m_ParentImage.fillAmount <= 0f)
            return;

        var vertex = UIVertex.simpleVert;
        vertex.color = color;

        var startingPoint = rectTransform.anchorMin;
        startingPoint -= rectTransform.pivot;
        startingPoint =
            new Vector2(
                rectTransform.rect.width * startingPoint.x,
                rectTransform.rect.height * startingPoint.y);

        startingPoint -= new Vector2(m_LineWidth / 2f, 0f);

        var totalValue =
            m_BarType == BarType.Health ?
            GameManager.self.playerData.health.totalValue :
            GameManager.self.playerData.defense.totalValue;

        var parentWidth = m_ParentImage.rectTransform.rect.width * m_ParentImage.fillAmount;
        var lineCount = totalValue / m_SegmentValue;

        var spacing = m_SegmentValue / totalValue * parentWidth;

        for (var i = 0; i <= lineCount; ++i)
        {
            var currentPosition = startingPoint + i * new Vector2(spacing, 0f);

            vertex.position = currentPosition;
            vh.AddVert(vertex);

            vertex.position = currentPosition + Vector2.up * rectTransform.rect.height;
            vh.AddVert(vertex);

            vertex.position = currentPosition + Vector2.right * m_LineWidth;
            vh.AddVert(vertex);

            vertex.position =
                currentPosition + Vector2.right * m_LineWidth + Vector2.up * rectTransform.rect.height;
            vh.AddVert(vertex);

            var vertexOffset = i * 4;
            vh.AddTriangle(0 + vertexOffset, 1 + vertexOffset, 2 + vertexOffset);
            vh.AddTriangle(1 + vertexOffset, 3 + vertexOffset, 2 + vertexOffset);
        }
    }
}
