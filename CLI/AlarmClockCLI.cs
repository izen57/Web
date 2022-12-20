using System.Drawing;
using System.Timers;

using RepositoriesImplementations;
using Logic;
using Model;

using User_Interface;

using Timer = System.Timers.Timer;

namespace CLI
{
	public class AlarmClockCLI: IAlarmClockUI
	{
		IAlarmClockService _alarmClockService;
		Timer _checkForTime = new(7000);

		public AlarmClockCLI()
		{
			_alarmClockService = new AlarmClockService(new AlarmClockFileRepo());
			_checkForTime.Enabled = true;
		}

		public void CreateAlarmClock()
		{
			Console.Write("Создание нового будильника. Введите время будильника: ");
			DateTime alarmTime;
			while (!DateTime.TryParse(Console.ReadLine(), out alarmTime))
				Console.Write("Ошибка ввода. Введите время будильника: ");

			Console.WriteLine("Введите название будильника: ");
			string alarmName = Console.ReadLine();

			Console.WriteLine("Введите цвет будильника на английском, которым он будет подавать сигналы: ");
			Color alarmColor = Color.FromName(Console.ReadLine());

			Console.WriteLine("Запустить будильник сейчас (y/n): ");
			bool isWorking = Console.ReadLine() == "y";

			try
			{
				_alarmClockService.Create(new AlarmClock(alarmTime, alarmName, alarmColor, isWorking));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Не удалось создать будильник. {ex.Message}.");
				return;
			}

			Console.WriteLine(
				"Будильник создан.\n\n" +
				$"Время (дд/ММ/гггг ЧЧ-мм-сс): {alarmTime}\n" +
				$"Название: {alarmName}\n" +
				$"Цвет: {alarmColor}\n" +
				$"Режим: {(isWorking == true ? "включён" : "выключен")}\n"
			);
		}

		public void DeleteAlarmClock()
		{
			ShowAlarmClocks();
			Console.WriteLine("============\nВыберите время, по которому будильник будет удалён (дд/ММ/гггг ЧЧ-мм-сс):");
			DateTime dateTime;
			while (!DateTime.TryParse(Console.ReadLine(), out dateTime))
				Console.Write("Ошибка ввода. Введите время будильника: ");

			bool flag = false;
			foreach (var alarmClock in _alarmClockService.GetAllAlarmClocks())
				if (dateTime == alarmClock.AlarmTime)
				{
					try
					{
						_alarmClockService.Delete(dateTime);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Не удалось удалить будильник. {ex.Message}.");
						return;
					}

					flag = true;
				}

			if (flag == false)
				Console.WriteLine("Таких будильников не найдено.");
			else
				Console.WriteLine("Будильник удалён.");
		}

		public void EditAlarmClock()
		{
			ShowAlarmClocks();
			Console.Write("============\nВыберите время, по которому будет изменён будильник (дд/ММ/гггг ЧЧ-мм-сс): ");
			DateTime dateTime;
			while (!DateTime.TryParse(Console.ReadLine(), out dateTime))
				Console.Write("Ошибка ввода. Введите время будильника: ");

			bool flag = false;
			foreach (var alarmClock in _alarmClockService.GetAllAlarmClocks())
				if (dateTime == alarmClock.AlarmTime)
				{
					ChooseAlarmClockParam(alarmClock);
					try
					{
						_alarmClockService.Edit(alarmClock, dateTime);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Не удалось изменить будильник. {ex.Message}.");
						return;
					}
					flag = true;
				}

			if (flag == false)
				Console.WriteLine("Таких будильников не найдено.");
			else
				Console.WriteLine("Будильник изменён.");
		}

