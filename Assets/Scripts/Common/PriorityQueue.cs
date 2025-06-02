using System;
using System.Collections.Generic;

// 기본 값은 최소 우선순위 큐
public class PriorityQueue<T>
{
    // 힙 조건 : 부모노드는 항상 자식노드보다 우선순위가 낮아야한다. 
    private readonly List<T> heap;

    private readonly IComparer<T> comparer;

    public PriorityQueue() : this(Comparer<T>.Default) { }

    public PriorityQueue(IComparer<T> comparerValue)
    {
        heap = new List<T>();
        comparer = comparerValue;
    }

    public int Count => heap.Count;

    //맨 끝에 추가하고 힙 조건을 다시 체크
    public void Enqueue(T item)
    {
        // 맨 끝에 추가
        heap.Add(item);
        Refresh();
    }

    // 가장 낮은 우선순위를 가진 노드(맨 위에 있는 노드)을 반환하고 힙 조건을 다시 체크
    public T Dequeue()
    {
        if (Count == 0)
        {
            Console.WriteLine("PriorityQueue's Count is 0!");
            return default;
        }

        // 힙의 루트에 있는 항목이 가장 낮은 우선순위를 가진다.
        // 최종 return되는 값은 이 값이지만, 힙 조건을 만족하는지 검사를 해야한다.
        T item = heap[0];

        // 힙의 마지막 노드를 맨 위로 이동시킨다.
        heap[0] = heap[Count - 1];
        heap.RemoveAt(Count - 1);

        int index = 0;

        // 힙 조건을 확인할 때까지 계속 돈다.
        while (true)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;

            // 모든 자식 노드를 확인한 경우 종료.
            if (leftChildIndex >= Count)
                break;

            // 두 자식 노드 중 우선순위가 낮은 노드를 선택한다.
            int minChildIndex = (rightChildIndex < Count    //오른쪽 노드가 있는지 부터 검사.. 없다면 leftChildIndex를 반환한다.
                && comparer.Compare(heap[rightChildIndex], heap[leftChildIndex]) < 0) ?     //둘 중 더 낮은 우선순위를 가진 노드를 뽑아야 한다.
                rightChildIndex : leftChildIndex;

            // 위 조건에서 뽑은 노드와 현재 노드를 비교한다.

            // 뽑은 노드가 현재 노드보다 우선순위가 높다면, 힙 조건을 만족하는 것이니 안바꿔도 되고, 그만체크해도 된다.
            if (comparer.Compare(heap[index], heap[minChildIndex]) <= 0)
                break;

            // 뽑은 노드가 현재 노드보다 우선순위가 낮다면, 현재 노드와 뽑은 노드를 바꿔줌으로서 힙 조건을 만족하게 한다.
            Swap(index, minChildIndex);

            // 이제 뽑았던 노드의 자식 노드를 다시 검사한다.
            index = minChildIndex;
        }

        return item;
    }

    public void Refresh()
    {
        int index = Count - 1;

        // 힙 조건을 확인할 때까지 계속 돈다.
        while (index > 0)
        {
            //방금 추가한 노드의 부모 노드
            int parentIndex = (index - 1) / 2;

            //부모 노드의 우선순위가 방금 추가한 노드보다 낮다면 힙조건이 만족한 것.
            if (comparer.Compare(heap[index], heap[parentIndex]) >= 0)
                break;

            //부모 노드가 더 우선순위가 더 높다면 서로 바꾼다.
            Swap(index, parentIndex);

            //이제 바꾼 노드의 자식노드를 다시 검사한다.
            index = parentIndex;
        }
    }

    private void Swap(int indexA, int indexB)
    {
        T temp = heap[indexA];
        heap[indexA] = heap[indexB];
        heap[indexB] = temp;
    }
}