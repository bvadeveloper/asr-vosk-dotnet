using Vosk;

namespace ConsoleAppSpeechRecognition;

public static class Program
{
    // wav files:
    private const string InputWavFileNameShort = "/data/test_256kbps_16000hz_mono.wav";
    private const string InputWavFileNameLong = "/data/output_audio_256kbps_16khz_mono.wav";

    // model dir:
    private const string ModelPath = "/data/model_full";

    public static async Task Main()
    {
        // You can set to -1 to disable logging messages
        Vosk.Vosk.SetLogLevel(0);

        using var model = new Model(ModelPath);
        var cts = new CancellationTokenSource();

        var outputText = VoskSpeechRecognizer.Recognize(model, InputWavFileNameShort, cts.Token);
        Console.WriteLine(outputText);

        await foreach (var output in VoskSpeechRecognizer.RecognizeAsync(model, InputWavFileNameLong, cts.Token))
        {
            Console.WriteLine(output);
        }
    }
}