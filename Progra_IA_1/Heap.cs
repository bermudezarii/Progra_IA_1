using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progra_IA_1
{
    class Heap<T> where T : IHeapElement<T>
    {
        /* Nodes to be stored */
        T[] elements;

        /* Total  */
        public int Count
        {
            get;
            set;
        }

        public Heap(int size_heap)
        {
            elements = new T[size_heap];
        }

        public void Add(T element)
        {
            element.Heap_index = Count;
            elements[Count] = element;

        }

        public T Remove_first()
        {
            T first_element = elements[0];
            Count--;
            elements[0] = elements[Count];
            elements[0].Heap_index = 0;
            Sort_down(elements[0]);
            return first_element;
        }
        /****************** First try, ignore it******************/
        public void Update_element(T element)
        {
            Sort_up(element);
        }
        /*********************************************************/
        public bool Contains(T element)
        {
            return Equals(elements[element.Heap_index], element);
        }

        private void Sort_up(T element)
        {
            int parent_index = (element.Heap_index - 1) / 2;

            while (true)
            {
                T parent_element = elements[parent_index];
                if(element.CompareTo(parent_element) > 0)
                {
                    Swap(element, parent_element);
                }
                else
                {
                    break;
                }

                parent_index = (element.Heap_index - 1) / 2;
            }
        }

        private void Sort_down(T element)
        {
            while(true)
            {
                int child_index_left = element.Heap_index * 2 + 1;
                int child_index_right = element.Heap_index * 2 + 2;

                int swap_index = 0;

                if(child_index_left < Count)
                {
                    swap_index = child_index_left;
                    if(child_index_right < Count)
                    {
                        if(elements[child_index_left].CompareTo(elements[child_index_right]) < 0)
                        {
                            swap_index = child_index_right;
                        }
                    }

                    if(element.CompareTo(elements[swap_index]) < 0)
                    {
                        Swap(element, elements[swap_index]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

               
            }
        }

        private void Swap(T e1, T e2)
        {
            elements[e1.Heap_index] = e2;
            elements[e2.Heap_index] = e1;

            int e1_index = e1.Heap_index;
            e1.Heap_index = e2.Heap_index;
            e2.Heap_index = e1_index;
        }
    }

    public interface IHeapElement<T> : IComparable<T>
    {
        int Heap_index
        {
            get;
            set;
        }
    }
}
