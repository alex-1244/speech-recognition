﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KateSpeechRecognition
{
	class Program
	{
		static void Main()
		{
			Console.OutputEncoding = Encoding.UTF8;

			var fr = new FileReader("../../InputFiles");
			var fc = new FileConverter(fr, "../../wav");
			var fileUploader = new FileUploader(fc);
			var speechRecognizer = new SpeechRecognizer();
			var fw = new OutputFileWriter("../../OutputFiles");
			var fm = new FileMover(fr, "../../ProcessedFiles");

			var files = fr.GetFileNames();
			var spinner = new ConsoleSpinner();

			Console.WriteLine($"Было обнаружено {files.Count} файлов, начинается распознавание");

			foreach (var file in files)
			{
				var token = new CancellationTokenSource();

				try
				{
					Console.WriteLine($"Сейчас обрабатывается файл: {file}");
					Task.Run(() => spinner.Turn(token.Token), token.Token);
					var convertedFile = fc.Convert(file);
					var fileUri = fileUploader.UploadFile(convertedFile);
					var recognitionResults = speechRecognizer.GetSpeechText(fileUri);
					fw.WriteText(convertedFile, recognitionResults);

					fm.Move(file);
					Console.WriteLine($"Файл {file} успешно обработан");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка при обработке файла {file}. Ошибка записана в файл 'log.txt'");
					File.AppendAllText("log.txt", ex.ToString());
				}
				finally
				{
					token.Cancel();
				}
			}

			Console.WriteLine("Программа завершила работу, файлы с распоззнанным текстом находтся в папке 'OutputFiles'");

			Console.WriteLine("Нажмите любую клавишу чтобы выйти");
			Console.ReadKey();
		}
	}
}
