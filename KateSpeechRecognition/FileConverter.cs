using System.Net.Http.Headers;
using NAudio.Wave;

namespace KateSpeechRecognition
{
	public class FileConverter
	{
		private readonly string _outputFilesPath;
		private readonly FileReader _fileReader;

		public FileConverter(FileReader fr, string outputFilesPath)
		{
			_fileReader = fr;
			_outputFilesPath = outputFilesPath;
		}

		public string Convert(string inputFileName)
		{
			var newName = inputFileName.Split('.')[0] + ".wav";
			using (var reader = new Mp3FileReader(_fileReader.GetFilePath(inputFileName)))
			{
				WaveFileWriter.CreateWaveFile(GetFilePath(newName), reader);
			}

			return newName;
		}

		public string GetFilePath(string filename)
		{
			return $"{_outputFilesPath}/{filename}";
		}
	}
}