using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Vosk;

namespace ConsoleAppSpeechRecognition;

public static class VoskSpeechRecognizer
{
    private const float WavSampleRate = 16000.0f; // or 8000.0f be careful this is important parameter for speech recognition, check your files first 

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        
    };

    /// <summary>
    /// Useful method for short wav files
    /// </summary>
    /// <param name="model"></param>
    /// <param name="inputWavFileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string Recognize(Model model, string inputWavFileName, CancellationToken cancellationToken)
    {
        ValidateInput(model, inputWavFileName);
        
        using var voskRecognizer = new VoskRecognizer(model, WavSampleRate);
        voskRecognizer.SetMaxAlternatives(0);
        voskRecognizer.SetWords(true);

        var sb = new StringBuilder();

        using (Stream stream = File.OpenRead(inputWavFileName))
        {
            var buffer = new byte[4096];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return string.Empty;
                }

                if (voskRecognizer.AcceptWaveform(buffer, bytesRead))
                {
                    var output = DeserializeOutput(voskRecognizer.Result());
                    if (string.IsNullOrWhiteSpace(output.Text)) continue;
                    sb.AppendLine(output.Text);
                }
            }
        }

        var finalOutput = DeserializeOutput(voskRecognizer.FinalResult());
        sb.AppendLine(finalOutput.Text);

        return sb.ToString();
    }

    /// <summary>
    /// Useful method for long wav files
    /// </summary>
    /// <param name="model"></param>
    /// <param name="inputWavFileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static async IAsyncEnumerable<string> RecognizeAsync(Model model, string inputWavFileName, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ValidateInput(model, inputWavFileName);

        using var voskRecognizer = new VoskRecognizer(model, WavSampleRate);
        voskRecognizer.SetMaxAlternatives(0);
        voskRecognizer.SetWords(true);

        await using (Stream stream = File.OpenRead(inputWavFileName))
        {
            var buffer = new byte[4096];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                if (!voskRecognizer.AcceptWaveform(buffer, bytesRead)) continue;
                var output = DeserializeOutput(voskRecognizer.Result());
                if (string.IsNullOrWhiteSpace(output.Text)) continue;
                yield return output.Text;
            }
        }

        var finalOutput = DeserializeOutput(voskRecognizer.FinalResult());
        yield return finalOutput.Text;
    }

    private static void ValidateInput(Model model, string inputWavFileName)
    {
        if (!File.Exists(inputWavFileName))
        {
            throw new ArgumentException($"File doesn't exist '{inputWavFileName}'");
        }

        if (model == default)
        {
            throw new ArgumentException($"{nameof(Model)} cannot be null");
        }
    }

    private static OutputStruct DeserializeOutput(string output) =>
        JsonSerializer.Deserialize<OutputStruct>(output, Options);
}