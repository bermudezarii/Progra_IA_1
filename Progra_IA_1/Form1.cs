using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Globalization;

namespace Progra_IA_1
{
    public partial class Form1 : Form
    {
        //random generator
        Random rand = new Random();

        // all related to speak and recognize voice
        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        // arrays related to the function that transforms number words into integers
        string[] numbers_str = { "cero", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve" };
        int[] number = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

        // board game info (: 
        // a is the size of each square at the visual board
        int a;
        //a_ is the size of each square at the logical board, necessary for A*
        int a_;
        // m is the total columns the player wants in the board
        int m;
        // n is the total rows the player wants in the board
        int n;
        // initial point
        Node initial_point;
        // final point
        Node final_point;
        //percent of obstacles in the board
        int perc_obst = 20;
        // flag if they want diagonals to work
        bool flag_diag;
        // points in matrix where there is an obstacle 
        List<Tuple<int, int>> tuple_list_obstacles = new List<Tuple<int, int>>();
        // logic board 
        List<List<Node>> logic_board = new List<List<Node>>();


        //flags for speech recognition, when they are = 1, they recognize the words related to that part
        int r_init;
        int r_pre; // prestablished parameters
        int r_col;
        int r_row;
        int r_squ; // means square
        int r_sta; // means start
        int r_end;
        int r_dia; // means diagonals 
        int r_rea; // means ready
        int r_cha; // means change
        int r_clean; // means clean actual path



        public Form1()
        {
            InitializeComponent();

            this.Text = "El juego de Laika";
            this.BackColor = Color.White;

            //this.Icon = new Icon("Resources/Laika_Dog.ico");

            panel1.VerticalScroll.Enabled = true;
            panel1.VerticalScroll.Visible = true;
            panel1.HorizontalScroll.Enabled = true;
            panel1.HorizontalScroll.Visible = true;

        }

        /********************************************************
         *Function that restart the game information to the     *
         *prestablished parameters
         ********************************************************/
        private void restart_game_info()
        {
            a = 60;
            m = 10;
            n = 10;
            a_ = 60;
            perc_obst = 20;
            flag_diag = false;
            tuple_list_obstacles = new List<Tuple<int, int>>();
            logic_board = new List<List<Node>>();
        }
        /********************************************************
         *Function that restart the flags to start the game again
        ********************************************************/
        private void restart_flags()
        {
            r_init = 1;
            r_pre = 0;
            r_col = 0;
            r_row = 0;
            r_squ = 0;
            r_sta = 0;
            r_end = 0;
            r_dia = 0;
            r_rea = 0;
            r_cha = 0;
            r_clean = 0;

            a = 60;

            show_git();

        }

        /********************************************************
        *Function that show the initial image at the game, the 
        * Laika Gif Animation
        ********************************************************/
        private void show_git()
        {
            this.pictureBox1.Visible = true;
            this.pictureBox1.Image = Progra_IA_1.Properties.Resources.Laika_Animation3;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        /********************************************************
       *Function that restart the flags to change the initial
       * position at the game
       ********************************************************/
        private void activate_flag_positions()
        {
            r_init = 0;
            r_pre = 0;
            r_col = 0;
            r_row = 0;
            r_squ = 0;
            r_sta = 1;
            r_end = 0;
            r_dia = 0;
            r_rea = 0;
            r_cha = 0;
            r_clean = 1;
        }

        /********************************************************
       *Function that convert a string number in a integer
       ********************************************************/
        private int from_number_str_to_int(string str)
        {
            Console.WriteLine("str_int");
            int x;
            try
            {
                x = Int32.Parse(str);
            }
            catch
            {
                for (int i = 0; i < 20; i++)
                {
                    if (str.Equals(numbers_str[i]))
                    {
                        return number[i];
                    }
                }
                x = -1;
            }
            return x;
        }

        /********************************************************
           *Function that resize the image to show it at the board
           ********************************************************/
        private Image resize_image(Image image)
        {
            Size new_size = new Size((int)(a - a * 0.10), (int)(a - a * 0.10));
            Image new_image = (Image)new Bitmap(image, new_size);
            return new_image;
        }

        /********************************************************
         *Function that check if the new position is posssible
         * Parameters:
         * x: row at the board
         * y: colum at the board
         * move: type of move, can be Up, Down, Left or Right
         ********************************************************/
        private Node check_position(int x, int y, string move)
        {
            int new_x = x;
            int new_y = y;
            switch (move)
            {
                case "Arriba":
                    new_x = new_x - 1; break;
                case "Abajo":
                    new_x = new_x + 1; break;
                case "Izquierda":
                    new_y = new_y - 1; break;
                case "Derecha":
                    new_y = new_y + 1; break;
            }

            Console.WriteLine("new point " + new_x.ToString() + " " + new_y.ToString() + " move: " + move);
            Node answer;

            if (new_x < n && new_y < m && new_x > -1 && new_y > -1) // ponerle los ceros 
            {
                answer = logic_board[new_x][new_y];
                if (its_obstacle(x, y))
                {
                    this.board.GetControlFromPosition(y, x).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.caquita);
                }
                else
                {
                    this.board.GetControlFromPosition(y, x).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.paw);
                }

                if (its_obstacle(new_x, new_y))
                {
                    //I need to change this, go to the next valid position
                    this.board.GetControlFromPosition(new_y, new_x).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.caquita);
                }
                else
                {
                    if (r_sta == 1)
                    {
                        this.board.GetControlFromPosition(new_y, new_x).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.laika);
                    }
                    else
                    {
                        this.board.GetControlFromPosition(new_y, new_x).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.house);
                    }

                }
                return answer;
            }
            else
            {
                synthesizer.SpeakAsync("Laika esta enojada, la posición se sale del tablero, por lo que la posición actual es la anterior");
                return logic_board.ElementAt(x).ElementAt(y);
            }
        }

        /********************************************************
         *Function that clean the screen and erases the actual 
         *board
         ********************************************************/
        private void clean_board()
        {
            board.AutoScroll = true;
            board.AutoSize = true;
            board.Visible = false;
            board.RowStyles.Clear();
            board.ColumnStyles.Clear();
            board.Controls.Clear();
        }

        /********************************************************
        *Function that clean the actual path given for the A*
        ********************************************************/
        private void clean_path()
        {
            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < m; y++)
                {
                    if (logic_board.ElementAt(x).ElementAt(y).IsPath == true)
                    {

                        logic_board.ElementAt(x).ElementAt(y).IsPath = false;
                        this.board.GetControlFromPosition(y, x).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.paw);

                    }
                }

            }

            synthesizer.SpeakAsync("Laika quiere que muevas el punto de inicio de la partida, si lo deseas ahí dí la palabra, listo");
        }

        /********************************************************
        *Function that create the board with the parameters given
        * by the user and puts the obstacles
        ********************************************************/
        private void set_visual_board()
        {

            this.pictureBox1.Visible = false;
            this.board.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            board.RowStyles.Clear();
            board.ColumnStyles.Clear();
            board.AutoScroll = true;
            board.AutoSize = true;
            board.Visible = false;
            board.Controls.Clear();
            board.SuspendLayout();
            for (int i = 0; i < m; i++)
            {
                board.ColumnCount++;
                this.board.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, a));


            }
            for (int i = 0; i < n; i++)
            {
                board.RowCount++;
                this.board.RowStyles.Add(new RowStyle(SizeType.Absolute, a));

            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    this.board.Controls.Add(new Panel { Dock = DockStyle.Fill }, j, i);
                    this.board.GetControlFromPosition(j, i).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.paw);
                }
            }
            board.ResumeLayout();
            board.AutoSize = true;
            board.Visible = true;
            this.Size = new Size(n * a, m * a);
        }


        /********************************************************
        *Function that receive the parameters given by the user 
        ********************************************************/
        private void recognizer_speech_recognized(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            Console.WriteLine("Laika entendió: " + e.Result.Text);
            if (confidence < 0.50)
            {
                Console.WriteLine("Low confidence");
            }
            else if (r_init == 1)
            {
                this.Size = new Size(500, 500);
                Console.WriteLine("r_init");
                if (e.Result.Text == "Iniciar")
                {

                    synthesizer.SpeakAsync("Laika pregunta si deseas utilizar la configuración prestablecida para el juego, di la palabra si o no.");
                    r_init = 0;
                    r_pre = 1;
                }
                else if (e.Result.Text == "Terminar")
                {
                    synthesizer.SpeakAsync("Laika te va a echar de menos, vuelve pronto");
                    this.Close();
                }
            }
            else if (r_pre == 1)
            {
                Console.WriteLine("r_pre");
                if (e.Result.Text == "Si")
                {

                    r_pre = 0;
                    r_sta = 1;
                    restart_game_info();
                    initialize_board_logic();
                    synthesizer.SpeakAsync("Laika quiere que muevas el punto de inicio de la partida, si lo deseas ahí dí la palabra, listo");
                    r_row = 0;
                    r_sta = 1;
                    Console.WriteLine("r_sta");
                    initial_point = search_initial_valid_position();

                    this.board.GetControlFromPosition(initial_point.Position_Y, initial_point.Position_X).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.laika);



                }
                else if (e.Result.Text == "No")
                {
                    synthesizer.SpeakAsync("Laika quiere saber el tamaño de cada cuadro, dí solo 1 número");
                    recognizer.LoadGrammarAsync(new DictationGrammar()); // put all together
                    r_pre = 0;
                    r_squ = 1;
                }
            }
            else if (r_squ == 1)
            {
                Console.WriteLine("r_squ");
                int number = from_number_str_to_int(e.Result.Text);
                if (number < 20 && number != -1)
                {
                    synthesizer.SpeakAsync("Laika está enojada, no puedes elegir números menores a 20");
                }
                else if (number >= 20)
                {
                    synthesizer.SpeakAsync("Laika dice que el número del tamaño del cuadro es");
                    synthesizer.SpeakAsync(number.ToString());
                    a_ = number;
                    synthesizer.SpeakAsync("Laika quiere saber cuántas columnas deseas en el tablero?");
                    r_squ = 0;
                    r_col = 1;
                }
            }
            else if (r_col == 1)
            {
                Console.WriteLine("r_col");
                int number = from_number_str_to_int(e.Result.Text);
                if (number < 3 && number != -1)
                {
                    synthesizer.SpeakAsync("Laika está enojada, no puedes elegir números menores a tres");

                }
                else if (number >= 3)
                {
                    synthesizer.SpeakAsync("Laika dice que la cantidad de columnas es:");
                    synthesizer.SpeakAsync(number.ToString());
                    m = number;
                    synthesizer.SpeakAsync("Laika quiere saber cuántas filas deseas en el tablero?");
                    r_col = 0;
                    r_row = 1;
                }
            }
            else if (r_row == 1)
            {
                Console.WriteLine("r_row");
                int number = from_number_str_to_int(e.Result.Text);
                if (number < 3 && number != -1)
                {
                    synthesizer.SpeakAsync("Laika está enojada, no puedes elegir números menores a tres");

                }
                else if (number >= 3)
                {
                    synthesizer.SpeakAsync("Laika dice que la cantidad de filas son:");
                    synthesizer.SpeakAsync(number.ToString());
                    n = number;
                    initialize_board_logic();
                    synthesizer.SpeakAsync("Laika quiere que muevas el punto de inicio de la partida, si lo deseas ahí dí la palabra, listo");
                    r_row = 0;
                    r_sta = 1;
                    Console.WriteLine("r_sta");
                    initial_point = search_initial_valid_position();

                    this.board.GetControlFromPosition(initial_point.Position_Y, initial_point.Position_X).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.laika);

                }
            }
            else if (r_sta == 1)
            {
                if ((e.Result.Text == "Arriba") || (e.Result.Text == "Abajo") || (e.Result.Text == "Izquierda") || (e.Result.Text == "Derecha"))
                {
                    initial_point = check_position(initial_point.Position_X, initial_point.Position_Y, e.Result.Text);
                }
                else if (e.Result.Text == "Listo")
                {
                    if (its_obstacle(initial_point.Position_X, initial_point.Position_Y))
                    {
                        synthesizer.SpeakAsync("Laika está enojada, el punto inicial no es un lugar válido, cambia la posición con los comandos dados anteriormente.");
                    }
                    else
                    {
                        synthesizer.SpeakAsync("Laika quiere que muevas el punto final de la partida, si lo deseas ahí dí la palabra, listo");
                        r_sta = 0;
                        r_end = 1;
                        if (r_clean == 0)
                        {
                            final_point = search_final_valid_position();
                        }
                        this.board.GetControlFromPosition(final_point.Position_Y, final_point.Position_X).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.house);
                    }
                }
            }
            else if (r_end == 1)
            {

                Console.WriteLine("r_end");
                if ((e.Result.Text == "Arriba") || (e.Result.Text == "Abajo") || (e.Result.Text == "Izquierda") || (e.Result.Text == "Derecha"))
                {
                    final_point = check_position(final_point.Position_X, final_point.Position_Y, e.Result.Text);
                }
                else if (e.Result.Text == "Listo")
                {
                    if (its_obstacle(final_point.Position_X, final_point.Position_Y))
                    {
                        synthesizer.SpeakAsync("Laika está enojada, el punto final no es un lugar válido, cambia la posición con los comandos dados anteriormente.");
                    }
                    else
                    {
                        synthesizer.SpeakAsync("Laika quiere saber si deseas que se utilicen diagonales en la respuesta del juego? Responde si o no.");
                        r_end = 0;
                        r_dia = 1;
                    }
                }
            }
            else if (r_dia == 1)
            {
                if (e.Result.Text == "Si")
                {
                    flag_diag = true;
                    synthesizer.SpeakAsync("Laika dice que las diagonales son permitidas");
                }

                else if (e.Result.Text == "No")
                {
                    flag_diag = false;
                    synthesizer.SpeakAsync("Laika dice que las diagonales no son permitidas");
                }
                r_dia = 0;
                r_cha = 1;
                run_a();
                synthesizer.SpeakAsync("Laika quiere saber si deseas cambiar las posiciones o jugar de nuevo, di cambiar o limpiar");
            }
            else if (r_cha == 1)
            {

                if (e.Result.Text == "Cambiar")
                {
                    clean_path();
                    activate_flag_positions();
                }
                else if (e.Result.Text == "Limpiar")
                {

                    restart_flags();
                    restart_game_info();
                    clean_board();
                    synthesizer.SpeakAsync("Laika quiere que digas Iniciar para jugar de nuevo o Terminar para salir del juego");
                }

            }

        }

        /********************************************************
        *Function that search the optimal path with the A*
        * algorithm
        ********************************************************/
        private void run_a()
        {
            A_star a_star = new A_star(logic_board, flag_diag, a_);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Stack<Node> path = a_star.Find_path(initial_point.Position_X, initial_point.Position_Y, final_point.Position_X, final_point.Position_Y);
            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Total Time: " + elapsedMs + " ms");
            int size_path;
            try
            {
                size_path = path.Count;
                for (int i = 0; i < size_path - 1; i++)
                {

                    Node n = path.Pop();
                    this.board.GetControlFromPosition(n.Position_Y, n.Position_X).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.bones);
                    logic_board.ElementAt(n.Position_X).ElementAt(n.Position_Y).IsPath = true;
                }
                path.Pop();
            }
            catch (Exception ex)
            {
                size_path = 0;
                Console.WriteLine("No hay solucion");
                synthesizer.SpeakAsync("No hay solucion");
            }


        }

        private bool assign_obstacle()
        {
            int number = rand.Next(100); // creates a number between 0 and 100
            if (number <= perc_obst)
                return true;
            else
                return false;
        }

        /********************************************************
        *Function that verify if the position is an obstacle
        * Parameters:
        * x: row
        * y: column
        ********************************************************/
        private bool its_obstacle(int x, int y)
        {
            bool traversable = logic_board.ElementAt(x).ElementAt(y).Traversable;
            if (traversable == false) // traversable means you can pass through, so if it is false means it is an obstacle 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /********************************************************
        *Function that initialize the logical board to manage the
        * game
        ********************************************************/
        private void initialize_board_logic()
        {

            logic_board = new List<List<Node>>();
            for (int i = 0; i < n; i++)
            {
                List<Node> row = new List<Node>();
                for (int j = 0; j < m; j++)
                {
                    if (assign_obstacle() == true)
                    {
                        row.Add(new Node(i, j, false));
                        tuple_list_obstacles.Add(Tuple.Create(i, j));
                    }
                    else
                    {
                        row.Add(new Node(i, j, true));
                    }
                }
                logic_board.Add(row);

            }
            set_visual_board();
            for (int i = 0; i < tuple_list_obstacles.Count; i++)
            {
                Tuple<int, int> tuple = tuple_list_obstacles.ElementAt(i);
                this.board.GetControlFromPosition(tuple.Item2, tuple.Item1).BackgroundImage = resize_image(Progra_IA_1.Properties.Resources.caquita);

            }

        }

        /********************************************************
        *Function that search the initial position for Laika
        ********************************************************/
        private Node search_initial_valid_position()
        {
            int x_position;
            int y_position;
            Node eval;
            while (true)
            {
                x_position = rand.Next(n);
                y_position = rand.Next(m);
                eval = logic_board[x_position][y_position];
                if (eval.Traversable == true)
                {
                    return eval;
                }
            }

        }

        /********************************************************
        *Function that search the final position for Laika's house
        ********************************************************/
        private Node search_final_valid_position()
        {
            int x_position;
            int y_position;
            Node eval;
            while (true)
            {
                x_position = rand.Next(n);
                y_position = rand.Next(m);
                eval = logic_board.ElementAt(x_position).ElementAt(y_position);
                if (eval.Traversable == true && eval != initial_point) //the final position and the initial position can't be the same
                {
                    return eval;
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Choices commands = new Choices();
            commands.Add(new string[] { "Iniciar", "Terminar", "Si", "No", "Limpiar", "Arriba", "Abajo", "Izquierda", "Derecha", "Listo", "Cambiar" });
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Culture = new System.Globalization.CultureInfo("es-ES");
            gBuilder.Append(commands);
            Grammar grammar = new Grammar(gBuilder);
            recognizer.SetInputToDefaultAudioDevice(); //uses normal microfone
            synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Child, 0, CultureInfo.GetCultureInfo("es-ES"));
            recognizer.LoadGrammarAsync(grammar); // put all together
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            synthesizer.SpeakAsync("Bienvenido a mi juego, yo soy Laika, estoy perdida, ayudame a volver a mi casa, me comunico a traves de mi traductor canino debajo de mi pañoleta, te dejo con mi traductor, Laika quiere que para empezar diga Iniciar, para jugar, o Terminar para salir del juego");
            restart_flags();
            this.Size = new Size(500, 500);
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_speech_recognized);
        }

        private void board_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
