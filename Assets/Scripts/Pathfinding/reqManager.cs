using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class reqManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestsQueue = new Queue<PathRequest>();
    PathRequest CurrentPathRequest;
    Pathfinding pathfinding;
    bool isProcessingPath;
    static reqManager instance;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback,bool _inWater)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback,_inWater);
        instance.pathRequestsQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }


    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestsQueue.Count > 0)
        {
            CurrentPathRequest = pathRequestsQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.startFindPath(CurrentPathRequest.pathStart, CurrentPathRequest.pathEnd,CurrentPathRequest.inWater);
        }
    }
    public void finishedProcessing(Vector3[] path, bool sucess)
    {
        CurrentPathRequest.callback(path, sucess);
        isProcessingPath = false;
        TryProcessNext();
    }
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public bool inWater;
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback,bool _inWater)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            inWater = _inWater; 
        }
    }
}
