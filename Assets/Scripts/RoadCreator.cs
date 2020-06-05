using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RoadCreator : MonoBehaviour
{
    [Range(.05f, 1.5f)]
    public float spacing;
    public float roadWidth = 25;
    public float tiling = 1;
    public float anchorDiameter = .1f;
    
    [Range(10f, 100f)]
    public float minR, maxR;
    [Range(-180f, 180f)]
    public float minAngle, maxAngle;

    private int startAmount = 5; 
    private int _milestone;

    public GameObject milestonePrefab;
    public Path path;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        
        CreatePath();
    }

    private void CreatePath()
    {
        path = new Path(transform.position);

        for (int i = 0; i < startAmount; i++)
        {
            Vector2 newPosition = path.GetSegmentFromBack(1).GetNextRandomPosition(path.GetSegmentFromBack(4),
                Random.Range(minAngle, maxAngle), Random.Range(minR, maxR));
            path.AddSegment(newPosition);
        }
        
        UpdateRoad();
    }

    [ContextMenu("Add random segment")]
    public void AddRandomSegment()
    {
        path.AddSegment(path.GetSegmentFromBack(1).GetNextRandomPosition(path.GetSegmentFromBack(4), Random.Range(minAngle, maxAngle),Random.Range(minR,maxR)));
        UpdateRoad();
    }

    [ContextMenu("Remove last segment")]
    public void RemoveLastSegment()
    {
        path.DeleteSegment();
        Destroy(transform.GetChild(0).gameObject);
        AddRandomSegment();
    }

    public void CheckMilestone(int index)
    {
        _milestone = index;
    }

    private void UpdateRoad()
    {
        Vector2[] points = path.CalculateEvenlySpacedPoints(spacing);
        _meshFilter.mesh = CreateRoadMesh(points);

        int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * .05f);
        _meshRenderer.sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);
    }
    
    Mesh CreateRoadMesh(Vector2[] points)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        
        int numTris = 2 * (points.Length - 1);
        int[] tris = new int[numTris * 3];
        
        int vertIndex = 0, trisIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 forward = Vector2.zero;
            if (i < points.Length - 1)
            {
                forward += points[(i + 1)%points.Length] - points[i];
            }
            if (i > 0)
            {
                forward += points[i] - points[(i - 1 + points.Length)%points.Length];
            }

            forward.Normalize();
            Vector2 left = new Vector2(-forward.y, forward.x);

            verts[vertIndex] = points[i] + left * roadWidth * .5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * .5f;

            float completionPercent = i / (float)(points.Length - 1);
            float v = 1 - Mathf.Abs(2 * completionPercent - 1);
            uvs[vertIndex] = new Vector2(0, v);
            uvs[vertIndex + 1] = new Vector2(1, v);
            
            if (i < points.Length - 1)
            {
                tris[trisIndex] = vertIndex;
                tris[trisIndex + 1] = (vertIndex + 2) % verts.Length;
                tris[trisIndex + 2] = vertIndex + 1;

                tris[trisIndex + 3] = vertIndex + 1;
                tris[trisIndex + 4] = (vertIndex + 2) % verts.Length;
                tris[trisIndex + 5] = (vertIndex + 3)  % verts.Length;
            }

            vertIndex += 2;
            trisIndex += 6;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        
        CreateMilestones();

        return mesh;
    }

    private void CreateMilestones()
    {
        for (int i = 0; i < path.points.Count; i++)
        {
            if (i % 3 == 0 && transform.childCount < i/3 + 1 )
            {
                Vector2 local = path.points[i];
                Milestone milestone = Instantiate(milestonePrefab, transform).GetComponent<Milestone>();
                milestone.transform.localPosition = local;
                milestone.milestoneIndex = i / 3;
                
                if (milestone.milestoneIndex == 0)
                {
                    milestone.visited = true;
                }
            }
        }
    }
}
