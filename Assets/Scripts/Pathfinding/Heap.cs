using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T:IHeapItem<T>
{
    T[] items;
    int currentItemCount;
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
     
    }
 
    public T removeFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }


    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex],item);
    }
    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }
    public void UpdateItem(T item)
    {
        SortUp(item);

    }
    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = (item.HeapIndex * 2) + 1;
            int childIndexRight = (item.HeapIndex * 2) + 2;
            int swapIndex = 0;
            if(childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight])<0)
                    {
                        swapIndex=childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    swap(item,items[swapIndex]);
                }
                else
                {
                    return;
                }
            }

            else
            {
                return ;
            }
        }
    }
    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
        {
            T parentItem=items[parentIndex];
            if(item.CompareTo(parentItem) > 0)
            {
                swap(item,parentItem);
            }
            else
            {
                break;
            }
            parentIndex=(item.HeapIndex-1)/2;
        }
    }

    void swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] =itemA;
        int itemAIndex=itemA.HeapIndex;
        itemA.HeapIndex=itemB.HeapIndex;
        itemB.HeapIndex=itemAIndex;
    }

    List<T> closedList = new List<T>();
    public void Add(T item)
    {
        if (currentItemCount >= items.Length)
        {
            ResizeHeap();
        }

        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;

        closedList.Add(item);
    }

    void ResizeHeap()
    {
        T[] newNodes = new T[items.Length + 100];

        for (int i = 0; i < items.Length; i++)
        {
            newNodes[i] = items[i];
        }

        items = newNodes;
    }

    public void ResetNodes()
    {
        foreach (T node in closedList)
        {
            node.HeapIndex = 0;
        }
    }


    }


public interface IHeapItem<T>: IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }

}
