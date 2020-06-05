using System;
using UnityEngine;

public class Milestone : MonoBehaviour
{
    public int milestoneIndex;
    public bool visited;
    public int xDespawnDistance;
    
    private Renderer _renderer;
    private RoadCreator _roadCreator;

    private void Start()
    {
        _roadCreator = FindObjectOfType<RoadCreator>();
    }

    private void OnTriggerExit2D(Collider2D other1)
    {
        if (!visited)
        {
            visited = true; 
            _roadCreator.CheckMilestone(milestoneIndex);
        }
    }

    private void Update()
    {
        if (visited && xDespawnDistance > transform.position.x)
        {
            _roadCreator.RemoveLastSegment();
        }
    }
}