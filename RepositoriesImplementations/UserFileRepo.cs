using Exceptions.AlarmClockExceptions;
using Exceptions.NoteExceptions;
using Exceptions.UserExceptions;

using Model;

using Repositories;

using Serilog;

using System.Drawing;
using System.Globalization;

namespace RepositoriesImplementations
{
	public class UserFileRepo: IUserRepo
	{
		DirectoryInfo _isoStore;

		public UserFileRepo()
		{
			try
			{
				_isoStore = new DirectoryInfo("IsolatedStorage");
				_isoStore.CreateSubdirectory("users");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("Папку пользователей не удалось создать.");
				throw new UserCreateException(
					"Невозможно создать защищённое хранилище пользователей.",
					ex
				);
			}
			Log.Logger.Information("Создана папка для заметок.");
		}

		public User Create(User user)
		{
			if (File.Exists($"IsolatedStorage/users/{user.Id}.txt"))
			{
				Log.Logger.Error($"UserCreate: Файл users/{user.Id}.txt нельзя открыть.");
				throw new UserCreateException(
					$"Пользователь с идентификатором {user.Id} уже существует.",
					new IOException("already exists")
				);
			}

			FileStream isoStream;
			try
			{
				isoStream = File.Create($"IsolatedStorage/users/{user.Id}.txt");
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"UserCreate: Файл users/{user.Id}.txt нельзя открыть.");
				throw new UserCreateException(
					$"Не удалось создать пользователя с идентификатором {user.Id}.",
					ex
				);
			}

			using StreamWriter TextUser = new(isoStream);

			string alarmClockLog = "A\n";
			foreach (AlarmClock alarmClock in user.UserAlarmClocks)
				alarmClockLog += alarmClock.Id + "\n";
			alarmClockLog += "A";

			string noteLog = "N\n";
			foreach (Note note in user.UserNotes)
				noteLog += note.Id + "\n";
			noteLog += "N";

			TextUser.WriteLine(user.Name);
			TextUser.WriteLine(user.Password);
			TextUser.WriteLine(alarmClockLog);
			TextUser.WriteLine(noteLog);

			Log.Logger.Information(
				"UserCreate: Создан файл пользователя со следующей информацией:\n" +
				$"{user.Id}," +
				$"{user.Name}," +
				$"{user.Password}" +
				alarmClockLog +
				noteLog
			);

			return user;
		}

		public void Delete(Guid guid)
		{
			FileStream isoStream;
			try
			{
				isoStream = new(
					$"IsolatedStorage/users/{guid}.txt",
					FileMode.Open,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"UserDelete: Файл users/{guid}.txt не найден.");
				throw new UserDeleteException(
					$"Пользователь с идентификатором {guid} не найден.",
					ex
				);
			}

			isoStream.Close();
			Log.Logger.Information($"UserDelete: Файл пользователя {guid} удалён.");
		}

		public User Edit(User user)
		{
			FileStream isoStream;
			try
			{
				isoStream = new(
					$"IsolatedStorage/users/{user.Id}.txt",
					FileMode.Create,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Файл users/{user.Id}.txt не найден.");
				throw new UserEditException(
					$"UserEdit: Пользователь с идентификатором {user.Id} не найден.",
					ex
				);
			}

			using StreamWriter TextUser = new(isoStream);

			string alarmClockLog = "A\n";
			foreach (AlarmClock alarmClock in user.UserAlarmClocks)
				alarmClockLog += alarmClock.Id + "\n";
			alarmClockLog += "A";

			string noteLog = "N\n";
			foreach (Note note in user.UserNotes)
				noteLog += note.Id + "\n";
			noteLog += "N";

			TextUser.WriteLine(user.Name);
			TextUser.WriteLine(user.Password);
			TextUser.WriteLine(alarmClockLog);
			TextUser.WriteLine(noteLog);

			Log.Logger.Information(
				"UserCreate: Файл пользователя изменён. Новая информация:\n" +
				$"{user.Id}," +
				$"{user.Name}," +
				$"{user.Password}" +
				alarmClockLog +
				noteLog
			);

			return user;
		}

		public User? GetUser(Guid guid)
		{
			FileStream isoStream;

			try
			{
				isoStream = new(
					$"IsolatedStorage/users/{guid}.txt",
					FileMode.Open,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"UserGet: Папка для пользователей в защищённом хранилище {guid} не найдена.");
				return null;
				//throw new UserGetException(
				//	$"Пользователь {guid} в защищённом хранилище не найдена.",
				//	ex
				//);
			}

			using var readerStream = new StreamReader(isoStream);
			string? userName = readerStream.ReadLine();
			string? userPassword = readerStream.ReadLine();
			if (userName == null || userPassword == null)
			{
				Log.Logger.Error($"GetUser: Ошибка разметки файла. Идентификатор пользователя: {guid}.");
				throw new ArgumentNullException();
			}

			List<AlarmClock> userAlarmClocks = GetUserAlarmClocks(guid, readerStream);
			List<Note> userNotes = GetUserNotes(guid, readerStream);

			return new User(
				guid,
				userName,
				userPassword,
				userAlarmClocks,
				userNotes
			);
		}

		public List<User> GetUsers()
		{
			IEnumerable<string> filelist;
			try
			{
				filelist = Directory.EnumerateDirectories($"IsolatedStorage/users");
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"UserGet: Папка пользователя в защищённом хранилище не найдена.");
				throw new UserGetException(
					"Папка пользователя в защищённом хранилище не найдена.",
					ex
				);
			}

			List<User> usersList = new();
			foreach (string fileName in filelist)
			{
				var user = GetUser(Guid.Parse(fileName.Replace("IsolatedStorage/users/", "")));
				usersList.Add(user!);
			}

			return usersList;
		}

