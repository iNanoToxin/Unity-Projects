using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour {

    Grid grid;

    private void Awake() {
        grid = GetComponent<Grid>();
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int dx = Mathf.Abs(nodeA.x - nodeB.x);
        int dy = Mathf.Abs(nodeA.y - nodeB.y);
        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);

        return 14 * min + 10 * (max - min);
    }

    Vector3[] RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 oldDirection = Vector2.zero;

        for (int i = 1; i < path.Count; i++) {
            Vector2 newDirection = new Vector2(path[i - 1].x - path[i].x, path[i - 1].y - path[i].y);

            if (oldDirection != newDirection) {
                waypoints.Add(path[i].position);
            }
            oldDirection = newDirection;
        }
        return waypoints.ToArray();
    }

    public void FindPath(PathRequest pathRequest, Action<PathResult> callback) {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(pathRequest.pathStart);
        Node endNode = grid.NodeFromWorldPoint(pathRequest.pathEnd);

        if (startNode.walkable && endNode.walkable) {

            Heap<Node> openSet = new Heap<Node>(grid.maxSize);
            HashSet<Node> closeSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0) {
                Node currentNode = openSet.Pop();

                closeSet.Add(currentNode);

                if (currentNode == endNode) {
                    pathSuccess = true;
                    sw.Stop();
                    UnityEngine.Debug.Log("Path found:" + sw.ElapsedMilliseconds + " ms");
                    break;
                }

                foreach (Node neighbor in grid.GetNeighbors(currentNode)) {
                    if (!neighbor.walkable || closeSet.Contains(neighbor)) {
                        continue;
                    }

                    int newMovementCost = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;

                    if (newMovementCost < neighbor.gCost || !openSet.Contains(neighbor)) {
                        neighbor.gCost = newMovementCost;
                        neighbor.hCost = GetDistance(neighbor, endNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor)) {
                            openSet.Add(neighbor);
                        }
                        else {
                            openSet.Update(neighbor);
                        }
                    }
                }
            }
        }

        if (pathSuccess) {
            waypoints = RetracePath(startNode, endNode);
            pathSuccess = waypoints.Length > 0;
        }

        callback(new PathResult(waypoints, pathSuccess, pathRequest.callback));
    }
}
