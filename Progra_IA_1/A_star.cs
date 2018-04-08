using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progra_IA_1
{
    class A_star
    {
        public List<List<Node>> board { get; set; }
        public bool diagonal_flag { get; set; }
        public double square_size { get; set; }
        public double hypotenuse
        {
            get
            {
                double side = Math.Pow(square_size, 2);
                return Math.Sqrt(2 * side);
            }
        }

        public int board_rows
        {
            get
            {
                return board.Count;
            }
        }

        public int board_columns
        {
            get
            {
                return board[0].Count;
            }
        }
        public int max_size
        {
            get
            {
                return board_rows * board_columns;
            }
        }
        public A_star(List<List<Node>> board, bool diagonal_flag, double square_size)
        {
            this.board = board;
            this.diagonal_flag = diagonal_flag;
            this.square_size = square_size;
        }

        /********************************************************
         *Function that return the optimal Path from start node *
         *to end node.                                          *
         *Parameters:                                           *
         *      start_X: X coordinate of the initial node       *
         *      start_Y: Y coordinate of the initial node       *
         *      end_X: X coordinate of the final node           *
         *      end_Y: Y coordinate of the final node           *
         ********************************************************/
        public Stack<Node> Find_path(int start_X, int start_Y, int end_X, int end_Y)
        {
            /* Create initial and final node */
            Node start = new Node(start_X, start_Y, true);
            Node end = new Node(end_X, end_Y, true);

            /* Final path from initial to final node */
            Stack<Node> path = new Stack<Node>();

            /* open_list: Nodes to be evaluate */
            Heap<Node> open_list = new Heap<Node>(max_size);

            /* close_list: Nodes already evaluate */
            List<Node> close_list = new List<Node>();

            /* adjacencies: Neighbors nodes of current node */
            List<Node> adjacencies;
            Node current = start;

            /* Add initial node to open_list */
            open_list.Add(current);

            /* Travel every node from open_list */
            while (open_list.Count != 0 && !close_list.Exists(x => ((x.Position_X == end.Position_X) && (x.Position_Y == end.Position_Y))))
            {
                /* Assign current node and remove it from open_list */
                current = open_list.Remove_first();

                /* Add element in close_list and get neighbor nodes */
                close_list.Add(current);
                adjacencies = Get_adjacent_nodes(current);

                foreach(Node n in adjacencies)
                {
                    /* Case node is obstacle or is not stored in close_list */
                    if(!close_list.Contains(n) && n.Traversable)
                    {
                        /* Case node is not stored in open_list*/
                        if(!open_list.Contains(n))
                        {
                            n.Parent = current;

                            /* Get heuristic value */

                            /* Case algorithm include diagonal*/
                            if (diagonal_flag)
                            {
                                n.H_cost = Distance_diagonal(n.Position_X, n.Position_Y, end.Position_X, end.Position_Y);
                                n.G_cost += hypotenuse + n.Parent.G_cost;
                            }

                            else
                            {
                                n.H_cost = Distance_manhattan(n.Position_X, n.Position_Y, end.Position_X, end.Position_Y);
                                n.G_cost += square_size + n.Parent.G_cost;
                            }
                            
                            open_list.Add(n);

                            /* Order open_list by f(n) = g(n) + h(n) */
                            //open_list = open_list.OrderBy(node => node.F_cost).ToList();
                        }
                    }
                }
            }

            /* Verify if final node is stored in close_list */
            if(!close_list.Exists(x => ((x.Position_X == end.Position_X) && (x.Position_Y == end.Position_Y))))
            {
                return null;
            }

            /* Get the optimal path */
            Node temp = close_list[close_list.IndexOf(current)];
            while(temp != start && temp != null)
            {
                path.Push(temp);
                temp = temp.Parent;
            }
            return path;
        }


        /****************************************************
         *Function that return the neighbors of a node      *
         *Parameters:                                       *
         *      node (Node): The node to use to obtain his  *
         *      neighbors.                                  *
         ****************************************************/
        private List<Node> Get_adjacent_nodes(Node node)
        {
            List<Node> temp = new List<Node>();

            int row = (int)node.Position_X;
            int column = (int)node.Position_Y;

            /* Check no-diagonal neighbors */
            if(row + 1 < board_rows)
            {
                temp.Add(board[row + 1][column]);
            }
            if(row - 1 >= 0)
            {
                temp.Add(board[row - 1][column]);
            }
            if(column + 1 < board_columns)
            {
                temp.Add(board[row][column + 1]);
            }
            if(column - 1 >= 0)
            {
                temp.Add(board[row][column - 1]);
            }

            /* Check diagonal neighbors */
            if(diagonal_flag)
            {
                if(row + 1 < board_rows && column + 1 < board_columns)
                {
                    temp.Add(board[row + 1][column + 1]);
                }
                if(row + 1 < board_rows && column - 1 >= 0)
                {
                    temp.Add(board[row + 1][column - 1]);
                }
                if(row - 1 >= 0 && column + 1 < board_columns)
                {
                    temp.Add(board[row - 1][column + 1]);
                }
                if(row - 1 >= 0 && column - 1 >= 0)
                {
                    temp.Add(board[row - 1][column - 1]);
                }
            }
            return temp;
        }


        /****************************************************************
         * Function that return the result of the Manhattan Distance    *
         * Parameters:                                                  *
         *      start_X: X coordinate of the current node               *
         *      start_Y: Y coordinate of the current node               *
         *      end_X: X coordinate of the final node                   *
         *      end_Y: Y coordinate of the final node                   *
         ****************************************************************/
        private double Distance_manhattan(int start_X, int start_Y, int end_X, int end_Y)
        {
            /* Formula: d = |x1 - x2| + |y1 - y2| */
            return Math.Abs(start_X - end_X) + Math.Abs(start_Y - end_Y);
        }

        /****************************************************************
         * Function that return the result of the Diagonal Distance     *
         * Parameters:                                                  *
         *      start_X: X coordinate of the current node               *
         *      start_Y: Y coordinate of the current node               *
         *      end_X: X coordinate of the final node                   *
         *      end_Y: Y coordinate of the final node                   *
         ****************************************************************/
        private double Distance_diagonal(int start_X, int start_Y, int end_X, int end_Y)
        {
            /* Formula: ((x2 - x1) ^ 2 + (y2 - y1) ^ 2) ^ (1/2) */
            double max_distance = Math.Max(Math.Abs(start_X - end_X), Math.Abs(start_Y - end_Y));
            double min_distance = Math.Min(Math.Abs(start_X - end_X), Math.Abs(start_Y - end_Y));

            return hypotenuse * min_distance + square_size * (max_distance - min_distance);
        }
    }
}
