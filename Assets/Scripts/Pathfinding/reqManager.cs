using System.Collections.Generic;
using UnityEngine;
using System;

// inspired by Seb Lagues video, not what I used in my end NEA but this is faster
// so implemented here

public class reqManager : MonoBehaviour
{
	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;

	static reqManager instance;
	Pathfinding pathfinding;

	bool isProcessingPath;

	void Awake()
	{
		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback,GameObject self)
	{
		PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback,self);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}

	void TryProcessNext()
	{
		if (!isProcessingPath && pathRequestQueue.Count > 0)
		{
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success)
	{
		if(currentPathRequest.self != null)
			currentPathRequest.callback(path, success);
		isProcessingPath = false;
		TryProcessNext();
	}

	struct PathRequest
	{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;
		public GameObject self;
		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback,GameObject _self)
		{
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
			self = _self;
		}

	}
}