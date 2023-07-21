using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T> {
    T[] items;
    int count;

    public Heap(int maxHeapSize) {
        items = new T[maxHeapSize];
    }

    public void Add(T item) {
        item.heapIndex = count;
        items[count] = item;
        SortUp(item);
        count++;
    }

    public T Pop() {
        T firstItem = items[0];
        count--;
        items[0] = items[count];
        items[0].heapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public int Count {
        get {
            return count;
        }
    }

    public void Update(T item) {
        SortUp(item);
    }

    public bool Contains(T item) {
        return Equals(items[item.heapIndex], item);
    }

    void SortUp(T item) {
        int parentIndex = (item.heapIndex - 1) / 2;

        while (true) {
            T parentItem = items[parentIndex];

            if (item.CompareTo(parentItem) > 0) {
                Swap(item, parentItem);
            }
            else {
                return;
            }
            parentIndex = (item.heapIndex - 1) / 2;
        }
    }

    void SortDown(T item) {
        while (true) {
            int childIndexLeft = item.heapIndex * 2 + 1;
            int childIndexRight = item.heapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < count) {
                swapIndex = childIndexLeft;

                if (childIndexRight < count) {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0) {
                    Swap(item, items[swapIndex]);
                }
                else {
                    return;
                }
            }
            else {
                return;
            }
        }
    }

    void Swap(T itemA, T itemB) {
        items[itemA.heapIndex] = itemB;
        items[itemB.heapIndex] = itemA;
        int itemAIndex = itemA.heapIndex;
        itemA.heapIndex = itemB.heapIndex;
        itemB.heapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T> {
    int heapIndex {
        get;
        set;
    }
}