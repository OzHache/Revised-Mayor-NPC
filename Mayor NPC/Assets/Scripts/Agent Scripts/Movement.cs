using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Generates a node based movement sceme 
/// </summary>
public class Movement : MonoBehaviour
{

    private readonly List<Node> m_nodes = new List<Node>();
    private readonly List<Node> m_openList = new List<Node>();
    //It's impassable for all objects that travel
    private static readonly List<Vector2> m_impassable = new List<Vector2>();
    private Node m_destination;
    public float m_proximity = 0.01f;
    [SerializeField] private Vector3 m_debugMovement;
    [SerializeField] private bool m_debugMove = false;
    [SerializeField] private bool m_debugIsMoving = false;

    private void Start()
    {
        m_debugMovement = transform.position;
    }
    private void Update()
    {
        if (m_debugMove && !m_debugIsMoving && Vector2.Distance(transform.position, m_debugMovement) > 1.5f)
        {
            bool canArrive = CanGetToDestination(m_debugMovement, 1.5f);
            m_debugMove = false;
            m_debugIsMoving = canArrive;
            Debug.Log("Can Arrive at destination: " + canArrive);
        }
    }
    public bool didArrive()
    {
        float distance = Vector2.Distance(gameObject.transform.position, m_destination.location);
        return distance < m_proximity;
    }
    public Vector2 GetNextCoordinate()
    {
        if (m_nodes.Count < 1)
        {
            return m_destination.location;
        }

        Vector2 location = m_nodes[0].location;
        m_nodes.RemoveAt(0);
        return location;
    }

    internal Vector3 GetDestination()
    {
        return m_destination.location;
    }

    public bool CanGetToDestination(Vector3 destination, float maxDistance)
    {

        m_destination = new Node();
        m_destination.location = new Vector2(destination.x, destination.y);
        bool validPath = Search(destination, maxDistance);
        return validPath;
    }

    private bool Search(Vector2 destination, float maxDistance)
    {
        m_impassable.Clear();
        m_openList.Clear();
        m_nodes.Clear();
        //Start at this location
        Node start = new Node();

        m_openList.Add(start);
        m_openList[0].location = CenterOfGridAt(transform.position);

        start.G = 0; //There is no distance from here to the start;
        start.H = Vector2.Distance(start.location, destination);
        int maxCount = 1000;
        int count = 0;
        while (m_openList.Count > 0 && count < maxCount)
        {
            count++;
            Node current = GetLowestFScore();

            if (Vector2.Distance(current.location, m_destination.location) < maxDistance)
            {
                //Set this as the new destination
                m_destination = current;
                return ReconstructPath(current);
            }
            //remove the open set
            m_openList.Remove(current);

            //for each neighbour of current
            foreach (Node node in GetNeighbours(current))
            {
                //Debug.Log("Checking Node at" + node.location);
                float tenativeGScore = current.G + Vector2.Distance(current.location, node.location);
                if (tenativeGScore < node.G)
                {
                    //this is a better path
                    node.m_cameFrom = current;
                    node.G = tenativeGScore;
                    if (m_openList.Contains(node))
                    {
                        continue;
                    }

                    m_openList.Add(node);
                }
            }

        }
        return false;

    }

    private bool ReconstructPath(Node current)
    {
        while (current.m_cameFrom != null)
        {
            m_nodes.Add(current);
            current = current.m_cameFrom;
        }
        m_nodes.Reverse();
        //return if we have found a valid path (this should never fail)
        return m_nodes.Count > 0 && m_nodes[m_nodes.Count - 1] == m_destination;
    }

    private Node GetLowestFScore()
    {
        int index = 0;
        for (int i = 0; i < m_openList.Count; i++)
        {
            if (m_openList[i].GetF() < m_openList[index].GetF())
            {
                index = i;
            }
        }
        return m_openList[index];
    }

    //Get the center of the given grid location
    private Vector2 CenterOfGridAt(Vector3 position)
    {
        Vector2 pos = position;
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;
        return pos;
    }

    private List<Node> GetNeighbours(Node home)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue; //skip myself
                }
                Vector2 direction = new Vector2(x, y);
                Vector2 destination = home.location + direction;
                if (m_impassable.Contains(destination))
                {
                    continue;
                }

                //see if this location is on the list of obstacle
                if (GridManager.GetGridManager().GridCellIsFilled(GridManager.Layers.k_obstacles, destination))
                {
                    m_impassable.Add(destination);
                    continue;
                }

                //see if there is an object in the way
                RaycastHit2D[] hits;
                //go to the center of the next unit
                hits = Physics2D.RaycastAll(home.location, direction, direction.magnitude);
                foreach (RaycastHit2D hit in hits)
                {
                    //if we have hit something, see if that is on the cell we are moving to.
                    if (hit.transform != null)
                    {
                        //if what I hit was a tile map collider, then I should have already ignored it
                        if (hit.collider is TilemapCollider2D)
                        {
                            continue;
                        }
                        //this is a trigger and so I can pass through it
                        else if (hit.collider.isTrigger)
                        {
                            continue;
                        }
                        else
                        {
                            //see if the center of the the object is within the cell I am trying to move to
                            if (Vector2.Distance(destination, hit.transform.position) < 0.5f)
                            {
                                m_impassable.Add(destination);
                                break;
                            }
                        }
                    }
                }
                //if the destination was added as an impassable location
                if (m_impassable.Contains(destination))
                {
                    continue;
                }
                //otherwise add the node 
                Node node = new Node();
                node.location = home.location + direction;
                //Distance to the destination
                node.H = Vector2.Distance(node.location, m_destination.location);
                neighbours.Add(node);

            }
        }
        return neighbours;
    }
}

internal class Node
{
    internal Vector2 location;
    internal Vector2 destination;
    internal Node m_cameFrom;
    internal float G = float.MaxValue;
    internal float H = float.MaxValue;
    internal float GetF() { return G + H; }
    public override bool Equals(object obj) => Equals(obj as Node);
    public bool Equals(Node other)
    {
        if (other is null)
        {
            return false;
        }

        // Optimization for a common success case.
        if (System.Object.ReferenceEquals(this, other))
        {
            return true;
        }
        return other.location == location;
    }

}
