using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {
    public bool walkable;
    public Vector3 position;

    public int movementPenalty;

    public int gCost;
    public int hCost;

    public int x;
    public int y;

    public Node parent;
    int HeapIndex;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public Node(bool walkable, Vector3 position, int x, int y, int movementPenalty) {
        this.walkable = walkable;
        this.position = position;
        this.x = x;
        this.y = y;
        this.movementPenalty = movementPenalty;
    }

    public int heapIndex {
        get {
            return HeapIndex;
        }
        set {
            HeapIndex = value;
        }
    }

    public int CompareTo(Node node) {
        int compare = fCost.CompareTo(node.fCost);

        if (compare == 0) {
            compare = hCost.CompareTo(node.hCost);
        }
        return -compare;
    }
}