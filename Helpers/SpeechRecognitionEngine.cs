using Microsoft.CognitiveServices.Speech;


namespace Helpers
{
    internal class SpeechRecognition
    {
        private string _apikey;
        private string _region;
        private SpeechRecognizer _recognizer;

        public SpeechRecognition(string apikey, string region)
        {
            _apikey = apikey;
            _region = region;
            _recognizer = new SpeechRecognizer(SpeechConfig.FromSubscription(apikey, region));  
        }

        async public Task StartSpeechRecognition()
        {

            await _recognizer.StartContinuousRecognitionAsync();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            await _recognizer.StopContinuousRecognitionAsync();
        }

        public SpeechRecognizer GetSpeechRecognizer()
        {
            return _recognizer;
        }

        //public async Task Run()
        //{
        //    var config = SpeechConfig.FromSubscription(_apikey, _region);

        //    using var recognizer = new SpeechRecognizer(config);

        //    recognizer.Recognizing += (s, e) =>
        //    {
        //        Console.WriteLine($"Recognizing: {e.Result.Text}");
        //    };

        //    recognizer.Recognized += (s, e) =>
        //    {
        //        if (e.Result.Reason == ResultReason.RecognizedSpeech)
        //        {
        //            Console.WriteLine($"Recognized: {e.Result.Text}");
        //        }
        //        else if (e.Result.Reason == ResultReason.NoMatch)
        //        {
        //            Console.WriteLine("No speech could be recognized.");
        //        }
        //    };

        //    recognizer.Canceled += (s, e) =>
        //    {
        //        Console.WriteLine($"Canceled: Reason={e.Reason}");
        //        if (e.Reason == CancellationReason.Error)
        //        {
        //            Console.WriteLine($"Error details: {e.ErrorDetails}");
        //        }
        //    };

        //    recognizer.SessionStopped += (s, e) =>
        //    {
        //        Console.WriteLine("Session stopped.");
        //    };

        //    Console.WriteLine("Speak into your microphone.");
        //    await recognizer.StartContinuousRecognitionAsync();

        //    Console.WriteLine("Press any key to stop...");
        //    Console.ReadKey();

        //    await recognizer.StopContinuousRecognitionAsync();
        //}
    }
}

