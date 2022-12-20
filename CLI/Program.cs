using Microsoft.Extensions.Configuration;

using Serilog;

namespace CLI
{
	public class Program
	{
		static void Main()
		{
			string _projectDir = Directory.GetCurrentDirectory();
			var _config = new ConfigurationBuilder()
				.SetBasePath(_projectDir)
				.AddJsonFile("appsettings.json")
				.Build();

			switch (_config["LoggerLevel"])
			{
				case "Verbose":
					Log.Logger = new LoggerConfiguration()
						.MinimumLevel.Verbose()
						.WriteTo.File(_config["Logger"])
						.CreateLogger();
					break;
				case "Debug":
					Log.Logger = new LoggerConfiguration()
						.MinimumLevel.Debug()
						.WriteTo.File(_config["Logger"])
						.CreateLogger();
					break;
				case "Information":
					Log.Logger = new LoggerConfiguration()
						.MinimumLevel.Information()
						.WriteTo.File(_config["Logger"])
						.CreateLogger();
					break;
				case "Warning":
					Log.Logger = new LoggerConfiguration()
						.MinimumLevel.Warning()
						.WriteTo.File(_config["Logger"])
						.CreateLogger();
					break;
				case "Error":
					Log.Logger = new LoggerConfiguration()
						.MinimumLevel.Error()
						.WriteTo.File(_config["Logger"])
						.CreateLogger();
					break;
				case "Fatal":
					Log.Logger = new LoggerConfiguration()
						.MinimumLevel.Fatal()
						.WriteTo.File(_config["Logger"])
						.CreateLogger();
					break;
			}

			Console.WriteLine(
				"Выберите пункт меню:\n" +
				"0 - Выход.\n" +
				"1 - Будильники.\n" +
				"2 - Заметки.\n" +
				"3 - Секундомер.\n"
			);

			bool flag = false;
			int choice = -1;
			while (flag == false)
			{
				try
				{
					choice = int.Parse(Console.ReadLine());
				}
				catch
				{
					choice = -1;
				}
				if (choice >= 0 && choice <= 3)
				{
					flag = true;
					break;
				}
				else
				{
					flag = false;
					Console.WriteLine("Ошибка ввода. Введите номер функции из списка.");
				}
			}

			switch (choice)
			{
				case 0:
					Console.WriteLine("Выход из приложения...");
					Log.Logger.Debug("Осуществлён выход из приложения.");
					Environment.Exit(0);
					break;
				case 1:
					AlarmClockCLI alarmClockCLI = new();
					alarmClockCLI.Menu();
					Log.Logger.Debug("Выбран пункт будильников.");
					break;
				case 2:
					NoteCLI noteCLI = new();
					noteCLI.Menu();
					Log.Logger.Debug("Выбран пункт заметок.");
					break;
				case 3:
					StopwatchCLI stopwatchCLI = new();
					stopwatchCLI.Menu();
					Log.Logger.Debug("Выбран пункт секундомера.");
					break;
			}
			Log.CloseAndFlush();
		}
	}
}
