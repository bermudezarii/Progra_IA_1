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
		int a = 40;
		// m is the total columns the player wants in the board
		int m = 2;
		// n is the total rows the player wants in the board
		int n = 2;
		// initial point
		Node initial_point;
		// final point
		Node final_point; 
		//percent of obstacles 
		int perc_obst = 50;
		// flag if they want diagonals to work
		bool flag_diag = false; 
		// points in matrix where there is an obstacle 
		List<Tuple<int, int>> tuple_list_obstacles = new List<Tuple<int, int>>();
		// logic board 
		List<List<Node>> logic_board = new List<List<Node>>();
		// tuple that indicates the tablelayout_paint what to do
		Tuple<int, int, int> instruction_tuple; 

		//flags for speech recognition, when they are = 1, they recognize the words related to that part
		int r_init = 0;
		int r_pre = 0; // prestablished parameters
		int r_col = 0;
		int r_row = 0;
		int r_squ = 0; // means square
		int r_sta = 0; // means start
		int r_end = 0; 
		int r_dia = 0; // means diagonals 
		int r_rea = 0; // means ready
		public Form1()
		{
			InitializeComponent();
			panel1.VerticalScroll.Enabled = true;
			panel1.VerticalScroll.Visible = true; 
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
					new_y = new_y - 1; break;
				case "Abajo":
					new_y = new_y + 1; break;
				case "Izquierda":
					new_x = new_x - 1; break;
				case "Derecha":
					new_x = new_x + 1; break;
			}
			
			Console.WriteLine("new point " + new_x.ToString() + " " + new_y.ToString() + " move: " + move);
			Node answer;
			if (new_x < n && new_y < m && new_x > -1 && new_y > -1) // ponerle los ceros 
			{
				answer = logic_board[new_x][new_y];
				/*if (its_obstacle(new_x, new_y))
				{
					Console.WriteLine("Entra a its obstacle de lo nuevo");
					instruction_tuple = Tuple.Create(x, y, 0);
					this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);
					this.board.Invalidate();
					

				}
				else
				{
					Console.WriteLine("Entra de lo nuevo");
					instruction_tuple = Tuple.Create(x, y, 1);
					Console.WriteLine("1");
					this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);
					Console.WriteLine("le vale verga 2");
					this.board.Invalidate();
					

				}*/
				if (its_obstacle(x, y))
				{
					instruction_tuple = Tuple.Create(x, y, 4);
					this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);
					this.board.Invalidate();
				}
				else {
					Console.WriteLine("me no entender"); 
					instruction_tuple = Tuple.Create(x, y, 3);
					this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);
					this.board.Invalidate();
				}
				if (its_obstacle(new_x, new_y))
				{
					Console.WriteLine("Entra a its obstacle de lo nuevo");
					instruction_tuple = Tuple.Create(new_x, new_y, 0);
					this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);
					this.board.Invalidate();


				}
				else
				{
					Console.WriteLine("Entra de lo nuevo");
					instruction_tuple = Tuple.Create(new_x, new_y, 1);
					Console.WriteLine("1");
					this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);
					Console.WriteLine("le vale verga 2");
					this.board.Invalidate();


				}
				return answer; 
			}
			else
			{
				synthesizer.SpeakAsync("La posición se sale del tablero, por lo que la posición actual es la anterior");
				return logic_board.ElementAt(x).ElementAt(y); 
			}
		}
		

		private void set_visual_board() {
			this.board.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			board.RowStyles.Clear();
			board.ColumnStyles.Clear();
			board.AutoScroll = true;
			board.AutoSize = true;
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
			this.board.ResumeLayout();
			board.AutoSize = true;
			this.board.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			Console.WriteLine("ya lo setie ):");
		}

		

		private void recognizer_speech_recognized(object sender, SpeechRecognizedEventArgs e)
		{
			float confidence = e.Result.Confidence;
			if (confidence < 0.10)
			{
				Console.WriteLine("Low confidence");
			}
			else if (r_init == 1)
			{
				Console.WriteLine("r_init");
				if (e.Result.Text == "Iniciar")
				{
					synthesizer.SpeakAsync("Desea utilizar la configuración prestablecida para el juego, diga si o no.");
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
					synthesizer.SpeakAsync("Iniciando el juego");
					r_pre = 0;
					r_sta = 1;
					initialize_board_logic();
					synthesizer.SpeakAsync("Mueva el punto de inicio de la partida, si lo desea ahí diga, listo");
					r_row = 0;
					r_sta = 1;
					Console.WriteLine("TUPI");
					Console.WriteLine("r_sta");
					initial_point = search_initial_valid_position();
					instruction_tuple = Tuple.Create(initial_point.Position_X, initial_point.Position_Y, 1);
					this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);

				}
				else if (e.Result.Text == "No")
				{
					synthesizer.SpeakAsync("Cual seria el tamaño de cada cuadro, diga solo 1 número");
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
					synthesizer.SpeakAsync("El número debe ser mayor o igual a 20");
				}
				else if (number >= 20)
				{
					synthesizer.SpeakAsync("El número del tamaño del cuadro es");
					synthesizer.SpeakAsync(number.ToString());
					a = number;
					synthesizer.SpeakAsync("Cuántas columnas desea en el tablero?");
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
					synthesizer.SpeakAsync("El número debe ser mayor o igual a tres");

				}
				else if (number >= 3)
				{
					synthesizer.SpeakAsync("La cantidad de columnas son:");
					synthesizer.SpeakAsync(number.ToString());
					m = number;
					synthesizer.SpeakAsync("Cuántas filas desea en el tablero?");
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
					synthesizer.SpeakAsync("El número debe ser mayor o igual a tres");

				}
				else if (number >= 3)
				{
					synthesizer.SpeakAsync("La cantidad de filas son:");
					synthesizer.SpeakAsync(number.ToString());
					n = number;
					initialize_board_logic();
					synthesizer.SpeakAsync("Mueva el punto de inicio de la partida, si lo desea ahí diga, listo");
					r_row = 0;
					r_sta = 1;
					Console.WriteLine("TUPI");
					Console.WriteLine("r_sta");
					initial_point = search_initial_valid_position();
					instruction_tuple = Tuple.Create(initial_point.Position_X, initial_point.Position_Y, 1);
					this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);

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
						synthesizer.SpeakAsync("El punto inicial no es un lugar válido, cambie la posición con los comandos dados anteriormente."); 
					}
					else {
						synthesizer.SpeakAsync("Mueva el punto final de la partida, si lo desea ahí diga, listo");
						r_sta = 0;
						r_end = 1; 
					}
				}
			}
			else if (r_end == 1) {
				final_point = search_final_valid_position();
				instruction_tuple = Tuple.Create(final_point.Position_X, final_point.Position_Y, 1);
				this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);
				Console.WriteLine("r_end");
				if ((e.Result.Text == "Arriba") || (e.Result.Text == "Abajo") || (e.Result.Text == "Izquierda") || (e.Result.Text == "Derecha"))
				{
					final_point = check_position(final_point.Position_X, final_point.Position_Y, e.Result.Text);
				}
				else if (e.Result.Text == "Listo")
				{
					if (its_obstacle(final_point.Position_X, final_point.Position_Y))
					{
						synthesizer.SpeakAsync("El punto final no es un lugar válido, cambie la posición con los comandos dados anteriormente.");
					}
					else
					{
						synthesizer.SpeakAsync("Finalmente, desea que se utilicen diagonales en la respuesta del juego? Responda si o no.");
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
					synthesizer.SpeakAsync("Las diagonales son permitidas, iniciando el juego");
					r_dia = 0;
					r_rea = 1;
				}

				else if (e.Result.Text == "No")
				{
					flag_diag = false;
					synthesizer.SpeakAsync("Las diagonales no son permitidas, iniciando el juego");
					r_dia = 0;
					r_rea = 1;
				}
			}
			else if (r_rea == 1)
			{
				A_star a_star = new A_star(logic_board, false, a);
				var watch = System.Diagnostics.Stopwatch.StartNew();
				Stack<Node> path = a_star.Find_path(initial_point.Position_X, initial_point.Position_Y, final_point.Position_X, final_point.Position_Y);
				watch.Stop();
				long elapsedMs = watch.ElapsedMilliseconds;
				Console.WriteLine("Total Time: " + elapsedMs + " ms");
				int size_path;
				try
				{
					size_path = path.Count;
				}
				catch (Exception ex)
				{
					size_path = 0;
					Console.WriteLine("No hay solucion");
				}
				for (int i = 0; i < size_path; i++)
				{
					Node n = path.Pop();
					Console.WriteLine("X: " + n.Position_X + ", Y: " + n.Position_Y);
					instruction_tuple = Tuple.Create(n.Position_X, n.Position_Y, 2);
					this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_color);
				}
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
			this.board.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.board_CellPaint_obstacles);

		}


		private Node search_initial_valid_position () {
			
			Console.WriteLine("quak");
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					Node eval = logic_board[i][j];
					if (eval.Traversable == true) {
						return eval;
					}
				}
			}
			return null; 
		}

		private Node search_final_valid_position()
		{
			for (int i = n-1; i >= 0; i--)
			{
				for (int j = m-1; j >= 0; j--)
				{
					Node eval = logic_board.ElementAt(i).ElementAt(j);
					if (eval.Traversable == true)
					{
						return eval;
					}
				}
			}
			return null;
		}


		private void Form1_Load(object sender, EventArgs e)
		{
            /*****************PRUEBA DE A ESTRELLA, LUEGO SE QUITA**************************/
			/*
            List<List<Node>> labyrinth = new List<List<Node>>();

            List<Node> nodes = new List<Node>
            {
                new Node(0, 0, true),
                new Node(0, 1, true),
                new Node(0, 2, false)
           
            };

            labyrinth.Add(nodes);
            nodes = new List<Node>
            {
                new Node(1, 0, true),
                new Node(1, 1, false),
                new Node(1, 2, true)
            };
            labyrinth.Add(nodes);

            nodes = new List<Node>
            {
                new Node(2, 0, false),
                new Node(2, 1, true),
                new Node(2, 2, true)
         
            };
            labyrinth.Add(nodes);

            A_star a_star = new A_star(labyrinth, false, 30);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Stack<Node> path = a_star.Find_path(0, 0, 2, 2);
            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Total Time: " + elapsedMs + " ms");
			int size_path; 
			try
			{
				size_path = path.Count;
			}
			catch (Exception ex) {
				size_path = 0;
				Console.WriteLine("No hay solucion");
			}
            for(int i = 0; i < size_path; i++)
            {
                Node n = path.Pop();
                Console.WriteLine("X: " + n.Position_X + ", Y: " + n.Position_Y);
            }*/
			/*********************TERMINA PRUEBA DE A ESTRELLA******************************/

			Choices commands = new Choices();
			commands.Add(new string[] {"Iniciar", "Terminar", "Si", "No", "Limpiar", "Arriba", "Abajo", "Izquierda", "Derecha", "Listo"});
			GrammarBuilder gBuilder = new GrammarBuilder();
			gBuilder.Culture = new System.Globalization.CultureInfo("es-ES");
			gBuilder.Append(commands);
			Grammar grammar = new Grammar(gBuilder);

			Console.WriteLine("input device recognised.......");
			recognizer.SetInputToDefaultAudioDevice(); //uses normal microfone
			synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0,CultureInfo.GetCultureInfo("es-ES"));
			
			recognizer.LoadGrammarAsync(grammar); // put all together
			recognizer.RecognizeAsync(RecognizeMode.Multiple);
			synthesizer.SpeakAsync("Bienvenido al juego, para jugar diga iniciar, o terminar para cerrar la aplicación");
			r_init = 1;
			

			recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_speech_recognized);
			Console.WriteLine("123123");
			
		}

		private void board_Paint(object sender, PaintEventArgs e)
		{

		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			
		}

		private void board_CellPaint_1(object sender, TableLayoutCellPaintEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
		}

		private void board_CellPaint_obstacles(object sender, TableLayoutCellPaintEventArgs e) {
			for (int i = 0; i < tuple_list_obstacles.Count; i++) {
				Tuple<int, int> tuple = tuple_list_obstacles.ElementAt(i);
				if (e.Row == tuple.Item1 && e.Column == tuple.Item2) {
					e.Graphics.FillRectangle(Brushes.Black, e.CellBounds);
				}

			}
		}


		private void board_CellPaint_color(object sender, TableLayoutCellPaintEventArgs e)
		{
			//Console.WriteLine("ok"); 
			if (e.Row == instruction_tuple.Item1 && e.Column == instruction_tuple.Item2) {
				Console.WriteLine("Entra para pintar.");
				if (instruction_tuple.Item3 == 0) {
					e.Graphics.FillRectangle(Brushes.Red, e.CellBounds);
					Console.WriteLine("Rojo.");
				}
				else if (instruction_tuple.Item3 == 1) {
					e.Graphics.FillRectangle(Brushes.Green, e.CellBounds);
					Console.WriteLine("Verde.");
				}
				else if (instruction_tuple.Item3 == 2) {
					e.Graphics.FillRectangle(Brushes.BlueViolet, e.CellBounds);
					Console.WriteLine("Violeta.");
				}
				else if (instruction_tuple.Item3 == 3) {
					e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
					Console.WriteLine("Blanco.");
				}
				else if (instruction_tuple.Item3 == 4) {
					e.Graphics.FillRectangle(Brushes.Black, e.CellBounds);
					Console.WriteLine("Negro");
				}

			}
			
		}


	}
}
