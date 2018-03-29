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
		int m = 20;
		// n is the total rows the player wants in the board
		int n = 20;
		// actual position in axe x 
		int axe_x = 0;
		// actual position in axe y 
		int axe_y = 0;
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

		private void check_position() {

		}

		private void initial_position(object sender, SpeechRecognizedEventArgs e) {
			Console.WriteLine("INITIAL POSITION");
			if (e.Result.Text == "arriba")
			{
				synthesizer.SpeakAsync("iniciando el juego");

			}
			else if (e.Result.Text == "abajo")
			{
				synthesizer.SpeakAsync("Cual seria el tamaño de cada cuadro, diga solo 1 número");
				recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(square_pixel_size_getter);
			}
			else if (e.Result.Text == "izquierda")
			{
				synthesizer.SpeakAsync("Cual seria el tamaño de cada cuadro, diga solo 1 número");
				recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(square_pixel_size_getter);
			}
			else if (e.Result.Text == "derecha")
			{
				synthesizer.SpeakAsync("Cual seria el tamaño de cada cuadro, diga solo 1 número");
				recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(square_pixel_size_getter);
			}
			else if (e.Result.Text == "listo")
			{
				synthesizer.SpeakAsync("Cual seria el tamaño de cada cuadro, diga solo 1 número");
				recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(square_pixel_size_getter);
			}
		}

		private void set_board() {
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
			
			Console.WriteLine("ya lo setie ): "); 

			
		}

		private void row_size_getter(object sender, SpeechRecognizedEventArgs e) {
			int number = from_number_str_to_int(e.Result.Text);
			if (number < 3 && number != -1)
			{
				synthesizer.SpeakAsync("El número debe ser mayor o igual a tres");
				recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(row_size_getter);
			}
			else if (number >= 3)
			{
				synthesizer.SpeakAsync("La cantidad de filas son:");
				synthesizer.SpeakAsync(number.ToString());
				n = number;
				set_board(); 
				synthesizer.SpeakAsync("Cuál es la ubicación que desea para iniciar el camino?");
                recognizer.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(row_size_getter);
                recognizer.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(column_size_getter);
                //recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(row_size_getter);
            }
		}

		private void column_size_getter(object sender, SpeechRecognizedEventArgs e) {
			int number = from_number_str_to_int(e.Result.Text);
			if (number < 3 && number != -1)
			{
				synthesizer.SpeakAsync("El número debe ser mayor o igual a tres");
				recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(column_size_getter);
			}
			else if (number >= 3)
			{
				synthesizer.SpeakAsync("La cantidad de columnas son:");
				synthesizer.SpeakAsync(number.ToString());
				m = number;
				synthesizer.SpeakAsync("Cuántas filas desea en el tablero?");
                recognizer.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(square_pixel_size_getter);
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(row_size_getter);
			}
		}

		private void square_pixel_size_getter(object sender, SpeechRecognizedEventArgs e) {
			/*string str = e.Result.Text; 
			string[] words = str.Split(new char[0]);*/
			int number = from_number_str_to_int(e.Result.Text);
			Console.WriteLine("square pixel"); 
			if (number < 20 && number != -1)
			{
				synthesizer.SpeakAsync("El número debe ser mayor o igual a 20");
				recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(square_pixel_size_getter);
			}
			else if (number >= 20) {
				synthesizer.SpeakAsync("El número del tamaño del cuadro es");
				synthesizer.SpeakAsync(number.ToString());
				a = number;
				synthesizer.SpeakAsync("Cuántas columnas desea en el tablero?");
                recognizer.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(start_decider);
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(column_size_getter);
			}
		}

		private void start_decider(object sender, SpeechRecognizedEventArgs e) {
			Console.WriteLine("START DECIDER");
			if (e.Result.Text == "correcto")
			{
				synthesizer.SpeakAsync("iniciando el juego");

			}
			else if (e.Result.Text == "no")
			{
				synthesizer.SpeakAsync("Cual seria el tamaño de cada cuadro, diga solo 1 número");
                recognizer.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(recognizer_speech_recognized);
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(square_pixel_size_getter);
			}
		}

		

		private void recognizer_speech_recognized(object sender, SpeechRecognizedEventArgs e)
		{
			Console.WriteLine("Quak");
            if (e.Result.Text == "iniciar")
			{
				synthesizer.SpeakAsync("Desea utilizar la configuración prestablecida para el juego, diga correcto o no. ]");
				recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(start_decider);
			}
			else if (e.Result.Text == "terminar") {
				this.Close(); 
			}

		}


		private void Form1_Load(object sender, EventArgs e)
		{
            /*****************PRUEBA DE A ESTRELLA, LUEGO SE QUITA**************************/

            List<List<Node>> labyrinth = new List<List<Node>>();

            List<Node> nodes = new List<Node>
            {
                new Node(0, 0, true),
                new Node(0, 1, true),
                new Node(0, 2, true),
                new Node(0, 3, true)
            };

            labyrinth.Add(nodes);
            nodes = new List<Node>
            {
                new Node(1, 0, true),
                new Node(1, 1, false),
                new Node(1, 2, false),
                new Node(1, 3, false)
            };
            labyrinth.Add(nodes);

            nodes = new List<Node>
            {
                new Node(2, 0, true),
                new Node(2, 1, true),
                new Node(2, 2, false),
                new Node(2, 3, true)
            };
            labyrinth.Add(nodes);

            A_star a_star = new A_star(labyrinth, true, 30);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Stack<Node> path = a_star.Find_path(2, 0, 0, 3);
            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Total Time: " + elapsedMs + " ms");
            int size_path = path.Count;
            for(int i = 0; i < size_path; i++)
            {
                Node n = path.Pop();
                Console.WriteLine("X: " + n.Position_X + ", Y: " + n.Position_Y);
            }
            /*********************TERMINA PRUEBA DE A ESTRELLA******************************/


            Console.WriteLine("input device recognised.......");
			recognizer.SetInputToDefaultAudioDevice(); //uses normal microfone
			synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0,CultureInfo.GetCultureInfo("es-ES"));
			Grammar grammar = new DictationGrammar();
			recognizer.LoadGrammar(grammar); // put all together
			recognizer.RecognizeAsync(RecognizeMode.Multiple);
			synthesizer.SpeakAsync("Bienvenido al juego, para jugar diga iniciar, o terminar para cerrar la aplicación");
			recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_speech_recognized);
			Console.WriteLine("123123");
			
		}

		private void board_Paint(object sender, PaintEventArgs e)
		{

		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			
		}
	}
}
/*
 SI SIRVE
 namespace Progra_IA_1
{
	public partial class Form1 : Form
	{
		SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
		public Form1()
		{
			InitializeComponent();
			
			
		}

		


		private void recognizer_speech_recognized(object sender, SpeechRecognizedEventArgs e)
		{

			Console.WriteLine("Quak");
			if (e.Result.Text == "iniciar") 
			{
				MessageBox.Show("Hello");
				label1.Text = "it works";
			}
			else{
				label1.Text = e.Result.Text; 
			}
		}


		

		private void button1_Click(object sender, EventArgs e)
		{
			recognizer.RecognizeAsync(RecognizeMode.Multiple);
			label1.Text = "Activo"; 
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Console.WriteLine("input device recognised.......");
			recognizer.SetInputToDefaultAudioDevice(); //uses normal microfone
			Grammar grammar = new DictationGrammar();
			recognizer.LoadGrammar(grammar); // put all together
			recognizer.RecognizeAsync(RecognizeMode.Multiple);
			recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_speech_recognized);
			Console.WriteLine("123123");
		}
	}
}

	 
	 */



