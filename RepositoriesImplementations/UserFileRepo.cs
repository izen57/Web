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
			List<User> existingUsers = GetUsers();
			if (existingUsers.DistinctBy(user => user.Name).Count() != existingUsers.Count)
			{
				Log.Logger.Error($"UserCreate: Пользователь с логином {user.Name} уже существует. Ид-ор: {user.Id}.");
				throw new UserCreateException(
					$"Пользователь с логином {user.Name} уже существует.",
					new IOException("already exists")
				);
			}

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
			string fileToDelete = $"IsolatedStorage/users/{guid}.txt";
			if (File.Exists(fileToDelete) == false)
			{
				Log.Logger.Error($"UserDelete: Файл users/{guid}.txt не найден.");
				throw new UserDeleteException($"Пользователь с идентификатором {guid} не найден.");
			}
			DeleteUserData(guid);
			File.Delete(fileToDelete);

			Log.Logger.Information($"UserDelete: Файл пользователя {guid} удалён.");
		}

		void DeleteUserData(Guid guid)
		{
			FileStream fileStream = new($"IsolatedStorage/users/{guid}.txt", FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new(fileStream);

			while (streamReader.ReadLine() != "A")
				;
			string? alarmClockId;
			while ((alarmClockId = streamReader.ReadLine()) != "A")
				if (alarmClockId != null)
				{
					try
					{
						File.Delete($"IsolatedStorage/alarmclocks/{alarmClockId}.txt");
					}
					catch (Exception ex)
					{
						Log.Logger.Error($"UserDeleteData: Будильник {alarmClockId} пользователя {guid} не найден.");
						throw new AlarmClockGetException(
							$"Будильник {alarmClockId} пользователя {guid} не найден",
							ex
						);
					}
					Log.Logger.Information($"UserDeleteData: Будильник {alarmClockId} пользователя {guid} удалён.");
				}

			while (streamReader.ReadLine() != "N")
				;
			string? noteId;
			while ((noteId = streamReader.ReadLine()) != "N")
				if (noteId != null)
				{
					string fileToDelete = $"IsolatedStorage/notes/{noteId}.txt";
					if (File.Exists(fileToDelete))
					{
						File.Delete(fileToDelete);
						Log.Logger.Information($"UserDeleteData: Заметка {noteId} пользователя {guid} удалён.");
					}
				}
			fileStream.Close();
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
					FileAccess.Read
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

			string? userName;
			string? userPassword;
			using (StreamReader readerStream = new(isoStream))
			{
				userName = readerStream.ReadLine();
				userPassword = readerStream.ReadLine();
				if (userName == null || userPassword == null)
				{
					Log.Logger.Error($"GetUser: Ошибка разметки файла. Идентификатор пользователя: {guid}.");
					throw new ArgumentNullException();
				}
			}
			isoStream.Close();

			List<AlarmClock> userAlarmClocks = GetUserAlarmClocks(guid);
			List<Note> userNotes = GetUserNotes(guid);

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
				filelist = Directory.EnumerateFiles("IsolatedStorage/users/");
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
				var user = GetUser(Guid.Parse(
					fileName
						.Replace("IsolatedStorage/users/", "")
						.Replace(".txt", "")
				));
				usersList.Add(user!);
			}

			return usersList;
		}

		public List<User> GetUsers(QueryStringParameters param)
		{
			return GetUsers()
				.Skip((param.PageNumber - 1) * param.PageSize)
				.Take(param.PageSize)
				.ToList();
		}

		Note? GetUserNote(Guid guid, Guid ownerId)
		{
			FileStream isoStream;

			try
			{
				isoStream = new(
					$"IsolatedStorage/notes/{guid}.txt",
					FileMode.Open,
					FileAccess.Read
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"NoteGet: Ошибка открытия заметки {guid}.");
				throw new NoteGetException(
					$"Ошибка получения доступа к заметке. Идентификатор заметки: {guid}. Идентификатор пользователя: {ownerId}.",
					ex
				);
			}

			using var readerStream = new StreamReader(isoStream);
			string? noteOwnerId = readerStream.ReadLine();
			string? noteCreationTime = readerStream.ReadLine();
			string? noteBody = readerStream.ReadLine();
			string? noteIsTemporal = readerStream.ReadLine();
			if (noteCreationTime == null || noteBody == null || noteIsTemporal == null || noteOwnerId == null)
			{
				Log.Logger.Error($"NoteGet: Ошибка разметки файла. Идентификатор заметки: {guid}. Идентификатор пользователя: {ownerId}.");
				throw new ArgumentNullException();
			}

			isoStream.Close();
			return new Note(
				guid,
				DateTime.Parse(noteCreationTime),
				noteBody,
				bool.Parse(noteIsTemporal),
				Guid.Parse(noteOwnerId)
			);
		}

		List<Note> GetUserNotes(Guid ownerId)
		{
			List<Note> noteList = new();
			FileStream fileStream = new(
				$"IsolatedStorage/users/{ownerId}.txt",
				FileMode.Open,
				FileAccess.Read
			);
			StreamReader streamReader = new(fileStream);

			while (streamReader.ReadLine() != "N")
				;
			string? noteId;
			while ((noteId = streamReader.ReadLine()) != "N")
				if (noteId != null)
					noteList.Add(GetUserNote(
						Guid.Parse(noteId),
						ownerId
					)!);

			fileStream.Close();
			return noteList;
		}

		AlarmClock? GetUserAlarmClock(Guid guid, Guid ownerId)
		{
			FileStream isoStream;

			try
			{
				isoStream = new(
					$"IsolatedStorage/alarmclocks/{guid}.txt",
					FileMode.Open,
					FileAccess.Read
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
			string? alarmClockOwnerId = readerStream.ReadLine();
			string? alarmClockName = readerStream.ReadLine();
			string? alarmClockTime = readerStream.ReadLine();
			string? alarmClockColor = readerStream.ReadLine();
			string? alarmClockWork = readerStream.ReadLine();
			if (alarmClockName == null || alarmClockTime == null || alarmClockColor == null || alarmClockWork == null || alarmClockOwnerId == null)
			{
				Log.Logger.Error($"AlarmClockGet: Ошибка разметки файла будильника {guid} пользователя {ownerId}.");
				throw new ArgumentNullException();
			}

			isoStream.Close();
			if (Guid.Parse(alarmClockOwnerId) == ownerId)
				return new AlarmClock(
					guid,
					DateTime.Parse(alarmClockTime),
			//DateTime.ParseExact(alarmClockTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
			alarmClockName,
					Guid.Parse(alarmClockOwnerId),
					Color.FromName(alarmClockColor),
					bool.Parse(alarmClockWork)
				);
			else
				return null;
		}

		List<AlarmClock> GetUserAlarmClocks(Guid ownerId)
		{
			List<AlarmClock> alarmClockList = new();
			FileStream fileStream = new(
				$"IsolatedStorage/users/{ownerId}.txt",
				FileMode.Open,
				FileAccess.Read
			);
			StreamReader streamReader = new(fileStream);

			while (streamReader.ReadLine() != "A")
				;
			string? alarmClockId;
			while ((alarmClockId = streamReader.ReadLine()) != "A")
				if (alarmClockId != null)
					alarmClockList.Add(GetUserAlarmClock(
						Guid.Parse(alarmClockId)!,
						ownerId
					)!);

			fileStream.Close();
			return alarmClockList;
		}
	}
}
