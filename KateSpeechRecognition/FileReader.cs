using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KateSpeechRecognition
{
	public class FileMover
	{
		private readonly string _path;
		private readonly FileReader _fr;
		private readonly string _folder;

		public FileMover(FileReader fr, string path)
		{
			_fr = fr;
			_path = path;

			var now = DateTime.UtcNow;
			_folder = $"{now.Day}_{now.Month}_{now.Year}_{now.Hour}_{now.Minute}_{now.Second}";
		}

		public void Move(string file)
		{
			var filePath = _fr.GetFilePath(file);

			var destPath = $"{_path}/{_folder}/{file}";

			if (File.Exists(destPath))
			{
				File.Delete($"{_path}/{_folder}/{file}");
			}

			File.Move(filePath, destPath);
		}
	}

	public class FileReader
	{
		private readonly string _filesPath;

		public FileReader(string filesPath)
		{
			_filesPath = filesPath;
		}

		public List<string> GetFileNames()
		{
			return Directory
				.EnumerateFiles(_filesPath)
				.Select(Path.GetFileName)
				.ToList();
		}

		public string GetFilePath(string filename)
		{
			return $"{_filesPath}/{filename}";
		}
	}
}
