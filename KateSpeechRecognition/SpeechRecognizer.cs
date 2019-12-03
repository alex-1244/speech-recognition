using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Grpc.Auth;

namespace KateSpeechRecognition
{
	public class SpeechRecognizer
	{
		private readonly SpeechClient _client;
		private readonly RecognitionConfig _config;

		public SpeechRecognizer()
		{
			var credentials = GoogleCredential.FromFile("auth.json");
			var channel = new Grpc.Core.Channel(SpeechClient.DefaultEndpoint.Host,
				credentials.ToChannelCredentials());

			_client = SpeechClient.Create(channel);
			_config = new RecognitionConfig
			{
				Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
				AudioChannelCount = 1,
				EnableWordTimeOffsets = true,
				LanguageCode = LanguageCodes.Russian.Russia
			};
		}

		public string GetSpeechText(string fileUri)
		{
			var response = _client
				.LongRunningRecognize(_config, RecognitionAudio.FromStorageUri(fileUri))
				.PollUntilCompleted();

			return response.Result.Results
				.Select(x=> x.Alternatives.First().Transcript)
				.Aggregate((x, y) =>
					$"{x} {y}");
		}
	}
}