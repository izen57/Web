using Model;
using System.Drawing;
using System.IO.IsolatedStorage;

using Serilog;
using Exceptions.AlarmClockExceptions;
using Repositories;
using System.Globalization;

namespace RepositoriesImplementations
{
	public class AlarmClockFileRepo: IAlarmClockRepo
	{
		DirectoryInfo _isoStore;

		public AlarmClockFileRepo()
		{
			try
			{
				_isoStore = new DirectoryInfo("IsolatedStorage");
				_isoStore.CreateSubdirectory("alarmclocks");
			}
			catch (Exception ex)
			{
				throw new AlarmClockCreateException(
					"AlarmClockFileRepo: Невозможно создать защищённое хранилище будильников.",
					ex
				);
			}
			Log.Logger.Information("Создано хранилище будильников.");
		}

		public AlarmClock Create(AlarmClock alarmClock)
		{
			string filepath = $"IsolatedStorage/alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt";

			if (File.Exists(filepath))
			{
				Log.Logger.Error($"Файл с названием \"alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt\" уже существует.");
				throw new AlarmClockCreateException(
					$"AlarmClockEdit: Не удалось создать будильник на дату и время \"alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt\".",
					new IOException("already exists")
				);
			}

			FileStream isoStream;
			try
			{
				isoStream = File.Create(filepath);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Файл с названием \"alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt\" нельзя открыть.");
				throw new AlarmClockCreateException(
					$"AlarmClockEdit: Не удалось создать будильник на дату и время \"alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt\".",
					ex
				);
			}

			using StreamWriter writer = new(isoStream);
			writer.WriteLine(alarmClock.Name);
			writer.WriteLine(alarmClock.AlarmClockColor.Name);
			writer.WriteLine(alarmClock.IsWorking);

			Log.Logger.Information("Создан файл будильника со следующей информацией:" +
				$"{alarmClock.Name}," +
				$"{alarmClock.AlarmTime}," +
				$"{alarmClock.AlarmClockColor.Name}," +
				$"{alarmClock.IsWorking}." +
				$"Путь: {isoStream.Name}"
			);
			return alarmClock;
		}

		public AlarmClock Edit(AlarmClock alarmClock, DateTime oldTime)
		{
			FileStream isoStream;
			try
			{
				isoStream = new(
					$"IsolatedStorage/alarmclocks/{oldTime:dd.MM.yyyy HH-mm-ss}.txt",
					FileMode.Create,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Файл с названием \"alarmclocks/{oldTime:dd.MM.yyyy HH-mm-ss}.txt\" не найден.");
				throw new AlarmClockEditException(
					$"AlarmClockEdit: Будильник на {oldTime:dd.MM.yyyy HH-mm-ss} не найден.",
					ex
				);
			}

			using (StreamWriter writer = new(isoStream))
			{
				writer.WriteLine(alarmClock.Name);
				writer.WriteLine(alarmClock.AlarmClockColor.Name);
				writer.WriteLine(alarmClock.IsWorking);
			}

			Log.Logger.Information("Изменён файл будильника следующей информацией:" +
				$"{alarmClock.Name}," +
				$"{alarmClock.AlarmTime}," +
				$"{alarmClock.AlarmClockColor.Name}," +
				$"{alarmClock.IsWorking}."
			);

			File.Move(
				$"IsolatedStorage/alarmclocks/{oldTime:dd.MM.yyyy HH-mm-ss}.txt",
				$"IsolatedStorage/alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt");

			Log.Logger.Information("Файл будильника переименован.\n " +
				$"Старое название файла: \"alarmclocks/{oldTime:dd.MM.yyyy HH-mm-ss}.txt\".\n" +
				$"Новое название файла: \"alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt\"."
			);
			return alarmClock;
		}

		public void Delete(DateTime alarmTime)
		{
			FileStream isoStream;
			string filepath = $"IsolatedStorage/alarmclocks/{alarmTime:dd.MM.yyyy HH-mm-ss}.txt";
			try
			{
				isoStream = new(
					filepath,
					FileMode.Open,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Файл с названием \"alarmclocks/{alarmTime:dd.MM.yyyy HH-mm-ss}.txt\" не найден.");
				throw new AlarmClockDeleteException(
					$"AlarmClockDelete: Будильник на {alarmTime:dd.MM.yyyy HH-mm-ss} не найден.",
					ex
				);
			}

			isoStream.Close();
			File.Delete(filepath);

			Log.Logger.Information($"Удалён файл будильника. Время будильника: {alarmTime}.");

		}

		public AlarmClock? GetAlarmClock(DateTime alarmTime)
		{
			string[] filelist;
			string filepath = $"IsolatedStorage/alarmclocks/{alarmTime:dd.MM.yyyy HH-mm-ss}.txt";
			try
			{
				filelist = Directory.GetFiles("IsolatedStorage/alarmclocks", $"{alarmTime:dd.MM.yyyy HH-mm-ss}.txt");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("Папка для будильников в защищённом хранилище не найдена.");
				throw new AlarmClockGetException(
					"AlarmClockGet: Хранилище будильников не найдено.",
					ex
				);
			}

			foreach (string fileName in filelist)
				if (fileName.Equals(filepath))
				{
					using var readerStream = new StreamReader(new FileStream(
						filepath,
						FileMode.Open,
						FileAccess.Read
					));
					string? alarmClockName = readerStream.ReadLine();
					string? alarmClockColor = readerStream.ReadLine();
					string? alarmClockWork = readerStream.ReadLine();
					if (alarmClockName == null || alarmClockColor == null || alarmClockWork == null)
					{
						Log.Logger.Error($"Ошибка разметки файла будильника. Время будильника: {alarmTime}.");
						throw new ArgumentNullException();
					}

					return new AlarmClock(
						alarmTime,
						alarmClockName,
						Color.FromName(alarmClockColor),
						bool.Parse(alarmClockWork)
					);
				}

			return null;
		}
		public List<AlarmClock> GetAllAlarmClocks()
		{

			string[] filelist;
			try
			{
				filelist = Directory.GetFiles("IsolatedStorage/alarmclocks/");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("Папка для будильников в защищённом хранилище не найдена.");
				throw new AlarmClockGetException(
					"AlarmClockGet: Хранилище будильников не найдено.",
					ex
				);
			}

			List<AlarmClock> alarmClockList = new();
			foreach (string fileName in filelist)
			{
				var alarmClock = GetAlarmClock(DateTime.ParseExact(
						fileName
							.Replace("IsolatedStorage/alarmclocks/", "")
							.Replace(".txt", "")
							.Replace("-", ":"),
						"dd.MM.yyyy HH:mm:ss",
						CultureInfo.InvariantCulture
					)
				);
				alarmClockList.Add(alarmClock!);
			}

			return alarmClockList;
		}

		public List<AlarmClock> GetAlarmClocksByQuery(QueryStringParameters param)
		{
			return GetAllAlarmClocks()
				.Skip((param.PageNumber-1) * param.PageSize)
				.Take(param.PageSize)
				.ToList();
		}
	}
}