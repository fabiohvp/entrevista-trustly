using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trustly.Managers
{
	public class RequestManager : IRequestManager
	{
		private long _counter = 1;

		public async Task<string> DownloadStringAsync(string url)
		{
			try
			{
				await CheckCounter();

				var request = CreateHttpWebRequest(url);
				var fileContent = await GetFileContent(request);
				return fileContent;
			}
			catch (Exception ex) //this is here just for debugging, in this demo I am not handling exceptions
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		private async Task<string> GetFileContent(HttpWebRequest request)
		{
			var response = await request.GetResponseAsync();

			using (var stream = response.GetResponseStream())
			{
				var reader = new StreamReader(stream, Encoding.UTF8);
				var fileContent = reader.ReadToEnd();
				return fileContent;
			}
		}

		private async Task CheckCounter()
		{
			if (_counter % 99 == 0) //100 will give Too Many Request
			{
				await Task.Delay(60000).ContinueWith(_ =>
				{
					Interlocked.Increment(ref _counter);
				});
			}
			else
			{
				Interlocked.Increment(ref _counter);
			}
		}

		private HttpWebRequest CreateHttpWebRequest(string url)
		{
			var request = HttpWebRequest.Create(url) as HttpWebRequest;
			request.Method = "GET";
			/* Sart browser signature */
			request.Headers.Add("referer", "https://github.com/dotnet/efcore");
			request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
			request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
			return request;
		}
	}
}