		public List<User> GetUsersByQuery(QueryStringParameters param)
		{
			return GetUsers()
				.Skip((param.PageNumber - 1) * param.PageSize)
				.Take(param.PageSize)
				.ToList();
		}

		public Note? GetUserNote(Guid ownerId, Guid Id)
		{
			FileStream isoStream;

			try
			{
				isoStream = new(
					$"IsolatedStorage/notes/{Id}.txt",
					FileMode.Open,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"NoteGet: Ошибка открытия заметки {Id}.");
				throw new NoteGetException(
					$"Ошибка получения доступа к заметке. Идентификатор заметки: {Id}. Идентификатор пользователя: {ownerId}.",
					ex
				);
			}

			using var readerStream = new StreamReader(isoStream);
			string? noteCreationTime = readerStream.ReadLine();
			string? noteBody = readerStream.ReadLine();
			string? noteOwnerId = readerStream.ReadLine();
			string? noteIsTemporal = readerStream.ReadLine();
			if (noteCreationTime == null || noteBody == null || noteIsTemporal == null || noteOwnerId == null)
			{
				Log.Logger.Error($"NoteGet: Ошибка разметки файла. Идентификатор заметки: {Id}. Идентификатор пользователя: {ownerId}.");
				throw new ArgumentNullException();
			}

			return new Note(
				Id,
				DateTime.Parse(noteCreationTime),
				noteBody,
				bool.Parse(noteIsTemporal),
				Guid.Parse(noteOwnerId)
			);
		}

		public List<Note> GetUserNotes(Guid ownerId, StreamReader streamReader)
		{
			List<Note> noteList = new();

			streamReader.ReadLine();
			while (streamReader.ReadLine() != "N")
			{
				string? noteId = streamReader.ReadLine();
				if (noteId != null)
					noteList.Add(GetUserNote(
						ownerId,
						Guid.Parse(noteId)
					)!);
			}

			return noteList;
		}

		public AlarmClock? GetUserAlarmClock(Guid ownerId, Guid guid)
		{
			FileStream isoStream;

			try
			{
				isoStream = new(
					$"IsolatedStorage/alarmclocks/{guid}.txt",
					FileMode.Open,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"AlarmClockGet: Ошибка открытия файла alarmclocks/{guid}.txt.");
				throw new AlarmClockGetException(
					$"Будильник {guid} пользователя {ownerId} не найден",
					ex
				);
			}

			using var readerStream = new StreamReader(isoStream);
			string? alarmClockName = readerStream.ReadLine();
			string? alarmClockTime = readerStream.ReadLine();
			string? alarmClockOwnerId = readerStream.ReadLine();
			string? alarmClockColor = readerStream.ReadLine();
			string? alarmClockWork = readerStream.ReadLine();
			if (alarmClockName == null || alarmClockTime == null || alarmClockColor == null || alarmClockWork == null || alarmClockOwnerId == null)
			{
				Log.Logger.Error($"AlarmClockGet: Ошибка разметки файла будильника {guid} пользователя {ownerId}.");
				throw new ArgumentNullException();
			}

			if (Guid.Parse(alarmClockOwnerId) == ownerId)
				return new AlarmClock(
					guid,
					DateTime.Parse(alarmClockTime),
					alarmClockName,
					Guid.Parse(alarmClockOwnerId),
					Color.FromName(alarmClockColor),
					bool.Parse(alarmClockWork)
				);
			else
				return null;
		}

		public List<AlarmClock> GetUserAlarmClocks(Guid ownerId, StreamReader streamReader)
		{
			List<AlarmClock> alarmClockList = new();

			streamReader.ReadLine();
			while (streamReader.ReadLine() != "A")
			{
				string? alarmClockId = streamReader.ReadLine();
				if (alarmClockId != null)
					alarmClockList.Add(GetUserAlarmClock(
						ownerId,
						Guid.Parse(alarmClockId)!
					)!);
			}

			return alarmClockList;
		}
	}
}
