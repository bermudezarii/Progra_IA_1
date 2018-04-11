using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progra_IA_1
{
    class Node : IHeapElement<Node>
    {
        public Node Parent { get; set; }

        public int Position_X { get; set; }
        public int Position_Y { get; set; }

        public double H_cost { get; set; }

        public double G_cost { get; set; }
        public double F_cost
        {
            get
            {
                if (H_cost != -1 && G_cost != -1)
                    return H_cost + G_cost;
                else
                    return -1;
            }
        }

        public bool Traversable { get; set; }

        


        public Node(int position_X, int position_Y, bool traversable)
        {
            Parent = null;
            Position_X = position_X;
            Position_Y = position_Y;
            H_cost = -1;
            G_cost = 0;
            Traversable = traversable;
        }

        public int Heap_index
        {
            get
            {
                return Heap_index;
            }
            set
            {
                Heap_index = value;
            }
        }

        public int CompareTo(Node node)
        {
            int compare = F_cost.CompareTo(node.F_cost);
            if(compare == 0)
            {
                compare = H_cost.CompareTo(node.H_cost);
            }
            return -compare;
        }
    }
}
