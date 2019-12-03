using System.IO;
using System.Text;

namespace KateSpeechRecognition
{
	public class OutputFileWriter
	{
		private readonly string _path;

		public OutputFileWriter(string path)
		{
			_path = path;
		}

		public void WriteText(string filename, string text)
		{
			var textFileName = filename.Split('.')[0] + ".txt";
			File.WriteAllText($"{_path}/{textFileName}", text, Encoding.UTF8);
		}
	}
}