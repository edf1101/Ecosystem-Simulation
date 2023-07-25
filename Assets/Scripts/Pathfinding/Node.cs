using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node :IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;
    public int penalty;
    public Node(bool _walkable,Vector3 _worldPosition,int _gridX,int _gridy,int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridy;
        penalty = _penalty;
    }
    public int fCost
    {
        get { return gCost+hCost; }
    }

    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }
    public int CompareTo(Node nodeToComp)
    {
        int compare=fCost.CompareTo(nodeToComp.fCost);
        if (compare == 0)
        {
            compare=hCost.CompareTo(nodeToComp.hCost);

        }
        return -compare;
    }

}
