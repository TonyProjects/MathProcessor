using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace ElshinAnton.DataStructures
{
    class TheClass
    {
        public int i { get; set; }
    }
    static class Test
    {
        static void Main(string[] args)
        {
            var list = new List<TheClass>()
            {
                new TheClass() {i = 9},
                new TheClass() {i = 5},
                new TheClass() {i = 8}
            };

            var arrr = list.ToArray();
            Array.Sort(arrr, (a,b) => (a.i > b.i) ? 1 : (a.i < b.i) ? -1 : 0);

            list = new List<TheClass>(arrr);

            list.ForEach(i => Console.WriteLine(i.i.ToString() + ' '));

            Console.ReadKey();

            //Algorithms.Sort.Test_MergeSort();
        }
    }
}



namespace ElshinAnton.DataStructures
{
    #region DataStructures

    public class DynamicArray<T>
    {
        private T[] cargo;
        private int count;      // length user thinks array is
        private int capacity;   // actual array size;

        public DynamicArray()
        {
            capacity = 16;
            cargo = new T[capacity];
        }

        public DynamicArray(int capacity)
        {
            if (capacity < 0) 
                throw new ArgumentException("Negative array size " + capacity.ToString());

            this.capacity = capacity;
            cargo = new T[capacity];
        }

        public int Count { get => count; }

        public bool IsEmpty { get => count == 0; }

        public T this[int i]
        {
            get => cargo[i];

            set => cargo[i] = value;
        }

        public void clear()
        {
            cargo = new T[16];
        }

        public virtual void Add(T item)
        {
            if (count + 1 >= capacity)
            {
                capacity = capacity == 0 ? 1 : 2 * capacity;

                Array.Resize(ref cargo, capacity);
            }

            cargo[count++] = item;
        }

        public void RemoveAt(int index)
        {
            Array.Copy(cargo, index + 1, cargo, index, count - index - 1);
            count -= 1;
        }

        public T Find(T item)
        {
            for (int i = 0; i < Count; ++i)
            {
                if (Object.Equals(item, cargo[i]))
                    return item;
            }
            return default(T);
        }

        public bool Contains(T item)
        {
            return object.Equals(item, Find(item));
        }
    
