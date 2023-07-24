using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Pathfinding : MonoBehaviour
{



    reqManager PreqManager;

    Grid grid;


    private void Awake()
    {
        PreqManager = GetComponent<reqManager>();
        grid = GetComponent<Grid>();

    }


    public void startFindPath(Vector3 startPos, Vector3 targetPos,bool waterGrid)
    {
        StartCoroutine(FindPath(startPos, targetPos,waterGrid));
    }
    public IEnumerator FindPath(Vector3 startPos, Vector3 endPos,bool waterGrid)
    {
        Vector3[] Waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.nodeFromWorld(startPos);
        Node targetNode = grid.nodeFromWorld(endPos);

        if (startNode.walkable && targetNode.walkable)
        {


            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                    {
                        if (openSet[i].hCost < node.hCost)
                            node = openSet[i];
                    }
                }
                openSet.Remove(node);
                closedSet.Add(node);

                if (node == targetNode)
                {

                    pathSuccess = true;

                    break;
                }
                // List<Node> neighs;


                foreach (Node neighbour in grid.getNeighbours(node))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

        }
        else
        {
           // print("unwalkable");
        }

        if (pathSuccess)
        {
            Waypoints = retracePath(startNode, targetNode);

            pathSuccess = Waypoints.Length > 0;

        }
        PreqManager.finishedProcessing(Waypoints, pathSuccess);
        yield return null;
    }

    Vector3[] retracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = simplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }
    Vector3[] simplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 dirOld = new Vector2(0, 0);
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 dirNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (dirNew != dirOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            dirOld = dirNew;
        }
        return waypoints.ToArray();
    }
    int GetDistance(Node nodeA, Node NodeB)
    {
        int dX = Mathf.Abs(nodeA.gridX - NodeB.gridX);
        int dY = Mathf.Abs(nodeA.gridY - NodeB.gridY);
        if (dX > dY)
        {
            return 14 * dY + 10 * (dX - dY);
        }
        else
            return 14 * dX + 10 * (dY - dX);
    }
}
