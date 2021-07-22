using System;
using System.Collections;
using System.Collections.Generic;

namespace MathProcessor
{
    namespace Collections
    {
        // Linked List
        public sealed class LinkedListNode<T>
        {
            internal LinkedListNode<T> next;
            internal LinkedListNode<T> prev;
            internal T item;

            internal LinkedListNode(T value)
            {
                item = value;
                next = null;
                prev = null;
            }

            internal LinkedListNode(T value, LinkedListNode<T> p, LinkedListNode<T> n)
            {
                item = value;
                next = n;
                prev = p;
            }

            public T Value
            {
                get { return item; }
                set { item = value; }
            }

            public LinkedListNode<T> Next
            {
                get { return next; }
                set { next = value; }
            }

            public LinkedListNode<T> Previous
            {
                get { return prev; }
                set { prev = value; }
            }
        }

        public sealed class LinkedList<T> : ICollection<T>
        {
            internal Int32 count;
            internal LinkedListNode<T> head;
            internal LinkedListNode<T> last;

            public LinkedList()
            {
                count = 0;
                head = null;
                last = null;
            }

            public LinkedList(T item)
            {
                head = last = new LinkedListNode<T>(item);
                count = 1;
            }

            // ICollection<T>
            public Int32 Count
            {
                get { return count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public void Clear()
            {
                count = 0;
                head = null;
            }

            public void Add(T item)
            {
                if (head == null)
                {
                    head = new LinkedListNode<T>(item);
                    last = head;
                    ++count;
                }
                else
                {
                    LinkedListNode<T> node = new LinkedListNode<T>(item, last, head);
                    head.prev = node;
                    last.next = node;
                    last = node;
                    ++count;
                }
            }

            public bool Remove(T item)
            {
                if (count == 0) return false;

                if (count == 1 && head.Equals(item))
                {
                    head = null;
                    --count;
                    return true;
                }
                else
                {
                    LinkedListNode<T> node = head;
                    do
                    {
                        if (node.Equals(item))
                        {

                        }
                    } while (node.next != head);
                    return false;
                }
            }

            public bool Contains(T item)
            {
                if (count == 0) return false;

                if (count == 1) return head.Equals(item);

                LinkedListNode<T> node = head.next;
                while (node != head)
                {
                    if (node.Equals(item))
                        return true;
                    node = node.next;
                }

                return node.Equals(item);
            }

            public void CopyTo(T[] array, Int32 arrayIndex)
            {

            }

            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            // IEnumerable<T>
            public IEnumerator<T> GetEnumerator()
            {
                LinkedListNode<T> node = head;
                do
                {
                    yield return node.item;
                    node = node.next;
                } while (node != head);
            }
            /*
            public IEnumerator<T> GetEnumerator()
            {
                return new LinkedListEnumerator<T>(this);
            }
            */
        }
        /*
        public sealed class LinkedListEnumerator<T> : IEnumerator<T>
        {
            private LinkedList<T> list;
            private LinkedListNode<T> current;

            public LinkedListEnumerator(LinkedList<T> llist)
            {
                list = llist;
                current = list.head;
            }

            // IEnumerator
            object IEnumerator.Current
            {
                get { return Current; }
            }

            // IEnumerator<T>
            public T Current
            {
                get { return current.item;  }
            }

            public bool MoveNext()
            {
                if (current == null)
                    return false;

                if (current.next == list.head)
                    return false;

                current = current.next;
                return true;
            }

            public void Reset()
            {
                current = list.head;
            }

            public void Dispose()
            { }
        }
        */
   
        // Graph
        public sealed class Graph<T>
        {
            private List<T> V;
            private List<Int32[]> E;

            public Graph()
            {
                V = new List<T>();
                E = new List<Int32[]>();
            }

            public void AddVertex(T vertex)
            {
                V.Add(vertex);
            }

            public void AddEdge(int v1, int v2)
            {
                E.Add(new Int32[2] { v1, v2 });
            }
        }
    }

    namespace V1
    {
        // Operations
        public interface IOperation
        {
            double Calculate();
        }

        class BinOp
        {
            protected IOperation operandA;
            protected IOperation operandB;

            protected BinOp(IOperation a, IOperation b)
            {
                operandA = a;
                operandB = b;
            }
        }

        sealed class BinOpAdd : BinOp, IOperation
        {
            internal BinOpAdd(IOperation a, IOperation b) : base(a, b) { }

            public double Calculate()
            {
                return operandA.Calculate() + operandB.Calculate();
            }
        }

        sealed class BinOpSub : BinOp, IOperation
        {
            internal BinOpSub(IOperation a, IOperation b) : base(a, b) { }

            public double Calculate()
            {
                return operandA.Calculate() - operandB.Calculate();
            }
        }

