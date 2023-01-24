using Exceptions.AlarmClockExceptions;

using Model;

using Repositories;

using Serilog;

using System.Drawing;
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
					"Невозможно создать защищённое хранилище будильников.",
					ex
				);
			}
			Log.Logger.Information("AlarmClockFileRepo: Создано хранилище будильников.");
		}

		public AlarmClock Create(AlarmClock alarmClock)
		{
			string filepath = $"IsolatedStorage/alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt";

			if (File.Exists(filepath))
			{
				Log.Logger.Error($"AlarmClockEdit: Файл alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt уже существует.");
				throw new AlarmClockCreateException(
					$"Не удалось создать будильник на дату и время {alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss} для пользователя {alarmClock.OwnerId}.",
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
				Log.Logger.Error($"AlarmClockEdit: Файл alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt нельзя открыть.");
				throw new AlarmClockCreateException(
					$"Не удалось создать будильник на дату и время {alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss} для пользователя {alarmClock.OwnerId}.",
					ex
				);
			}

			using StreamWriter writer = new(isoStream);
			writer.WriteLine(alarmClock.Name);
			writer.WriteLine(alarmClock.OwnerId);
			writer.WriteLine(alarmClock.AlarmClockColor.Name);
			writer.WriteLine(alarmClock.IsWorking);

			Log.Logger.Information("Создан файл будильника со следующей информацией:" +
				$"{alarmClock.Name}," +
				$"{alarmClock.OwnerId}," +
				$"{alarmClock.AlarmTime}," +
				$"{alarmClock.AlarmClockColor.Name}," +
				$"{alarmClock.IsWorking}."
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
				Log.Logger.Error($"AlarmClockEdit: Файл alarmclocks/{oldTime:dd.MM.yyyy HH-mm-ss}.txt не найден.");
				throw new AlarmClockEditException(
					$"Будильник пользователя {alarmClock.OwnerId} на {oldTime:dd.MM.yyyy HH-mm-ss} не найден.",
					ex
				);
			}

			using (StreamWriter writer = new(isoStream))
			{
				writer.WriteLine(alarmClock.Name);
				writer.WriteLine(alarmClock.OwnerId);
				writer.WriteLine(alarmClock.AlarmClockColor.Name);
				writer.WriteLine(alarmClock.IsWorking);
			}

			Log.Logger.Information("Файл будильника изменён. Новая информация:" +
				$"{alarmClock.Name}," +
				$"{alarmClock.OwnerId}," +
				$"{alarmClock.AlarmTime}," +
				$"{alarmClock.AlarmClockColor.Name}," +
				$"{alarmClock.IsWorking}."
			);

			File.Move(
				$"IsolatedStorage/alarmclocks/{oldTime:dd.MM.yyyy HH-mm-ss}.txt",
				$"IsolatedStorage/alarmclocks/{alarmClock.AlarmTime:dd.MM.yyyy HH-mm-ss}.txt"
			);

			return alarmClock;
		}

		public void Delete(DateTime alarmTime)
		{
			FileStream isoStream;
			string filepath = $"IsolatedStorage/alarmclocks/{alarmTime:dd.MM.yyyy HH-mm-ss}.txt";
			try
			{
				isoStream = new(filepath, FileMode.Open, FileAccess.Write);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"AlarmClockDelete: Файл alarmclocks/{alarmTime:dd.MM.yyyy HH-mm-ss}.txt не найден.");
				throw new AlarmClockDeleteException(
					$"Будильник на время {alarmTime:dd.MM.yyyy HH-mm-ss} не найден.",
					ex
				);
			}

			isoStream.Close();
			File.Delete(filepath);

			Log.Logger.Information($"AlarmClockDelete: Удалён файл будильника. Время будильника: {alarmTime}.");
		}

		public AlarmClock? GetAlarmClock(DateTime alarmTime)
		{
			FileStream isoStream;

			try
			{
				isoStream = new(
					$"IsolatedStorage/alarmclocks/{alarmTime:dd.MM.yyyy HH-mm-ss}.txt",
					FileMode.Open,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"AlarmClockGet: Ошибка открытие файла alarmclocks/{alarmTime:dd.MM.yyyy HH-mm-ss}.txt.");
				return null;
				//throw new AlarmClockGetException(
				//	$"Будильник пользователя {_ownerId} на время {alarmTime:dd.MM.yyyy HH-mm-ss}. не найден",
				//	ex
				//);
			}

			using var readerStream = new StreamReader(isoStream);
			string? alarmClockName = readerStream.ReadLine();
			string? alarmClockOwnerId = readerStream.ReadLine();
			string? alarmClockColor = readerStream.ReadLine();
			string? alarmClockWork = readerStream.ReadLine();
			if (alarmClockName == null || alarmClockColor == null || alarmClockWork == null || alarmClockOwnerId == null)
			{
				Log.Logger.Error($"AlarmClockGet: Ошибка разметки файла будильника. Время будильника: {alarmTime}.");
				throw new ArgumentNullException();
			}

			return new AlarmClock(
				alarmTime,
				alarmClockName,
				Guid.Parse(alarmClockOwnerId),
				Color.FromName(alarmClockColor),
				bool.Parse(alarmClockWork)
			);
		}

		public List<AlarmClock> GetAllAlarmClocks()
		{
			IEnumerable<string> filelist;
			List<AlarmClock> alarmClockList = new();
			try
			{
				filelist = Directory.EnumerateFiles("IsolatedStorage/alarmclocks");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("AlarmClockGet: Папка для будильников в защищённом хранилище не найдена.");
				return alarmClockList;
			}

			foreach (string fileName in filelist)
			{
				var alarmClock = GetAlarmClock(DateTime.ParseExact(
					fileName
						.Replace("IsolatedStorage/alarmclocks/", "")
						.Replace(".txt", "")
						.Replace("-", ":"),
					"dd.MM.yyyy HH:mm:ss",
					CultureInfo.InvariantCulture
				));
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