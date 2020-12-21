using System;
using Xunit;
using Trustly.Managers;

namespace tests
{
	public class RequestManagerTest
	{
		[Fact]
		public void DownloadStringAShouldNotBeEmpty()
		{
			var requestManager = new RequestManager();
			var content = requestManager.DownloadStringAsync("https://www.google.com").Result;
			Assert.NotEqual(content, "");
		}
	}
}
