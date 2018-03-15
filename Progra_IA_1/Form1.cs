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
namespace Progra_IA_1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			speech_init();
		}

		


		private void recognizer_speech_recognized(object sender, SpeechRecognizedEventArgs e)
		{
			if (e.Result.Text == "test") 
			{
				MessageBox.Show("Hello");
				label1.Text = "it works";
			}
			else{
				label1.Text = e.Result.Text; 
			}
		}


		private void speech_init() {
			Console.WriteLine("input device recognised.......");
			/*SpeechSynthesizer synthesizer = new SpeechSynthesizer();
			PromptBuilder builder = new PromptBuilder();*/
			SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
			Console.WriteLine("1");
			Choices commands = new Choices();
			Console.WriteLine("2");
			commands.Add(new string[] { "start", "stop" });
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
		}

	
		
	}
}
