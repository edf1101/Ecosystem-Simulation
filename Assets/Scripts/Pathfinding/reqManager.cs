using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// The A* algorithm/ request manager came from Seb lague as hadnt covered the algorithm at time of writing and also more performant 
//than my code which is a necessity when dealing with large( 300 calls/ s)

public class reqManager : MonoBehaviour
{
	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;

	static reqManager instance;
	Pathfinding pathfinding;
	[SerializeField] int queueLen;
	bool isProcessingPath;

	void Awake()
	{
		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool,bool> callback,GameObject self) // this gets called by my animal path script when it wants to get an A* path
	{
		PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback,self);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.queueLen = instance.pathRequestQueue.Count;
		instance.TryProcessNext();
	}

	void TryProcessNext() // after enqueuing an item from request path it ties to start looking for the path at front of queue if it isnt already
	{
		if (!isProcessingPath && pathRequestQueue.Count > 0)
		{
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success)// once finished finding  path return callback and then begin on next path
	{
		if(currentPathRequest.self != null)
			currentPathRequest.callback(path, success,false);
		isProcessingPath = false;
		TryProcessNext();
	}

	struct PathRequest // used for callbacks
	{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool,bool> callback;
		public GameObject self;
		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool,bool> _callback,GameObject _self)
		{
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
			self = _self;
		}

	}
}