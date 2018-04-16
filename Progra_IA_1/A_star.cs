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
            HashSet<Node> close_list = new HashSet<Node>();

            /* adjacencies: Neighbors nodes of current node */
            List<Node> adjacencies;

            /* Add initial node to open_list */
            Node current;
            open_list.Add(start);
            int round = 1;

            /* If not solution found, return null */
            while(open_list.Count > 0)
            {
                
                current = open_list.Remove_first();

                close_list.Add(current);
                Console.WriteLine("Iteration " + round + ": Node to evaluate = (" + current.Position_X + ", " + current.Position_Y + ")");

                /* Verify if current is the final position */
                if (current.Position_X == end.Position_X && current.Position_Y == end.Position_Y)
                {
                    Node temp = current;
                    while (temp != start && temp != null)
                    {
                        path.Push(temp);
                        temp = temp.Parent;
                    }
                    return path;
                }

                
                adjacencies = Get_adjacent_nodes(current);

                foreach(Node neighbor in adjacencies)
                {
                    /* Verify if neighbor isn't an object or if it is inside close_list*/
                    if(!neighbor.Traversable || close_list.Contains(neighbor))
                    {
                        continue;
                    }

                    if (!open_list.Contains(neighbor))
                    {
                        neighbor.Parent = current;

                        /* If includes diagonals*/
                        if(diagonal_flag)
                        {
                            neighbor.H_cost = Distance_diagonal(neighbor.Position_X, neighbor.Position_Y, end.Position_X, end.Position_Y);

                            if(neighbor.Parent.Position_X != neighbor.Position_X && neighbor.Position_Y != neighbor.Position_Y)
                            {
                                neighbor.G_cost = hypotenuse + neighbor.Parent.G_cost;
                            }
                            else
                            {
                                neighbor.G_cost = square_size + neighbor.Parent.G_cost;
                            }
                        }
                        else
                        {
                            neighbor.H_cost = Distance_manhattan(neighbor.Position_X, neighbor.Position_Y, end.Position_X, end.Position_Y);
                            neighbor.G_cost = square_size + neighbor.Parent.G_cost;
                        }
                        open_list.Add(neighbor);
                        Console.WriteLine("(" + neighbor.Position_X + ", " + neighbor.Position_Y + "): " + neighbor.F_cost);
                    }
                    

                }
                round++;
            }
            return null;            
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
            /*double max_distance = Math.Max(Math.Abs(start_X - end_X), Math.Abs(start_Y - end_Y));
            double min_distance = Math.Min(Math.Abs(start_X - end_X), Math.Abs(start_Y - end_Y));

            return hypotenuse * min_distance + square_size * (max_distance - min_distance);*/

            double dx = Math.Abs(start_X - end_X);
            double dy = Math.Abs(start_Y - end_Y);

            return square_size * (dx + dy) + (hypotenuse - 2 * square_size) * Math.Min(dx, dy);
        }
    }
}
