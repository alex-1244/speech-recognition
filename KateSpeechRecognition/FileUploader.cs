using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;

namespace KateSpeechRecognition
{
	public class FileUploader
	{
		private readonly FileConverter _fileConverter;
		private readonly Bucket _bucket;
		private readonly StorageClient _client;

		public FileUploader(FileConverter fc)
		{
			_fileConverter = fc;

			var credentials = GoogleCredential.FromFile("auth.json");
			_client = StorageClient.Create(credentials);
			
			var now = DateTime.UtcNow;
			_bucket = _client.CreateBucket("kateSpeechRecognition", $"{now.Day}_{now.Month}_{now.Year}_{now.Hour}_{now.Minute}_{now.Second}");
		}

		public string UploadFile(string filename)
		{
			var filePath = _fileConverter.GetFilePath(filename);
			var content = File.ReadAllBytes(filePath);

			var audioFile = _client.UploadObject(_bucket.Name, filename, "audio/wav", new MemoryStream(content));

			return $"gs://{_bucket.Name}/{filename}";
		}
	}
}