using Serilog;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;

namespace IO.Swagger
{
	/// <summary>
	/// Program
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.File("logs/log.txt")
				.CreateLogger();

			foreach (var arg in args)
				Log.Logger.Information($"{arg}0_0\n");

			CreateWebHostBuilder(args).Build().Run();
		}

		/// <summary>
		/// Create the web host builder.
		/// </summary>
		/// <param name="args"></param>
		/// <returns>IWebHostBuilder</returns>
		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
