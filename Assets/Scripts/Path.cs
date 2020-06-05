using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    public List<Vector2> points;
    public Path(Vector2 center)
    {
        points = new List<Vector2>
        {
            center,
            center + Vector2.up * .5f,
            center + (Vector2.right + Vector2.down) * .5f,
            center + Vector2.right * 50f
        };
    }
    
    public int NumSegment
    {
        get { return (points.Count - 4) / 3 + 1; }
    }

    public void AddSegment(Vector2 anchor)
    {
        points.Add(points[points.Count-1] * 2 - points[points.Count-2]);
        points.Add((points[points.Count-1] + anchor) * .5f);
        points.Add(anchor);
    }
    
    public void DeleteSegment()
    {
        if (NumSegment > 2)
        {
            points.RemoveRange(0, 3);
        }
    }

    public Vector2[] GetPointsInSegment( int i)
    {
        return new Vector2[]{points[i*3], points[i*3+1], points[i*3+2], points[i*3+3]};
    }

    public Vector2 GetSegmentFromBack(int i)
    {
        return points[points.Count - i];
    }

    public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
    {
        List<Vector2> evenlySpacedPoints = new List<Vector2>();
        evenlySpacedPoints.Add(points[0]);
        Vector2 previousPoint = points[0];
        float dstSinceLastEvenPoint = 0;

        for (int segmentIndex = 0; segmentIndex < NumSegment; segmentIndex++)
        {
            Vector2[] p = GetPointsInSegment(segmentIndex);
            float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
            float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLength / 2f;
            int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
            float t = 0;
            while (t <= 1)
            {
                t += 1f/divisions;
                Vector2 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                dstSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

                while (dstSinceLastEvenPoint >= spacing)
                {
                    float overshootDst = dstSinceLastEvenPoint - spacing;
                    Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                    evenlySpacedPoints.Add(newEvenlySpacedPoint);
                    dstSinceLastEvenPoint = overshootDst;
                    previousPoint = newEvenlySpacedPoint;
                }

                previousPoint = pointOnCurve;
            }
        }

        return evenlySpacedPoints.ToArray();
    }
}
