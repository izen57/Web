using Model;
using System.Drawing;
using System.IO.IsolatedStorage;

using Serilog;
using Exceptions.AlarmClockExceptions;
using Repositories;

namespace RepositoriesImplementations
{
	public class AlarmClockFileRepo: IAlarmClockRepo
	{
		IsolatedStorageFile _isoStore;

		public AlarmClockFileRepo()
		{
			try
			{
				_isoStore = IsolatedStorageFile.GetUserStoreForAssembly();
				_isoStore.CreateDirectory("alarmclocks");
			}
			catch (Exception ex)
			{
				throw new AlarmClockCreateException(
					"AlarmClockFileRepo: Невозможно создать защищённое хранилище будильников.",
					ex
				);
			}
			Log.Logger.Information("Создана папка для будильников.");
			
		}

		public void Create(AlarmClock alarmClock)
		{
			if (_isoStore.AvailableFreeSpace <= 0)
			{
				Log.Logger.Error("Место в защищённом хранилище будильников закончилось.");
				throw new AlarmClockCreateException(
					"AlarmClockCreate: Место в защищённом хранилище будильников закончилось.",
					new IsolatedStorageException()
				);
			}
			string filepath = $"alarmclocks/{alarmClock.AlarmTime:dd/MM/yyyy HH-mm-ss}.txt";

			IsolatedStorageFileStream isoStream;
			try
			{
				isoStream = _isoStore.CreateFile(filepath);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Файл с названием \"alarmclocks/{alarmClock.AlarmTime:dd/MM/yyyy HH-mm-ss}.txt\" нельзя открыть.");
				throw new AlarmClockEditException(
					$"AlarmClockEdit: Файл с названием \"alarmclocks/{alarmClock.AlarmTime:dd/MM/yyyy HH-mm-ss}.txt\" нельзя открыть.",
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
				$"{alarmClock.IsWorking}."
			);
			
		}

		public void Edit(AlarmClock alarmClock, DateTime oldTime)
		{
			IsolatedStorageFileStream isoStream;
			try
			{
				isoStream = new(
					$"alarmclocks/{oldTime:dd/MM/yyyy HH-mm-ss}.txt",
					FileMode.Create,
					FileAccess.Write,
					_isoStore
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Файл с названием \"alarmclocks/{oldTime:dd/MM/yyyy HH-mm-ss}.txt\" не найден.");
				throw new AlarmClockEditException(
					$"AlarmClockEdit: Файл с названием \"alarmclocks/{oldTime:dd/MM/yyyy HH-mm-ss}.txt\" не найден.",
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

			_isoStore.MoveFile($"alarmclocks/{oldTime:dd/MM/yyyy HH-mm-ss}.txt", $"alarmclocks/{alarmClock.AlarmTime:dd/MM/yyyy HH-mm-ss}.txt");

			Log.Logger.Information("Файл будильника переименован.\n " +
				$"Старое название файла: \"alarmclocks/{oldTime:dd/MM/yyyy HH-mm-ss}.txt\".\n" +
				$"Новое название файла: \"alarmclocks/{alarmClock.AlarmTime:dd/MM/yyyy HH-mm-ss}.txt\"."
			);
			
		}

		public void Delete(DateTime alarmTime)
		{
			IsolatedStorageFileStream isoStream;
			string filepath = $"alarmclocks/{alarmTime:dd/MM/yyyy HH-mm-ss}.txt";
			try
			{
				isoStream = new(
					filepath,
					FileMode.Open,
					FileAccess.Write,
					_isoStore
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Файл с названием \"alarmclocks/{alarmTime:dd/MM/yyyy HH-mm-ss}.txt\" не найден.");
				throw new AlarmClockDeleteException(
					$"AlarmClockDelete: Файл с названием \"alarmclocks/{alarmTime:dd/MM/yyyy HH-mm-ss}.txt\" не найден.",
					ex
				);
			}

			isoStream.Close();
			_isoStore.DeleteFile(filepath);

			Log.Logger.Information($"Удалён файл будильника. Время будильника: {alarmTime}.");

		}

		public AlarmClock? GetAlarmClock(DateTime alarmTime)
		{
			string[] filelist;
			string filepath = $"alarmclocks/{alarmTime:dd/MM/yyyy HH-mm-ss}.txt";
			try
			{
				filelist = _isoStore.GetFileNames(filepath);
			}
			catch (Exception ex)
			{
				Log.Logger.Error("Папка для будильников в защищённом хранилище не найдена.");
				throw new AlarmClockGetException(
					"AlarmClockGet: Папка для будильников в защищённом хранилище не найдена.",
					ex
				);
			}

			foreach (string fileName in filelist)
				if (fileName.Equals($"{alarmTime:dd/MM/yyyy HH-mm-ss}.txt"))
				{
					using var readerStream = new StreamReader(new IsolatedStorageFileStream(
						filepath,
						FileMode.Open,
						FileAccess.Read,
						_isoStore
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

		public List<AlarmClock> GetAllAlarmClocksList()
		{
			string[] filelist;
			try
			{
				filelist = _isoStore.GetFileNames("alarmclocks/");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("Папка для будильников в защищённом хранилище не найдена.");
				throw new AlarmClockGetException(
					"AlarmClockGet: Папка для будильников в защищённом хранилище не найдена.",
					ex
				);
			}

			List<AlarmClock> alarmClockList = new();
			foreach (string fileName in filelist)
			{
				var alarmClock = GetAlarmClock(DateTime.Parse(fileName.Replace(".txt", "").Replace("-", ":")));
				alarmClockList.Add(alarmClock!);
			}

			return alarmClockList;
		}
	}
}