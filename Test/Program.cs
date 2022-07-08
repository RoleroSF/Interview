using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test
{
    class Program
    {
        public class ListNode
        {
            public ListNode Previous;
            public ListNode Next;
            public ListNode Random;
            public string Data;
        }


        class ListRandom : IEnumerable<ListNode>
        {
            public ListNode Head;
            public ListNode Tail;
            public int Count;
            Random r  = new Random();

            public void Add(ListNode node)
            {
                ListNode cnode = new ListNode() { Data = node.Data };
                if (Head == null)
                    Head = cnode;
                else
                {
                    Tail.Next = cnode;
                    cnode.Previous = Tail;
                }
                Tail = cnode;
                Count++;
                cnode.Random = this.ToArray().GetValue(r.Next(this.Count)) as ListNode;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)this).GetEnumerator();
            }

            IEnumerator<ListNode> IEnumerable<ListNode>.GetEnumerator()
            {
                ListNode current = Head;
                while (current != null)
                {
                    yield return current;
                    current = current.Next;
                }
            }

            public void Serialize(Stream s)
            {
                using (StreamWriter w = new StreamWriter(s))
                    foreach (ListNode n in this)
                        w.WriteLine(n.Data.ToString() + ":" + this.ToList().IndexOf(n.Random).ToString());
            }

            public void Deserialize(Stream s)
            {
                List<ListNode> collection = new List<ListNode>();
                ListNode temp = new ListNode();

                string line;
                Count = 0;
                try
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!line.Equals(""))
                            {
                                Count++;
                                temp.Data = line;
                                ListNode next = new ListNode();
                                temp.Next = next;
                                collection.Add(temp);
                                next.Previous = temp;
                                temp = next;
                            }
                        }    
                    }
                    Tail = temp.Previous;
                    Tail.Next = null;

                    collection.ForEach(i => i.Random = collection[Convert.ToInt32(i.Data.Split(':')[1])]);
                    collection.ForEach(i => i.Data = i.Data.Split(':')[0]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Не удалось обработать файл данных");
                    Console.WriteLine(ex.Message);
                    Console.Read();
                }
            }
        }

        static void Main(string[] args)
        {
            ListRandom linkedList = new ListRandom();

            linkedList.Add(new ListNode() { Data = "Значение 1" });
            linkedList.Add(new ListNode() { Data = "Значение 2" });
            linkedList.Add(new ListNode() { Data = "Значение 3" });
            linkedList.Add(new ListNode() { Data = "Значение 4" });
            linkedList.Add(new ListNode() { Data = "Значение 5" });

            foreach (ListNode item in linkedList)
            {
                string i = item.Data;
                string n = item.Next == null ? "null" : item.Next.Data;
                string p = item.Previous == null ? "null" : item.Previous.Data;
                Console.WriteLine(p + "\r\n" + i + "\r\n" + n + "\r\n");
                Console.ReadKey();
            }
            Console.ReadKey();

            //serialize
            FileStream fs = new FileStream(@"X:\\Test\fileDat.dat", FileMode.OpenOrCreate);
            linkedList.Serialize(fs);

            //deserialize
            ListRandom secondLinkedList = new ListRandom();
            try
            {
                fs = new FileStream("dat.dat", FileMode.Open);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
            secondLinkedList.Deserialize(fs);
            if (secondLinkedList.Tail.Data == linkedList.Tail.Data) Console.WriteLine("Success");
            Console.Read();
        }
    }
}