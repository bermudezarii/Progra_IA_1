using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progra_IA_1
{
    class Node
    {
        public static int Node_size = 30;
        public Node Parent { get; set; }

        public int Position_X { get; set; }
        public int Position_Y { get; set; }

        public int Center_X
        {
            get
            {
                return Position_X + Node_size / 2;
            }
        }

        public int Center_Y
        {
            get
            {
                return Position_Y + Node_size / 2;
            }
        }

        public float Distance_target { get; set; }

        public float Real_cost { get; set; }
        public float F_cost
        {
            get
            {
                if (Distance_target != -1 && Real_cost != -1)
                    return Distance_target + Real_cost;
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
            Distance_target = -1;
            Real_cost = 1;
            Traversable = traversable;
        }
    }
}
