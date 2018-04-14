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
		string[] numbers_str = {"cero", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve"};
		int[] number = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

		// board game info (: 
		// a is the size of each square
		int a;
		// m is the total columns the player wants in the board
		int m;
		// n is the total rows the player wants in the board
		int n;
		// initial point
		Node initial_point;
		// final point
		Node final_point; 
		//percent of obstacles 
		int perc_obst;
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

			panel1.VerticalScroll.Enabled = true;
			panel1.VerticalScroll.Visible = true; 
            panel1.HorizontalScroll.Enabled = true;
			panel1.HorizontalScroll.Visible = true;
        
        }

        private void restart_game_info()
        {
            a = 30;
		    m = 20;
		    n = 20;
		    perc_obst = 20;
		    flag_diag = false; 
		    tuple_list_obstacles = new List<Tuple<int, int>>();
		    logic_board = new List<List<Node>>();
        }   

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
        }

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
        }

		private int from_number_str_to_int(string str) {
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
					this.board.GetControlFromPosition(y, x).BackgroundImage = Progra_IA_1.Properties.Resources.Obstacle;
				}
				else {
					this.board.GetControlFromPosition(y, x).BackgroundImage = Progra_IA_1.Properties.Resources.Three;
				}
				if (its_obstacle(new_x, new_y))
				{
                    //I need to change this, go to the next valid position
					this.board.GetControlFromPosition(new_y, new_x).BackgroundImage = Progra_IA_1.Properties.Resources.Obstacle;
				}
				else
				{
					this.board.GetControlFromPosition(new_y, new_x).BackgroundImage = Progra_IA_1.Properties.Resources.Three;
				}
				return answer; 
			}
			else
			{
				synthesizer.SpeakAsync("La posición se sale del tablero, por lo que la posición actual es la anterior");
				return logic_board.ElementAt(x).ElementAt(y); 
			}
		}

        private void clean_board()
        {
            board.AutoScroll = true;
			board.AutoSize = true;
            board.Visible = false;
            board.RowStyles.Clear();
			board.ColumnStyles.Clear();
			board.Controls.Clear();
        }

        private void clean_path()
        {
            for (int x = 0; x < n; x++)
			{
                for (int y = 0; y < m; y++)
                    {
                        if (this.board.GetControlFromPosition(y, x).BackgroundImage == Progra_IA_1.Properties.Resources.Bone)
                        {
                           this.board.GetControlFromPosition(y, x).BackgroundImage = Progra_IA_1.Properties.Resources.Three;
                        }
                    }
				
			}
        }
		

		private void set_visual_board() {
			this.board.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			board.RowStyles.Clear();
			board.ColumnStyles.Clear();
			board.AutoScroll = true;
			board.AutoSize = true;
			board.Visible = false;
			board.Controls.Clear();
			board.SuspendLayout();
			for (int i = 0; i < m; i++) {
				Console.WriteLine(i);
				board.ColumnCount++;
				this.board.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, a));
				

			}
			for (int i = 0; i < n; i++)
			{
				Console.WriteLine(i);
				board.RowCount++;
				this.board.RowStyles.Add(new RowStyle(SizeType.Absolute, a));
				
			}
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					this.board.Controls.Add(new Panel { Dock = DockStyle.Fill }, j, i);
				}	
			}
			board.ResumeLayout();
			board.Visible = true;
			board.AutoSize = true;
			this.board.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			Console.WriteLine("ya lo setie ):");
		}

		

		private void recognizer_speech_recognized(object sender, SpeechRecognizedEventArgs e)
		{
			float confidence = e.Result.Confidence;
			if (confidence < 0.50)
			{
				Console.WriteLine("Low confidence");
			}
			else if (r_init == 1)
			{
				Console.WriteLine("r_init");
				if (e.Result.Text == "Iniciar")
				{
                    clean_board();
					synthesizer.SpeakAsync("Laika pregunta si deseas utilizar la configuración prestablecida para el juego, di la palabra si o no.");
					r_init = 0;
					r_pre = 1;
				}
				else if (e.Result.Text == "Terminar")
				{
					this.Close();
				}
			}
			else if (r_pre == 1)
			{
				Console.WriteLine("r_pre");
				if (e.Result.Text == "Si")
				{
					//synthesizer.SpeakAsync("Iniciando el juego");
					r_pre = 0;
					r_sta = 1;
                    restart_game_info();
                    Console.WriteLine("game info");
					initialize_board_logic();
					synthesizer.SpeakAsync("Laika quiere que muevas el punto de inicio de la partida, si lo deseas ahí dí la palabra, listo");
					r_row = 0;
					r_sta = 1;
					Console.WriteLine("TUPI");
					Console.WriteLine("r_sta");
					initial_point = search_initial_valid_position();
					this.board.GetControlFromPosition(initial_point.Position_Y, initial_point.Position_X).BackColor = Color.Green;

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
					a = number;
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
					Console.WriteLine("TUPI");
					Console.WriteLine("r_sta");
					initial_point = search_initial_valid_position();
					this.board.GetControlFromPosition(initial_point.Position_Y, initial_point.Position_X).BackgroundImage = Progra_IA_1.Properties.Resources.Laika_Dog1;

				}
			}
			else if (r_sta == 1) {
                if ((e.Result.Text == "Arriba") || (e.Result.Text == "Abajo") || (e.Result.Text == "Izquierda") || (e.Result.Text == "Derecha"))
				{
					initial_point = check_position(initial_point.Position_X, initial_point.Position_Y, e.Result.Text); 
				}
				else if (e.Result.Text == "Listo")
				{
					if (its_obstacle(initial_point.Position_X, initial_point.Position_Y)) {
						synthesizer.SpeakAsync("Laika está enojada, el punto inicial no es un lugar válido, cambia la posición con los comandos dados anteriormente."); 
					}
					else {
						synthesizer.SpeakAsync("Laika quiere que muevas el punto final de la partida, si lo deseas ahí dí la palabra, listo");
						r_sta = 0;
						r_end = 1; 
						final_point = search_final_valid_position();
						this.board.GetControlFromPosition(final_point.Position_Y, final_point.Position_X).BackgroundImage = Progra_IA_1.Properties.Resources.Bone;
					}
				}
			}
			else if (r_end == 1) {
				
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
                }
       
            }
                
		}

		private void run_a() {
			A_star a_star = new A_star(logic_board, flag_diag, a);
			var watch = System.Diagnostics.Stopwatch.StartNew();
			Stack<Node> path = a_star.Find_path(initial_point.Position_X, initial_point.Position_Y, final_point.Position_X, final_point.Position_Y);
			watch.Stop();
			long elapsedMs = watch.ElapsedMilliseconds;
			Console.WriteLine("Total Time: " + elapsedMs + " ms");
			int size_path;
			try
			{
				size_path = path.Count-1;
			}
			catch (Exception ex)
			{
				size_path = 0;
				Console.WriteLine("No hay solucion");
				synthesizer.SpeakAsync("No hay solucion");
			}
			for (int i = 0; i < size_path; i++)
			{
				Node n = path.Pop();
				Console.WriteLine("X: " + n.Position_X + ", Y: " + n.Position_Y);
				this.board.GetControlFromPosition(n.Position_Y, n.Position_X).BackgroundImage = Progra_IA_1.Properties.Resources.Bone;
			}
		}

		private bool assign_obstacle() {
			int number = rand.Next(100); // creates a number between 0 and 100
			if (number <= perc_obst)
				return true;
			else
				return false;
		}

		private bool its_obstacle(int x, int y) {
			bool traversable = logic_board.ElementAt(x).ElementAt(y).Traversable;
			if (traversable == false) // traversable means you can pass through, so if it is false means it is an obstacle 
			{
				return true;
			}
			else {
				return false; 
			}
		} 

		private void initialize_board_logic() {
			
			logic_board = new List<List<Node>>();
			for (int i = 0; i < n; i++) {
				List<Node> row = new List<Node>(); 
				for (int j = 0; j < m; j++) {
					if (assign_obstacle() == true)
					{
						row.Add(new Node(i, j, false));
						tuple_list_obstacles.Add(Tuple.Create(i,j));
					}
					else {
						row.Add(new Node(i, j, true));
					}
					Console.WriteLine("done"); 
				}
				logic_board.Add(row); 
				
			}
			set_visual_board();
			for (int i = 0; i < tuple_list_obstacles.Count; i++)
			{
				Tuple<int, int> tuple = tuple_list_obstacles.ElementAt(i);
				this.board.GetControlFromPosition(tuple.Item2, tuple.Item1).BackgroundImage = Progra_IA_1.Properties.Resources.Obstacle;	
			}

		}


		private Node search_initial_valid_position () 
        {
			int x_position;
            int y_position;
            Node eval;
            while (true)
            {
                x_position = rand.Next(n);
                y_position = rand.Next(m);
                eval = logic_board[x_position][y_position];
				if (eval.Traversable == true) {
					return eval;
				}
            }
            
		}

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
			commands.Add(new string[] {"Iniciar", "Terminar", "Si", "No", "Limpiar", "Arriba", "Abajo", "Izquierda", "Derecha", "Listo", "Cambiar"});
			GrammarBuilder gBuilder = new GrammarBuilder();
			gBuilder.Culture = new System.Globalization.CultureInfo("es-ES");
			gBuilder.Append(commands);
			Grammar grammar = new Grammar(gBuilder);
			recognizer.SetInputToDefaultAudioDevice(); //uses normal microfone
			synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0,CultureInfo.GetCultureInfo("es-ES"));
			recognizer.LoadGrammarAsync(grammar); // put all together
			recognizer.RecognizeAsync(RecognizeMode.Multiple);
			synthesizer.SpeakAsync("Bienvenido al juego, para jugar diga iniciar, o terminar para cerrar la aplicación");
			restart_flags();
			recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_speech_recognized);
		}

		private void board_Paint(object sender, PaintEventArgs e)
		{

		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			
		}

	}
}