        sealed class BinOpMlt : BinOp, IOperation
        {
            internal BinOpMlt(IOperation a, IOperation b) : base(a, b) { }

            public double Calculate()
            {
                return operandA.Calculate() * operandB.Calculate();
            }
        }

        sealed class BinOpDiv : BinOp, IOperation
        {
            internal BinOpDiv(IOperation a, IOperation b) : base(a, b) { }

            public double Calculate()
            {
                return operandA.Calculate() / operandB.Calculate();
            }
        }

        sealed class BinOpPow : BinOp, IOperation
        {
            internal BinOpPow(IOperation a, IOperation b) : base(a, b) { }

            public double Calculate()
            {
                return Math.Pow(operandA.Calculate(), operandB.Calculate());
            }
        }

        sealed class UnOpNeg : IOperation
        {
            internal IOperation operand;

            internal UnOpNeg(IOperation a)
            {
                operand = a;
            }

            public double Calculate()
            {
                return -operand.Calculate();
            }
        }

        sealed class ValOp : IOperation
        {
            private readonly double value;

            public ValOp(double value)
            {
                this.value = value;
            }

            public double Calculate()
            {
                return value;
            }
        }
    }

    namespace V2
    {
        namespace Collections
        {
            public static class BinSymbolTable
            {
                static List<Tuple<string, BinOpEssence.OpEssence>> table;

                static BinSymbolTable()
                {
                    table = new List<Tuple<string, BinOpEssence.OpEssence>>();
                    table.Add(new Tuple<string, BinOpEssence.OpEssence>("pow", BinOpEssence.Pow));
                    table.Add(new Tuple<string, BinOpEssence.OpEssence>("log", BinOpEssence.Log));
                }

                public static BinOpEssence.OpEssence Find(string name)
                {
                    for (Int32 i = 0; i < table.Count; ++i)
                    {
                        if (string.Equals(name, table[i].Item1))
                            return table[i].Item2;
                    }
                    return null;
                }
            }

            public static class UnSymbolTable
            {
                static List<Tuple<string, UnOpEssence.OpEssence>> table;

                static UnSymbolTable()
                {
                    table = new List<Tuple<string, UnOpEssence.OpEssence>>();
                    table.Add(new Tuple<string, UnOpEssence.OpEssence>("abs", UnOpEssence.Abs));
                    table.Add(new Tuple<string, UnOpEssence.OpEssence>("sqr", UnOpEssence.Sqr));
                    table.Add(new Tuple<string, UnOpEssence.OpEssence>("sqrt", UnOpEssence.Sqrt));
                }

                public static UnOpEssence.OpEssence Find(string name)
                {
                    for (Int32 i = 0; i < table.Count; ++i)
                    {
                        if (string.Equals(name, table[i].Item1))
                            return table[i].Item2;
                    }
                    return null;
                }
            }
        }

        public interface IOperation
        {
            double Calculate();
        }

        // Binary
        public class BinOp : IOperation
        {
            BinOpEssence.OpEssence op;
            IOperation operandA, operandB;

            public BinOp(BinOpEssence.OpEssence op, IOperation a, IOperation b)
            {
                this.op = op;
                operandA = a;
                operandB = b;
            }

            // IOperation
            public double Calculate()
            {
                return op.Invoke(operandA, operandB);
            }
        }

        public static class BinOpEssence
        {
            public delegate double OpEssence(IOperation a, IOperation b);

            public static double Add(IOperation a, IOperation b)
            {
                return a.Calculate() + b.Calculate();
            }

            public static double Sub(IOperation a, IOperation b)
            {
                return a.Calculate() - b.Calculate();
            }

            public static double Mlt(IOperation a, IOperation b)
            {
                return a.Calculate() * b.Calculate();
            }

            public static double Div(IOperation a, IOperation b)
            {
                return a.Calculate() / b.Calculate();
            }

            public static double Pow(IOperation a, IOperation b)
            {
                return Math.Pow(a.Calculate(), b.Calculate());
            }

            public static double Log(IOperation a, IOperation b)
            {
                return Math.Log(a.Calculate()) / Math.Log(b.Calculate());
            }
        }

        // Unary
        public class UnOp : IOperation
        {
            UnOpEssence.OpEssence op;
            IOperation operand;

            public UnOp(UnOpEssence.OpEssence op, IOperation a)
            {
                this.op = op;
                operand = a;
            }

            public double Calculate()
            {
                return op.Invoke(operand);
            }
        }

        public static class UnOpEssence
        {
            public delegate double OpEssence(IOperation a);

            public static double Neg(IOperation a)
            {
                return -a.Calculate();
            }

            public static double Abs(IOperation a)
            {
                return Math.Abs(a.Calculate());
            }

            public static double Sqr(IOperation a)
            {
                return Math.Pow(a.Calculate(), 2);
            }

