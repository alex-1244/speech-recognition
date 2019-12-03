using System;
using System.IO;
using System.Linq;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Google.Cloud.Storage.V1;
using Grpc.Auth;
using NAudio.Wave;
using static Google.Cloud.Speech.V1.RecognitionConfig.Types;

namespace KateSpeechRecognition
{
	class Program
	{
		static void Main(string[] args)
		{
			var fr = new FileReader("../../InputFiles");
			var fc = new FileConverter(fr, "../../wav");
			var fileUploader = new FileUploader(fc);
			var speechRecognizer = new SpeechRecognizer();
			var fw = new OutputFileWriter("../../OutputFiles");
			var fm = new FileMover(fr, "../../ProcessedFiles");

			var files = fr.GetFileNames();

			foreach (var file in files)
			{
				try
				{
					var convertedFile = fc.Convert(file);
					var fileUri = fileUploader.UploadFile(convertedFile);
					var recognitionResults = speechRecognizer.GetSpeechText(fileUri);
					fw.WriteText(convertedFile, recognitionResults);

					fm.Move(file);
				}
				catch (Exception ex)
				{
					File.AppendAllText("log.txt", ex.ToString());
				}
			}
		}

		//static void Main1()
		//{
		//	var credentials = GoogleCredential.FromFile("auth.json");
		//	var channel = new Grpc.Core.Channel(SpeechClient.DefaultEndpoint.Host,
		//		credentials.ToChannelCredentials());
		//	var client = SpeechClient.Create(channel);

		//	var storageClient = StorageClient.Create(credentials);

		//	RecognitionConfig config = new RecognitionConfig
		//	{
		//		Encoding = AudioEncoding.Linear16,
		//		SampleRateHertz = 8000,
		//		AudioChannelCount = 1,
		//		EnableWordTimeOffsets = true,
		//		LanguageCode = LanguageCodes.Russian.Russia
		//	};

		//	var fileUri = SaveFileToCloudStorage(storageClient, "Recording4.mp3");
		//	var response = client
		//		.LongRunningRecognize(config, RecognitionAudio.FromStorageUri(fileUri))
		//		.PollUntilCompleted();

		//	Console.OutputEncoding = System.Text.Encoding.UTF8;
		//	Console.WriteLine(response.Result.Results.First().Alternatives.First().Transcript);
		//}

		//private static string SaveFileToCloudStorage(StorageClient client, string filename)
		//{
		//	string newName = ConvertToWav(filename);

		//	var now = DateTime.UtcNow;
		//	var bucket = client.CreateBucket("kateSpeechRecognition", $"{now.Day}_{now.Month}_{now.Year}_{now.Hour}_{now.Minute}_{now.Second}");

		//	var content = File.ReadAllBytes(newName);

		//	var audioFile = client.UploadObject(bucket.Name, newName, "audio/wav", new MemoryStream(content));

		//	return $"gs://{bucket.Name}/{newName}";
		//}

		//private static string ConvertToWav(string filename)
		//{
		//	var newName = filename.Split('.')[0] + ".wav";
		//	using (var reader = new Mp3FileReader(filename))
		//	{
		//		WaveFileWriter.CreateWaveFile(newName, reader);
		//	}

		//	return newName;
		//}
	}
}
