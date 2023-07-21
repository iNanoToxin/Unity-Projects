using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathRequestManager : MonoBehaviour {

    Queue<PathResult> results = new Queue<PathResult>();
    static PathRequestManager instance;
    Pathfinding pathfinding;

    private void Awake() {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update() {
        if (results.Count > 0) {
            int itemsInQueue = results.Count;

            lock (results) {
                for (int i = 0; i  < itemsInQueue; i++) {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    public static void RequestPath(PathRequest pathRequest) {
        ThreadStart threadStart = delegate {
            instance.pathfinding.FindPath(pathRequest, instance.FinishProcessingPath);
        };
        threadStart.Invoke();
    }

    public void FinishProcessingPath(PathResult pathResult) {
        lock (results) {
            results.Enqueue(pathResult);
        }
    }
}

public struct PathRequest {
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}

public struct PathResult {
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback) {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}