using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Trustly
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
				Host.CreateDefaultBuilder(args)
						.ConfigureWebHostDefaults(webBuilder =>
						{
							var builder = new ConfigurationBuilder()
									 .SetBasePath(Directory.GetCurrentDirectory())  //location of the exe file
									 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

							var configuration = builder.Build();
							var maxRequestTimeoutInMinutes = configuration.GetValue<int>("MaxRequestTimeoutInMinutes");

							webBuilder
										.UseStartup<Startup>()
										.ConfigureKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(maxRequestTimeoutInMinutes); });
						});
	}
}
