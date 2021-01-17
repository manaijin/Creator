using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class CircleImage : Image
{
    [Tooltip("圆形或扇形填充比例")] [Range(0, 1)] public float FillPercent = 1f;
    [Tooltip("是否填充圆形")] public bool Fill = true;
    [Tooltip("圆环宽度")] public float Thickness = 5;
    [Tooltip("顶点数量")] [Range(3, 100)] public int Segements = 20;

    private Rect _rect;
    private float _degreeDelta;

    protected override void Awake()
    {
        base.Awake();        
        OnRectTransformDimensionsChange();
    }
    
    protected override void OnRectTransformDimensionsChange()
    {
        _rect = rectTransform.rect;
        Thickness = Mathf.Clamp(Thickness, 0, _rect.width / 2);
        base.OnRectTransformDimensionsChange();
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        CalcData();
        _degreeDelta = 2 * Mathf.PI / Segements;
        int curSegements = Mathf.CeilToInt(Segements * FillPercent);
        float outerRadius = 0.5f * Mathf.Min(_rect.width, _rect.height);
        float curDegree = 0;
        if (Fill)
        {
            int verticeCount = curSegements + 1;
            // 圆心
            AddVertex(vh, 0, 0);
            for (int i = 1; i < verticeCount; i++)
            {
                float cosA = Mathf.Cos(curDegree);
                float sinA = Mathf.Sin(curDegree);
                curDegree += _degreeDelta;
                AddVertex(vh, cosA * outerRadius, sinA * outerRadius);
            }

            int triangleCount = curSegements * 3;
            for (int i = 0, vIdx = 1; i < triangleCount - 3; i += 3, vIdx++)
            {
                vh.AddTriangle(vIdx, 0, vIdx + 1);
            }

            if (FillPercent == 1) //首尾顶点相连
            {
                vh.AddTriangle(verticeCount - 1, 0, 1);
            }
        }
        else //圆环
        {
            float innerRadius = outerRadius - Thickness;

            int verticeCount = curSegements * 2;
            for (int i = 0; i < verticeCount; i += 2)
            {
                float cosA = Mathf.Cos(curDegree);
                float sinA = Mathf.Sin(curDegree);
                curDegree += _degreeDelta;

                AddVertex(vh, cosA * innerRadius, sinA * innerRadius);// 外圈
                AddVertex(vh, cosA * outerRadius, sinA * outerRadius);// 内圈
            }

            int triangleCount = curSegements * 3 * 2;
            for (int i = 0, vIdx = 0; i < triangleCount - 6; i += 6, vIdx += 2)
            {
                vh.AddTriangle(vIdx + 1, vIdx, vIdx + 3);
                vh.AddTriangle(vIdx, vIdx + 2, vIdx + 3);
            }

            if (FillPercent == 1) //首尾顶点相连
            {
                vh.AddTriangle(verticeCount - 1, verticeCount - 2, 1);
                vh.AddTriangle(verticeCount - 2, 0, 1);
            }
        }
    }

    /// <summary>
    /// 缓存一些Mesh计算的数据，这些数据每次取都会有一定的耗时
    /// </summary>
    private void CalcData()
    {
        _tmpVert.color = color;

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        _uvMinX = uv.x;
        _uvMinY = uv.y;
        _uvScaleX = (uv.z - uv.x) / _rect.width;
        _uvScaleY = (uv.w - uv.y) / _rect.height;

        _rectXMin = _rect.xMin;
        _rectXMax = _rect.xMax;
        _rectYMin = _rect.yMin;
        _rectYMax = _rect.yMax;
    }

    // UV分布到Rect上的单位距离
    private float _uvScaleX;
    private float _uvScaleY;
    // UV范围
    private float _uvMinX;
    private float _uvMinY;
    // RectTransform范围
    private float _rectXMin;
    private float _rectXMax;
    private float _rectYMin;
    private float _rectYMax;

    private UIVertex _tmpVert = UIVertex.simpleVert;

    private void AddVertex(VertexHelper vh, float vertX, float vertY)
    {
        // 做Clamp是为了防止圆形Mesh超出RectTransform的范围
        float posX = Mathf.Clamp(vertX, _rectXMin, _rectXMax);
        float posY = Mathf.Clamp(vertY, _rectYMin, _rectYMax);

        _tmpVert.position.x = posX;
        _tmpVert.position.y = posY;
        _tmpVert.uv0.x = _uvMinX + _uvScaleX * (posX - _rectXMin);
        _tmpVert.uv0.y = _uvMinY + _uvScaleY * (posY - _rectYMin);

        vh.AddVert(_tmpVert);
    }
}
