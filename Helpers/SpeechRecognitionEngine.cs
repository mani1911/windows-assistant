using System.Speech.Recognition;
namespace SpeechRecognitionApp
{
    class SpeechRecognition
    {
        static bool shouldExit = false;
        static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Recognized text: " + e.Result.Text);
        }
        public static void Run()
        {

            foreach (var device in SpeechRecognitionEngine.InstalledRecognizers())
                Console.WriteLine("Installed recognizer: " + device.Culture);

            SpeechRecognitionEngine recognizer =
              new SpeechRecognitionEngine(
                new System.Globalization.CultureInfo("en-US"));

            recognizer.LoadGrammar(new DictationGrammar()); // Open-ended speech

            recognizer.SetInputToDefaultAudioDevice(); // Use system mic

            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            // Keep app alive while recognizing
            while (!shouldExit)
            {
                Console.ReadLine();
            }
        }
    }
}
