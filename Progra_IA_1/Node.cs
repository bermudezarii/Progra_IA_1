using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progra_IA_1
{
    class Node
    {
        private bool is_accessible;
        private int world_position_X;
        private int world_position_Y;

        public Node(bool is_accessible, int world_position_X, int world_position_Y)
        {
            this.is_accessible = is_accessible;
            this.world_position_X = world_position_X;
            this.world_position_Y = world_position_Y;
        }

        public bool Is_accessible
        {
            get
            {
                return is_accessible;
            }
            set
            {
                is_accessible = value;
            }
        }

        public int World_position_X
        {
            get
            {
                return world_position_X;
            }
            set
            {
                world_position_X = value;
            }
        }

        public int World_position_Y
        {
            get
            {
                return world_position_Y;
            }
            set
            {
                world_position_Y = value;
            }
        }

    }
}