        public int IndexOf(T item)
        {
            for (int i = 0; i < count; ++i)
            {
                if (object.Equals(item, cargo[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public DynamicArray<T> Clone()
        {
            var clone = new DynamicArray<T>(capacity);

            for (int i = 0; i < count; ++i)
                clone.Add(this[i]);

            return clone;
        }
    }

    public class LinkedListNode<T>
    {
        public LinkedListNode<T> Prev { get; set; }
        public LinkedListNode<T> Next { get; set; }
        public T Data { get; set; }
    }

    public class LinkedList<T> : ICollection<T>
    {
        private LinkedListNode<T> head;
        private LinkedListNode<T> tail;
        private Int32 count;

        public LinkedList()
        {
        }

        public Int32 Count { get => count; }

        public bool IsReadOnly { get => false; }

        public void Add(T item)
        {
            AddLast(item);
        }

        public void AddLast(T item)
        {
            if (count == 0)
            {
                head = new LinkedListNode<T>() { Data = item };
                tail = head;
            }
            else
            {
                tail.Next = new LinkedListNode<T>() { Data = item, Prev = tail };
                tail = tail.Next;
            }
            count++;
        }

        public void AddFirst(T item)
        {
            if (count == 0)
            {
                head = new LinkedListNode<T>() { Data = item };
                tail = head;
            }
            else
            {
                head.Prev = new LinkedListNode<T>() { Data = item, Next = head };
                head = head.Prev;
            }
            count++;
        }

        public bool Remove(T item)
        {
            var pointer = Find(item);

            if (pointer != null)
            {
                pointer.Prev.Next = pointer.Next;
                --count;
                return true;
            }

            return false;

        }

        public void RemoveLast()
        {
            if (count == 0) return;

            if (count == 1) head = tail = null;

            else
            {
                tail = tail.Prev;
                tail.Next = null;
            }

            --count;
        }

        public void RemoveFirst()
        {
            if (count == 0) return;

            if (count == 1) head = tail = null;

            else
            {
                head = head.Next;
                head.Prev = null;
            }

            --count;
        }

        public void Clear()
        {
            head = tail = null;
            count = 0;
        }

        public LinkedListNode<T> First { get => head; }

        public LinkedListNode<T> Last { get => tail; }

        public LinkedListNode<T> Find(T item)
        {
            var pointer = head;
            while (pointer != null)
            {
                if (item.Equals(pointer.Data))
                    return pointer;
                pointer = pointer.Next;
            }
            return pointer;
        }

        public LinkedListNode<T> FindBackward(T item)
        {
            var pointer = tail;
            while (pointer != null)
            {
                if (item.Equals(pointer.Data))
                    return pointer;
                pointer = pointer.Prev;
            }
            return pointer;
        }

        public bool Contains(T item)
        {
            return Find(item) != null;
        }

        public void CopyTo(T[] array, Int32 index)
        {

        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class Stack<T> : ICollection
    {
        private LinkedList<T> list;

        public Stack()
        {
            list = new LinkedList<T>();
        }

        public int Count { get => list.Count; }

        public bool IsEmpty { get => list.Count == 0; }

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public T Peek()
        {
            if (IsEmpty)
                throw new System.IndexOutOfRangeException("Stask is empty");

            return list.Last.Data;
        }
        public T Pop()
        {
            if (IsEmpty)
                throw new System.IndexOutOfRangeException("Stask is empty");

            var node = list.Last; list.RemoveLast();
            return node.Data;
        }

        public void Push(T item)
        {
            list.Add(item);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class Queue<T> : ICollection
    {
        private LinkedList<T> list;

        public int Count => list.Count;

        public bool IsEmpty => list.Count == 0;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public Queue()
        {
            list = new LinkedList<T>();
        }

        public T Peek()
        {
            if (IsEmpty)
                throw new IndexOutOfRangeException("Queue is empty");
            return list.First.Data;
        }

        public T Dequeue()
        {
            if (IsEmpty)
                throw new IndexOutOfRangeException("Queue is empty");
            var node = list.First; list.RemoveFirst();
            return node.Data;
        }

        public void Enqueue(T item)
        {
            list.AddLast(item);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class Set<T> : DynamicArray<T>
    {
        public Set(int capacity) : base(capacity)
        { }

        public bool Add(T item)
        {
            if (base.Contains(item)) return false;

            base.Add(item);

            return true;
        }

        public void Unite(Set<T> B)
        {

        }
    }


    public class TreeNode<T>
    {
        public DynamicArray<TreeNode<T>> nodes { get; set; }
    }

    public class Tree<T>
    {
        private int count;
        private TreeNode<T> root;

        public void Unite(DynamicArray<T> branch)
        {

        }
    }



    interface IBinaryHeap<T>
    {
        void Push(T element);
        T Pop(T element);
    }

    public class LinkedBinaryHeapNode<T>
    {
        public LinkedBinaryHeapNode<T> Up { get; set; }
        public LinkedBinaryHeapNode<T> Left { get; set; }
        public LinkedBinaryHeapNode<T> Right { get; set; }

        public T Data { get; set; }
    }

    public class LinkedBinaryHeap<T> : IBinaryHeap<T>
    {
        private LinkedBinaryHeapNode<T> root;
        
        private int depth;
        private int width;
        private int count;

        public void Push(T element)
        {
            var iterator = root;
            var node = new LinkedBinaryHeapNode<T> 
            { 
                Up=null, Left=null, Right=null, 
                Data=element
            };

            if (count == 0)
                root = node;

            else
            {
                while (true)
                {
                    
                }
            }

            ++count;
        }

        public T Pop(T element)
        {
            return root.Data;
        }
    }

    public class ArrayBinaryHeap<T> : IBinaryHeap<T>
    {
        private T[] cargo;
        
        private int depth;
        private int width;
        private int count;
        private int capacity;

        public void Push(T element)
        {

        }

        public T Pop(T element)
        {
            return cargo[count];
        }
    }


    public class Routes<T>
    {
        int count = 0;
        Tuple<T, T, DynamicArray<T>>[] routes;

        public Routes(int capacity)
        {
            routes = new Tuple<T, T, DynamicArray<T>>[capacity];
        }

        public int AddRoute(T from, T to, DynamicArray<T> route)
        {
            routes[count] = new Tuple<T, T, DynamicArray<T>>(from, to, route);
            return ++count == routes.Length ? --count : count;
        }
        public int AddRoute(Tuple<T, T, DynamicArray<T>> route)
        {
            routes[count] = route;
            return ++count == routes.Length ? --count : count;
        }

        public T GetRouteA(int i) => routes[i].Item1;
        public T GetRouteB(int i) => routes[i].Item2;
        public DynamicArray<T> GetRoute(int i) => routes[i].Item3;

        public int IndexOf(T from, T to)
        {
            for (int i = 0; i < routes.Length; ++i)
            {
                if (Object.Equals(routes[i].Item1, from)
                && Object.Equals(routes[i].Item2, to))
                {
                    return i;
                }
            }
            return -1;
        }
   
        public Tuple<T, T, DynamicArray<T>> CloneRoute(int i)
        {
            return new Tuple<T, T, DynamicArray<T>>
            (
                routes[i].Item1,
                routes[i].Item2,
                routes[i].Item3.Clone()
            );
        }
    


        public Set<Edge<T>> ToEdgeSet()
        {
            var set = new Set<Edge<T>>(16);

            foreach(var route in routes)
            {
                var r = route.Item3;

                for (int i = 0; i < r.Count - 1; ++i)
                {
                    var edge = new Edge<T>();
                        edge.vertex[0] = r[i];
                        edge.vertex[1] = r[i + 1];

                    set.Add(edge);
                }
            }

            return set;
        }
    }
  
    
    public class Vertexes<T>
    {
        T[] vertexes;

        public Vertexes(T[] vertexes)
        {
            this.vertexes = vertexes;
        }

        public int IndexOf(T v)
        {
            for (int i = 0; i < vertexes.Length; ++i)
            {
                if (v.Equals(vertexes[i]))
                    return i;
            }
            return -1;
        }
    }

    public class Vertex<T>
    {
        public T Value { get; set; }
    }

    public class Edge<T>
    {
        public T[] vertex = new T[2];
    }

    public class Graph<T>
    {

        int[,] adjacency;

        T[] vertexes;

        Vertexes<T> vertexes2;
        Routes<T> graphRoutes;

        public Graph(T[] vertexes, int[,] adj)
        {
            this.vertexes = vertexes;
            this.adjacency = adj;
            this.vertexes2 = new Vertexes<T>(vertexes);
        }

        public Routes<T> GraphRoutes 
        { 
            get { return graphRoutes ??= FindGraphRoutes(); } 
        }


        public int DepthFind(int a, int b)
        {
            var vertexQueue = new Queue<int>();
                vertexQueue.Enqueue(a);

            var vertexList = new DynamicArray<int>(adjacency.Length);

            while (!vertexQueue.IsEmpty)
            {
                var c = vertexQueue.Dequeue();
                for (int j = 0; j < adjacency.Length - 1; ++j)
                {
                    if (adjacency[c, j] == 1 
                    && !vertexList.Contains(j))
                    {
                        vertexList.Add(j);
                        vertexQueue.Enqueue(j);
                    }
                }
            }

            return -1;
        }
        public int WidthFind(int a, int b)
        {
            var vertexStack = new Stack<int>();
                vertexStack.Push(a);

            var vertexList = new DynamicArray<int>(adjacency.Length);

            while (!vertexStack.IsEmpty)
            {
                var c = vertexStack.Pop();
                for (int j = 0; j < adjacency.Length - 1; ++j)
                {
                    if (adjacency[c, j] == 1
                    && !vertexList.Contains(j))
                    {
                        vertexList.Add(j);
                        vertexStack.Push(j);
                    }
                }
            }

            return -1;
        }
    
 
        private Routes<T> FindGraphRoutes()
        {
            var routeCount = vertexes.Length - 1;
            var vertexCount = vertexes.Length;
            var graphaRoutes = new Routes<T>(routeCount * routeCount);

            for (int i = 0; i < vertexCount; ++i)
            {
                var x = vertexes[i];

                var vertexQueue = new Queue<T>();
                    vertexQueue.Enqueue(x);

                var visitedVertexes = new DynamicArray<T>(vertexCount);
                var vertexRoutes = new Routes<T>(routeCount);

                while (!vertexQueue.IsEmpty)
                {
                    var c = vertexQueue.Dequeue();
                    for (int j = 0; j < vertexCount; ++j)
                    {
                        if (i == j) continue;

                        var vertex = vertexes[j];
                        
                        var childIsNotVisited = adjacency[vertexes2.IndexOf(c), vertexes2.IndexOf(vertex)] == 1 
                                                && !visitedVertexes.Contains(vertex);
                        if (childIsNotVisited)
                        {
                            vertexQueue.Enqueue(vertex);
                            visitedVertexes.Add(vertex);
                        }

                        var vertexIsSource = x.Equals(c);
                        if (childIsNotVisited && vertexIsSource)
                        {
                            var route = new DynamicArray<T>(vertexCount);
                                route.Add(x);
                                route.Add(vertex);

                            vertexRoutes.AddRoute(x, vertex, route);
                        }
                        else if (childIsNotVisited)
                        {
                            var route = vertexRoutes.GetRoute(vertexRoutes.IndexOf(x, c));
                                route = route.Clone();
                                route.Add(vertex);

                            vertexRoutes.AddRoute(x, vertex, route);
                        }
                    }
                }

                for (int r = 0; r < routeCount; ++r)
                {
                    var route = new Tuple<T, T, DynamicArray<T>>(
                        vertexRoutes.GetRouteA(r),
                        vertexRoutes.GetRouteB(r),
                        vertexRoutes.GetRoute(r));

                    graphaRoutes.AddRoute(route);
                }
            }

            return graphaRoutes;
        }
    }

    #endregion

}


namespace ElshinAnton.Algorithms
{
    public delegate int Comparator<T>(T a, T b);

    public static class Sort
    {
        #region sort
        /*
         * Для каждой пары групп (g[i], g[i+1]) 
         * 
         */
        static List<T> Merge<T>(List<T> list, int p, Comparator<T> c)
        {
            List<T> buffer = new List<T>(list.Count);

            int group_count = list.Count / p + (list.Count % p == 0 ? 0 : 1);
            int group_length = p;
            int group_module = list.Count % p;

            for (int g = 0; g < group_count - 1; ++g)
            {
                int g1 = g * p, g2 = (g + 1) * p;
                int gi = 0, gj = 0;

                while (gi < group_length && ((g + 1 == group_count && gj < group_module) || gj < group_length))
                {
                    int comparation = c(list[gi], list[gj]);

                    if (comparation == -1)
                    {
                        buffer.Add(list[g1 + gi++]);
                    }
                    else
                    {
                        buffer.Add(list[g2 + gj++]);
            }   }   }

            return buffer;
        }

        public static void MergeSort<T> (List<T> l1, Comparator<T> cmpr)
        {
            /*
             * 1) Есть последовательность
             * 2) разбиваем ее на p групп
             * 3) выполняем слияние групп
             */

            for (int p = 1; (int)p / 2 < l1.Count; p *= 2)
            {
                l1 = Merge(l1, p, cmpr);
            }
        }

        #endregion

        #region Test

        public static void Test_MergeSort()
        {
            List<int> list = new List<int> { 0, 5, 1, 3, 8, 2, 7, 6, 4 };

            MergeSort<int>(list, (a, b) => { return a < b ? -1 : a == b ? 0 : 1; });

            list.ForEach(element => { Console.Write(element); Console.Write(' '); });
            Console.ReadKey();
        }

        #endregion
    }

}