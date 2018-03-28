using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progra_IA_1
{
    class Board
    {
        /* Total of rows and columns in board */
        private int rows;
        private int columns;

        /* Size in x and y of board */
        private int grid_size_X;
        private int grid_size_Y;

        /* Size of each square */
        private int square_size;
        private Node[,] board;

        public Board(int [,] board, int rows, int columns, int square_size)
        {

            /* TODO: The size of each square (in GUI) is the same, so we need to use
             * a specific number instead to use square_size */
            this.rows = rows;
            this.columns = columns;
            this.square_size = square_size;
            grid_size_X = rows * square_size;
            grid_size_Y = columns * square_size;

            create_board(board);
        }

        /****************************************************************
         *Function that create the board for the application            *
         * Parameters:                                                  *
         *      board(int []): Board with numbers 0 and 1, represents   *
         *      the objects and walkable places in board                *
         * Output:                                                      *
         *      Does not apply                                          *
         *                                                              *
         ****************************************************************/
        public void create_board(int [,] board)
        {
            bool accessible;
            for(int i = 0; i < grid_size_X; i++)
            {
                for(int j = 0; j < grid_size_Y; j++)
                {
                    if (board[i,j] == 1)
                    {
                        accessible = false;
                    }
                    else
                    {
                        accessible = true;
                    }
                }
            }
        }
    }
}
