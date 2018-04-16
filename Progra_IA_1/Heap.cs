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

        /*************************************************
         * Function that add an element in the heap and  *
         * sort it.                                      *
         * Parameters:                                   *
         *      element: Object to include in the heap   *
         *************************************************/
        public void Add(T element)
        {
            element.Heap_index = Count;
            elements[Count] = element;
            Sort_up(element);
            Count++;

        }


        /*************************************************
         * Function that remove an element of the heap   *
         * and sort it.                                  *
         *************************************************/
        public T Remove_first()
        {
            T first_element = elements[0];
            Count--;
            elements[0] = elements[Count];
            elements[0].Heap_index = 0;
            Sort_down(elements[0]);
            return first_element;
        }


        /*************************************************
         * Function that verify if element is contained  *
         * in heap                                       *
         * Parameters:                                   *
         *      element: Object to verify in the heap    *
         *************************************************/
        public bool Contains(T element)
        {
            return Equals(elements[element.Heap_index], element);
        }


        /*************************************************
         * Function that sort the heap(Up).              *
         * Parameters:                                   *
         *      element: Object to verify in the heap    *
         *************************************************/
        private void Sort_up(T element)
        {
            int parent_index = (element.Heap_index - 1) / 2;

            while (true)
            {
                T parent_element = elements[parent_index];

                /* Verify if child is lower than parent */
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


        /*************************************************
         * Function that sort the heap(Down).            *
         * Parameters:                                   *
         *      element: Object to verify in the heap    *
         *************************************************/
        private void Sort_down(T element)
        {
            while(true)
            {
                int child_index_left = element.Heap_index * 2 + 1;
                int child_index_right = element.Heap_index * 2 + 2;

                int swap_index = 0;

                /* Verify if left child exist */
                if(child_index_left < Count)
                {
                    swap_index = child_index_left;

                    /* Verify if right child exist */
                    if(child_index_right < Count)
                    {

                        /* Get the child with the lowest value */
                        if(elements[child_index_left].CompareTo(elements[child_index_right]) < 0)
                        {
                            swap_index = child_index_right;
                        }
                    }
                    /* Verify if child is lower than parent*/
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


        /*************************************************
         * Function that swap position of two elements.  *
         * Parameters:                                   *
         *      e1:First element                         *
         *      e2:Second element                        *
         *************************************************/
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
