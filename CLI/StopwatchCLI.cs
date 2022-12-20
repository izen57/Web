using Logic;
using Model;

using System.Drawing;
using User_Interface;

using SysStopwatch = System.Diagnostics;

namespace CLI
{
	public class StopwatchCLI: IStopwatchUI
	{
		private IStopwatchService _stopwatchService;

		public StopwatchCLI()
		{
			_stopwatchService = new StopwatchService(new Stopwatch(
				"Секундомер",
				Color.White,
				new SysStopwatch.Stopwatch(),
				new SortedSet<DateTime>(),
				false
			));
		}

		public void Exit()
		{
			Console.WriteLine("Выход из приложения...");
			Environment.Exit(0);
		}

		public void Menu()
		{
			Update();
			Console.WriteLine(
				"\n0 - Выход из программы.\n" +
				"1 - Запустить секундомер.\n" +
				"2 - Сбросить секундомер.\n" +
				"3 - Остановить секундомер.\n" +
				"4 - Установить флаг.\n" +
				"5 - Поменять цвет.\n" +
				"6 - Обновить представление.\n" +
				"\nВведите номер функции из списка:"
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

				if (choice >= 0 && choice <= 6)
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
					Exit();
					break;
				case 1:
					SetStopwatch();
					Menu();
					break;
				case 2:
					ResetStopwatch();
					Menu();
					break;
				case 3:
					StopStopWatch();
					Menu();
					break;
				case 4:
					AddStopwatchFlag();
					Menu();
					break;
				case 5:
					EditStopwatchColor();
					Menu();
					break;
				case 6:
					Update();
					Menu();
					break;
			}
		}

		private void Update()
		{
			Console.ForegroundColor = FromColor(_stopwatchService.Get().StopwatchColor);
			Console.WriteLine(
				$"Секундомер:\nНазвание: {_stopwatchService.Get().Name}\n" +
				$"Цвет: {_stopwatchService.Get().StopwatchColor.Name}\n" +
				$"Время: {_stopwatchService.Get().Timing.Elapsed}\n" +
				$"Режим работы: {(_stopwatchService.Get().IsWorking == true ? "Запущен" : "Не запущен")}\n");
			Console.WriteLine("Флаги:\n");
			ShowFlags();
			Console.ResetColor();
		}

		private ConsoleColor FromColor(Color c)
		{
			int index = c.R > 128 | c.G > 128 | c.B > 128 ? 8 : 0;
			index |= c.R > 64 ? 4 : 0;
			index |= c.G > 64 ? 2 : 0;
			index |= c.B > 64 ? 1 : 0;
			return (ConsoleColor) index;
		}

		private void ShowFlags()
		{
			foreach (var elem in _stopwatchService.Get().TimeFlags)
				Console.WriteLine(elem);
		}

		private void EditStopwatchColor()
		{
			Console.Write("Введите название цвета на английском: ");
			Color color = Color.FromName(Console.ReadLine());
			_stopwatchService.EditColor(color);
			Console.WriteLine("Цвет изменён.");
		}

		public void ResetStopwatch()
		{
			_stopwatchService.Reset();
			Console.WriteLine("Секундомер сброшен.");
		}

		public void SetStopwatch()
		{
			_stopwatchService.Set();
			Console.WriteLine("Секундомер установлен.");
		}

		public void AddStopwatchFlag()
		{
			Console.WriteLine($"Флаг на: {_stopwatchService.AddStopwatchFlag()}.");
		}

		public void StopStopWatch()
		{
			_stopwatchService.Get().IsWorking = false;
			Console.WriteLine($"Секундомер остановлен. Время: {_stopwatchService.Stop()}.");
		}
	}
}
