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
        public A_star(List<List<Node>> board)
        {
            this.board = board;
        }

        public Stack<Node> Find_path(int start_X, int start_Y, int end_X, int end_Y)
        {
            Node start = new Node(start_X, start_Y, true);
            Node end = new Node(end_X, end_Y, true);

            Stack<Node> path = new Stack<Node>();
            List<Node> open_list = new List<Node>();
            List<Node> close_list = new List<Node>();
            List<Node> adjacencies;
            Node current = start;

            //Add start node in open list
            open_list.Add(current);

            while (open_list.Count != 0 && !close_list.Exists(x => ((x.Position_X == end.Position_X) && (x.Position_Y == end.Position_Y))))
            {
                current = open_list[0];
                open_list.Remove(current);

                close_list.Add(current);
                adjacencies = Get_adjacent_nodes(current);

                foreach(Node n in adjacencies)
                {
                    if(!close_list.Contains(n) && n.Traversable)
                    {
                        if(!open_list.Contains(n))
                        {
                            n.Parent = current;
                            n.Distance_target = Distance_manhattan(n.Position_X, n.Position_Y, end.Position_X, end.Position_Y);
                            n.Real_cost = 1 + n.Parent.Real_cost;
                            open_list.Add(n);
                            open_list = open_list.OrderBy(node => node.F_cost).ToList<Node>();
                        }
                    }
                }
            }

            if(!close_list.Exists(x => ((x.Position_X == end.Position_X) && (x.Position_Y == end.Position_Y))))
            {
                return null;
            }

            Node temp = close_list[close_list.IndexOf(current)];
            while(temp != start && temp != null)
            {
                //Console.WriteLine("X: " + temp.Position_X + ", Y: " + temp.Position_Y);
                path.Push(temp);
                temp = temp.Parent;
                
            }
            return path;
        }

        private List<Node> Get_adjacent_nodes(Node node)
        {
            List<Node> temp = new List<Node>();

            int row = (int)node.Position_X;
            int column = (int)node.Position_Y;

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
            return temp;
        }

        private float Distance_manhattan(int start_X, int start_Y, int end_X, int end_Y)
        {
            return Math.Abs(start_X - end_X) + Math.Abs(start_Y - end_Y);
        }
    }
}
