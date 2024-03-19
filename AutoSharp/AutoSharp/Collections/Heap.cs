using System;
using System.Collections.Generic;

namespace AutoSharp.Collections
{
    public class Heap<T> where T : IComparable<T>
    {
        public Heap()
        {
            heap = new List<T>();
        }

        private readonly List<T> heap;

        public int Count => heap.Count;

        public bool IsEmpty => heap.Count == 0;

        public void Push(T value)
        {
            heap.Add(value);
            HeapifyUp(heap.Count - 1);
        }

        public T Pop()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }

            T min = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);
            return min;
        }

        public T Peek()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }

            return heap[0];
        }

        public bool Remove(T value)
        {
            int index = heap.IndexOf(value);
            if (index == -1)
            {
                return false;
            }

            heap[index] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            HeapifyDown(index);
            HeapifyUp(index);

            return true;
        }

        public void Clear()
        {
            heap.Clear();
        }

        private int Parent(int index)
        {
            return (index - 1) / 2;
        }

        private int LeftChild(int index)
        {
            return 2 * index + 1;
        }

        private int RightChild(int index)
        {
            return 2 * index + 2;
        }

        private void Swap(int index1, int index2)
        {
            var temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }

        private void HeapifyUp(int index)
        {
            int parent = Parent(index);
            if (index > 0 && heap[index].CompareTo(heap[parent]) < 0)
            {
                Swap(index, parent);
                HeapifyUp(parent);
            }
        }

        private void HeapifyDown(int index)
        {
            int minIndex = index;
            int left = LeftChild(index);
            int right = RightChild(index);

            if (left < heap.Count && heap[left].CompareTo(heap[minIndex]) < 0)
            {
                minIndex = left;
            }

            if (right < heap.Count && heap[right].CompareTo(heap[minIndex]) < 0)
            {
                minIndex = right;
            }

            if (index != minIndex)
            {
                Swap(index, minIndex);
                HeapifyDown(minIndex);
            }
        }
    }
}
