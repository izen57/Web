using Exceptions.NoteExceptions;

using Model;

using Repositories;

using Serilog;

namespace RepositoriesImplementations
{
	public class NoteFileRepo: INoteRepo
	{
		DirectoryInfo _isoStore;

		public NoteFileRepo()
		{
			try
			{
				_isoStore = new DirectoryInfo("IsolatedStorage");
				_isoStore.CreateSubdirectory("notes");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("Не удалось создать папку заметок.");
				throw new NoteCreateException(
					"NoteFileRepo: Невозможно создать защищённое хранилище.",
					ex
				);
			}
			Log.Logger.Information("Создана папка заметок.");
		}

		public Note Create(Note note)
		{
			if (File.Exists($"IsolatedStorage/notes/{note.Id}.txt"))
			{
				Log.Logger.Error($"NoteCreate: Файл notes/{note.Id}.txt нельзя открыть.");
				throw new NoteCreateException(
					$"Заметка с идентификатором {note.Id} уже существует.",
					new IOException("already exists")
				);
			}

			FileStream isoStream;
			try
			{
				isoStream = File.Create($"IsolatedStorage/notes/{note.Id}.txt");
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"NoteCreate: Файл notes/{note.Id}.txt нельзя открыть.");
				throw new NoteCreateException(
					$"Заметку с идентификатором {note.Id} нельзя открыть.",
					ex
				);
			}

			using (StreamWriter TextNote = new(isoStream))
			{
				TextNote.WriteLine(note.CreationTime);
				TextNote.WriteLine(note.OwnerId);
				TextNote.WriteLine(note.Body);
				TextNote.WriteLine(note.IsTemporal);
			}

			string userFilePath = $"IsolatedStorage/user/{note.OwnerId}.txt";

			try
			{
				isoStream = File.OpenWrite(userFilePath);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"AlarmClockCreate: Файл user/{note.OwnerId}.txt нельзя открыть.");
				throw new NoteCreateException(
					$"Не удалось создать будильник {note.Id} для пользователя {note.OwnerId}.",
					ex
				);
			}

			List<string> previousUserLines = File.ReadAllLines(userFilePath).ToList();
			previousUserLines.Insert(previousUserLines.IndexOf("N"), note.Id.ToString()!);
			File.WriteAllLines(userFilePath, previousUserLines);

			Log.Logger.Information(
				"NoteCreate: Создан файл заметки со следующей информацией:\n" +
				$"{note.Id}," +
				$"{note.OwnerId}," +
				$"{note.CreationTime}," +
				$"{note.Body}," +
				$"{note.IsTemporal}."
			);

			return note;
		}

		public Note Edit(Note note)
		{
			FileStream isoStream;
			try
			{
				isoStream = new(
					$"IsolatedStorage/notes/{note.Id}.txt",
					FileMode.Create,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"NoteEdit: Файл notes/{note.Id}.txt не найден.");
				throw new NoteEditException(
					$"Заметка с идентификатором {note.Id} не найдена.",
					ex
				);
			}

			using StreamWriter writer = new(isoStream);

			writer.WriteLine(note.CreationTime);
			writer.WriteLine(note.OwnerId);
			writer.WriteLine(note.Body);
			writer.WriteLine(note.IsTemporal);

			Log.Logger.Information("NoteEdit: Изменён файл заметки со следующей информацией:" +
				$"{note.Id}," +
				$"{note.OwnerId}," +
				$"{note.CreationTime}," +
				$"{note.Body}," +
				$"{note.IsTemporal}."
			);

			return note;
		}

		public void Delete(Guid guid, Guid ownerId)
		{
			string noteFilePath = $"IsolatedStorage/notes/{guid}.txt";
			if (File.Exists(noteFilePath))
			{
				Log.Logger.Error($"NoteDelete: Файл notes/{guid}.txt не найден.");
				throw new NoteDeleteException(
					$"Заметка с идентификатором {guid} не найдена.",
					new FileNotFoundException(noteFilePath)
				);
			}

			string userFilePath = $"IsolatedStorage/user/{ownerId}.txt";
			try
			{
				List<string> previousUserLines = File.ReadAllLines(userFilePath).ToList();
				previousUserLines.Remove(guid.ToString());
				File.WriteAllLines(userFilePath, previousUserLines);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"NoteDelete: Файл user/{ownerId}.txt не найден.");
				throw new NoteDeleteException(
					$"Пользователь {ownerId} не найден.",
					ex
				);
			}

			File.Delete(noteFilePath);
			Log.Logger.Information($"NoteDelete: Файл заметки удалён. Идентификатор заметки: {guid}.");
		}

		public Note? GetNote(Guid guid)
		{
			FileStream isoStream;

			try
			{
				isoStream = new(
					$"IsolatedStorage/notes/{guid}.txt",
					FileMode.Open,
					FileAccess.Write
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"NoteGet: Ошибка открытия заметки {guid}.");
				return null;
				//throw new NoteGetException(
				//	$"Заметка в защищённом хранилище пользователя {_ownerId} не найдена.",
				//	ex
				//);
			}

			using var readerStream = new StreamReader(isoStream);
			string? noteCreationTime = readerStream.ReadLine();
			string? noteBody = readerStream.ReadLine();
			string? noteOwnerId = readerStream.ReadLine();
			string? noteIsTemporal = readerStream.ReadLine();
			if (noteCreationTime == null || noteBody == null || noteIsTemporal == null || noteOwnerId == null)
			{
				Log.Logger.Error($"NoteGet: Ошибка разметки файла. Идентификатор заметки: {guid}.");
				throw new ArgumentNullException();
			}

			return new Note(
				guid,
				DateTime.Parse(noteCreationTime),
				noteBody,
				bool.Parse(noteIsTemporal),
				Guid.Parse(noteOwnerId)
			);
		}

		public List<Note> GetNotes()
		{
			IEnumerable<string> filelist;
			List<Note> noteList = new();
			try
			{
				filelist = Directory.EnumerateFiles("IsolatedStorage/notes");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("NoteGet: Папка для заметок в защищённом хранилище не найдена.");
				return noteList;
			}

			foreach (string fileName in filelist)
			{
				var note = GetNote(Guid.Parse(fileName.Replace(".txt", "").Replace("IsolatedStorage/notes/", "")));
				noteList.Add(note!);
			}

			return noteList;
		}

		public List<Note> GetNotes(Guid ownerId)
		{
			List<Note> noteList = new();
			FileStream fileStream = new(
				$"IsolatedStorage/users/{ownerId}.txt",
				FileMode.Open,
				FileAccess.Write
			);
			StreamReader streamReader = new(fileStream);

			while (streamReader.ReadLine() != "N")
				;
			while (streamReader.ReadLine() != "N")
			{
				string? noteId = streamReader.ReadLine();
				if (noteId != null)
					noteList.Add(GetNote(Guid.Parse(noteId))!);
			}

			return noteList;
		}

		public List<Note> GetNotes(Guid ownerId, QueryStringParameters param)
		{
			return GetNotes(ownerId)
				.Skip((param.PageNumber - 1) * param.PageSize)
				.Take(param.PageSize)
				.ToList();
		}
	}
}
