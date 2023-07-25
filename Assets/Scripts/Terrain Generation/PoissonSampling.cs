using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonSampling 
{
   public List<Vector2> GetPoissonPoints(int numPoints,float minRad,int gridSize,int maxAttempts=1000,bool protectEdge=true)
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < numPoints; i++)
        {
            bool pointValid=false;
            int attempts = 0;
            Vector2 testPoint=new Vector2();
            while (!pointValid)
            {
                attempts++;
                if(protectEdge)
                 testPoint=new Vector2(Random.Range(minRad,gridSize-minRad), Random.Range(minRad, gridSize - minRad));
                else
                    testPoint = new Vector2(Random.Range(10, gridSize - 10), Random.Range(10, gridSize - 10));
                pointValid = validatePoint(testPoint, points, minRad);
                if (attempts > maxAttempts)
                    break;
            }
            if (attempts > maxAttempts)
                break;
            points.Add(testPoint);
        }
        return points;
    }

    bool validatePoint(Vector2 point, List<Vector2> points,float minRad)
    {
        foreach(Vector2 p in points)
        {
            if(Vector2.Distance(point,p)<minRad*2)
                return false;
        }
        return true;
    }
    public List<Vector2> getPoissoninArea(int numPoints,float minRad,Dictionary<Vector2,int> areaKeys,Dictionary<int,Vector2> area,int maxAttempts)
    {
        List<Vector2> points = new List<Vector2>();
        int[]range=getBounds(area);
        for (int i = 0; i < numPoints; i++)
        {
            bool pointValid = false;
            int attempts = 0;
            Vector2 testPoint = new Vector2();
            while (!pointValid)
            {
                attempts++;
                testPoint = new Vector2(Random.Range(range[0], range[1]), Random.Range(range[2], range[3]));
                pointValid = validatePoint(testPoint, points, minRad) && areaKeys.ContainsKey(testPoint);
                if (attempts > maxAttempts)
                    break;
            }
            if (attempts > maxAttempts)
                break;
            points.Add(testPoint);
        }
        return points;
    }

    int[] getBounds(Dictionary<int,Vector2> areaKeys)
    {
        float minX = 1000000000;
        float maxX = 0;
        float minY = 1000000000;
        float maxY = 0;

        for (int i= 0; i < areaKeys.Count; i++)
        {
            if(areaKeys[i].x<minX)
                minX = areaKeys[i].x;
            if(areaKeys[i].y<minY)
                minY = areaKeys[i].y;
            if(areaKeys[i].x>maxX)
                maxX = areaKeys[i].x;
            if(areaKeys[i].y>maxY)
                maxY = areaKeys[i].y;  
        }
        int[] range= new int[] { (int)minX, (int)maxX, (int)minY, (int)maxY };
        return range;
    }
}