		private void ChooseAlarmClockParam(AlarmClock alarmClock)
		{
			int choice;
			do
			{
				Console.WriteLine(
					"============\nВыберите параметр, по которому будет изменён выбранный будильник:\n" +
					"0 - Готово\n" +
					"1 - Время (дд/ММ/гггг ЧЧ-мм-сс)\n" +
					"2 - Название\n" +
					"3 - Цвет (на английском)\n" +
					$"4 - {(alarmClock.IsWorking == true ? "Выключить" : "Включить")}"
				);
				try
				{
					choice = int.Parse(Console.ReadLine());
				}
				catch
				{
					choice = 0;
				}

				switch (choice)
				{
					case 1:
						DateTime dateTime;
						while (!DateTime.TryParse(Console.ReadLine(), out dateTime))
							Console.Write("Ошибка ввода. Введите время будильника: ");

						alarmClock.AlarmTime = dateTime;
						break;
					case 2:
						Console.Write("Введите новое название будильника: ");
						alarmClock.Name = Console.ReadLine();
						break;
					case 3:
						Console.Write("Введите новый цвет будильника: ");
						alarmClock.AlarmClockColor = Color.FromName(Console.ReadLine());
						break;
					case 4:
						alarmClock.IsWorking = !alarmClock.IsWorking;
						break;
					default:
						continue;
				}
			} while (choice != 0);
		}

		public void Menu()
		{
			_checkForTime.Elapsed += AlarmClockSignal;
			Console.WriteLine(
				"\n0 - Выход из программы.\n" +
				"1 - Показать все будильники.\n" +
				"2 - Установить новый будильник.\n" +
				"3 - Изменить существующий будильник.\n" +
				"4 - Удалить существующий будильник.\n" +
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

				if (choice >= 0 && choice <= 4)
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
					ShowAlarmClocks();
					Menu();
					break;
				case 2:
					CreateAlarmClock();
					Menu();
					break;
				case 3:
					EditAlarmClock();
					Menu();
					break;
				case 4:
					DeleteAlarmClock();
					Menu();
					break;
			}
		}

		private ConsoleColor FromColor(Color c)
		{
			int index = c.R > 128 | c.G > 128 | c.B > 128 ? 8 : 0;
			index |= c.R > 64 ? 4 : 0;
			index |= c.G > 64 ? 2 : 0;
			index |= c.B > 64 ? 1 : 0;
			return (ConsoleColor) index;
		}

		public void AlarmClockSignal(object sender, ElapsedEventArgs e)
		{
			foreach (var alarmClock in _alarmClockService.GetAllAlarmClocks())
				if (alarmClock.IsWorking && DateTime.Now >= alarmClock.AlarmTime && DateTime.Now <= alarmClock.AlarmTime + new TimeSpan(0, 0, 30))
				{
					for (int i = 0; i < 10; ++i)
					{
						Console.Clear();
						Console.BackgroundColor = FromColor(alarmClock.AlarmClockColor);
						Console.Clear();
						Thread.Sleep(100);
						Console.ResetColor();
					}
					Console.Clear();

					Console.WriteLine($"Только что сработал будильник, установленный на время {alarmClock.AlarmTime}. Теперь этот будильник выключен.");

					alarmClock.IsWorking = false;
					_alarmClockService.Edit(alarmClock, alarmClock.AlarmTime);
					Menu();
				}
		}

		public void ShowAlarmClocks()
		{
			Console.WriteLine("Список всех будильников\n============");
			try
			{
				foreach (var alarmclock in _alarmClockService.GetAllAlarmClocks())
					Console.WriteLine(
						$"\nВремя: {alarmclock.AlarmTime}\n" +
						$"Название: {alarmclock.Name}\n" +
						$"Цвет: {alarmclock.AlarmClockColor.Name}\n" +
						$"Режим: {(alarmclock.IsWorking == true ? "включён" : "выключен")}"
					);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Не удалось просмотреть будильник. {ex.Message}.");
				return;
			}

			Console.WriteLine("============");
		}

		public void Exit()
		{
			Console.WriteLine("Выход из приложения...");
			Environment.Exit(0);
		}
	}
}
