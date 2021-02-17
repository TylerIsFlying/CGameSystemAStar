using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAFollow : MonoBehaviour
{
    // serialized vars
    [Header("Targets")]
    [SerializeField]
    private Transform seeker, target;
    [SerializeField]
    private float speed;
    [Header("Range")]
    [SerializeField]
    private float detectRange = 0.1f;
    [Header("Grid")]
    [SerializeField]
    private TAGrid grid;
    [Header("Gizmos")]
    [SerializeField]
    private bool enableGizmos;
    [SerializeField]
    private Color gizmosColor;
    // private vars
    private List<TANode> path;
    private int pathCounter;
    private Vector3 targetLastPos;
    private void Start()
    {
        path = new List<TANode>();
        pathCounter = 0;
        path = TAPathing.SetTarget(seeker.position, target.position, grid);
        targetLastPos = target.transform.position;
    }
    private void Update()
    {
        if (path != null && (path.Count > 0 && !(pathCounter >= path.Count)))
        {
            if (Vector3.Distance(seeker.position, new Vector3(path[pathCounter].position.x, seeker.position.y, path[pathCounter].position.z)) < 1.0f)
            {
                pathCounter++;
            }
            else
            {
                seeker.position = Vector3.MoveTowards(seeker.position, new Vector3(path[pathCounter].position.x, seeker.position.y, path[pathCounter].position.z), Time.deltaTime * speed);
            }
        }
        if (Vector3.Distance(target.position, targetLastPos) > detectRange)
        {
            path = TAPathing.SetTarget(seeker.position, target.position, grid);
            targetLastPos = target.transform.position;
            pathCounter = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if ((Application.isPlaying && (path != null && path.Count > 0)) && enableGizmos) 
        {
             foreach(TANode n in path) 
             {
                Gizmos.color = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b);
                Gizmos.DrawCube(n.position, new Vector3(1,1,1));
            }
        }
    }
}