            public static double Sqrt(IOperation a)
            {
                return Math.Sqrt(a.Calculate());
            }

        }

        // Value
        public class ValOp : IOperation
        {
            double value;

            public ValOp(double value)
            {
                this.value = value;
            }

            public double Calculate()
            {
                return value;
            }
        }
    }

    namespace V3
    {
        namespace Collections
        {
            public static class BinSymbolTable
            {
                static List<Tuple<string, BinOpEssence.OpEssence>> table;

                static BinSymbolTable()
                {
                    table = new List<Tuple<string, BinOpEssence.OpEssence>>();
                    table.Add(new Tuple<string, BinOpEssence.OpEssence>("pow", BinOpEssence.Pow));
                    table.Add(new Tuple<string, BinOpEssence.OpEssence>("log", BinOpEssence.Log));
                }

                public static BinOpEssence.OpEssence Find(string name)
                {
                    for (Int32 i = 0; i < table.Count; ++i)
                    {
                        if (string.Equals(name, table[i].Item1))
                            return table[i].Item2;
                    }
                    return null;
                }
            }

            public static class UnSymbolTable
            {
                static List<Tuple<string, UnOpEssence.OpEssence>> table;

                static UnSymbolTable()
                {
                    table = new List<Tuple<string, UnOpEssence.OpEssence>>();
                    table.Add(new Tuple<string, UnOpEssence.OpEssence>("abs", UnOpEssence.Abs));
                    table.Add(new Tuple<string, UnOpEssence.OpEssence>("sqr", UnOpEssence.Sqr));
                    table.Add(new Tuple<string, UnOpEssence.OpEssence>("sqrt", UnOpEssence.Sqrt));
                }

                public static UnOpEssence.OpEssence Find(string name)
                {
                    for (Int32 i = 0; i < table.Count; ++i)
                    {
                        if (string.Equals(name, table[i].Item1))
                            return table[i].Item2;
                    }
                    return null;
                }
            }

            public static class DeclatationTable
            {
                internal static List<Tuple<string, IOperation>> table;

                static DeclatationTable()
                {
                    table = new List<Tuple<string, IOperation>>();
                    table.Add(new Tuple<string, IOperation>("PI", new ValOp(Math.PI)));
                }

                public static IOperation Find(string name)
                {
                    for (Int32 i = 0; i < table.Count; ++i)
                    {
                        if (string.Equals(name, table[i].Item1))
                            return table[i].Item2;
                    }
                    return null;
                }
            }
        }

        public interface IOperation
        {
            double Calculate();
        }

        // Binary
        public class BinOp : IOperation
        {
            BinOpEssence.OpEssence op;
            IOperation operandA, operandB;

            public BinOp(BinOpEssence.OpEssence op, IOperation a, IOperation b)
            {
                this.op = op;
                operandA = a;
                operandB = b;
            }

            // IOperation
            public double Calculate()
            {
                return op.Invoke(operandA, operandB);
            }
        }

        public static class BinOpEssence
        {
            public delegate double OpEssence(IOperation a, IOperation b);

            public static double Add(IOperation a, IOperation b)
            {
                return a.Calculate() + b.Calculate();
            }

            public static double Sub(IOperation a, IOperation b)
            {
                return a.Calculate() - b.Calculate();
            }

            public static double Mlt(IOperation a, IOperation b)
            {
                return a.Calculate() * b.Calculate();
            }

            public static double Div(IOperation a, IOperation b)
            {
                return a.Calculate() / b.Calculate();
            }

            public static double Pow(IOperation a, IOperation b)
            {
                return Math.Pow(a.Calculate(), b.Calculate());
            }

            public static double Log(IOperation a, IOperation b)
            {
                return Math.Log(a.Calculate()) / Math.Log(b.Calculate());
            }
        }

        // Unary
        public class UnOp : IOperation
        {
            UnOpEssence.OpEssence op;
            IOperation operand;

            public UnOp(UnOpEssence.OpEssence op, IOperation a)
            {
                this.op = op;
                operand = a;
            }

            public double Calculate()
            {
                return op.Invoke(operand);
            }
        }

        public static class UnOpEssence
        {
            public delegate double OpEssence(IOperation a);

            public static double Neg(IOperation a)
            {
                return -a.Calculate();
            }

            public static double Abs(IOperation a)
            {
                return Math.Abs(a.Calculate());
            }

            public static double Sqr(IOperation a)
            {
                double r = a.Calculate();
                return r * r;
            }

            public static double Sqrt(IOperation a)
            {
                return Math.Sqrt(a.Calculate());
            }

        }

        // Value
        public class ValOp : IOperation
        {
            double value;

            public ValOp(double value)
            {
                this.value = value;
            }

            public double Calculate()
            {
                return value;
            }
        }
    }
}