/*

 ESTE NO SIRVE 
			Console.WriteLine("input device recognised.......");
		
Console.WriteLine("1");
			Choices commands = new Choices();
Console.WriteLine("2");
			commands.Add(new string[] { "Iniciar", "Terminar" });
			Console.WriteLine("3");
			GrammarBuilder grammarBuilder = new GrammarBuilder();
grammarBuilder.Append(commands);
			Console.WriteLine("4");
			Grammar grammar = new Grammar(grammarBuilder);
recognizer.LoadGrammarAsync(grammar); // put all together
			recognizer.SetInputToDefaultAudioDevice(); //uses normal microfone
			Console.WriteLine("1");
			recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_speech_recognized);
			Console.WriteLine("123123");



	 */


/*
 ESQUELETO ESQUELETIN QUE TENIA PENSADO PARA LO DE LA VOZ


Inicio
Syntetizer (si o no)
Supongo que tengo que hacer una funcion como la que se suma pero con si o no. Inicio_decider

Inicio_decider 
If e.response.text == "si"
	Valores() 
If iniciar 
  Inicio() 
If terminar
   Cerrar() 
If no 
   Game
Else 
	Nada? 

Pixeles () 
Sytetizer tamaño en pixeles
Ver si lo de la función decider se puede meter aqui
If es setval ==1
 Filas() 

Pixeles_decider()
Resp = E.respuesta
Separla en mini strings
Func sacar num
For i in tamaño de resp 
	  If Mini.i es num peq
			  Transformar a num
			   Break
	  Else if es num grand
			  Transformar a num
			   Break
	  Else 
			Di nop 
If resultado =! Null 
	   Syntetizer el num es tal 
Else 
	   Syntetizer no es un número valido


Filas() 
Syntetizer cuantas filas 
El += filas_decider
If setfilas =  1
	Cols()

Same para cols() 

Luego

Game () 
+= commands


 */
