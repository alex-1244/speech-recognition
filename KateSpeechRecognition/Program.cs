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
	}
}
