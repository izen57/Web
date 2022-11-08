﻿using Exceptions.NoteExceptions;

using Model;

using Repositories;

using Serilog;

using System.IO.IsolatedStorage;

namespace RepositoriesImplementations
{
	public class NoteFileRepo: INoteRepo
	{
		IsolatedStorageFile _isoStore;

		public NoteFileRepo()
		{
			try
			{
				_isoStore = IsolatedStorageFile.GetUserStoreForAssembly();
				_isoStore.CreateDirectory("notes");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("Папку заметок не удалось создать.");
				throw new NoteCreateException(
					"NoteFileRepo: Невозможно создать защищённое хранилище заметок.",
					ex
				);
			}
			Log.Logger.Information("Создана папка для заметок.");
		}

		public Note Create(Note note)
		{
			if (_isoStore.AvailableFreeSpace <= 0)
			{
				Log.Logger.Error("Место в хранилище заметок закончилось.");
				throw new NoteCreateException(
					"NoteCreate: В системе слишком много заметок.",
					new IsolatedStorageException()
				);
			}

			IsolatedStorageFileStream isoStream;
			try
			{
				isoStream = _isoStore.CreateFile($"notes/{note.Id}.txt");
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Файл с названием \"notes/{note.Id}.txt\" нельзя открыть.");
				throw new NoteCreateException(
					$"Заметку с идентификатором {note.Id} нельзя открыть.",
					ex
				);
			}

			using StreamWriter TextNote = new(isoStream);
			TextNote.WriteLine(note.CreationTime);
			TextNote.WriteLine(note.Body);
			TextNote.WriteLine(note.IsTemporal);
			Log.Logger.Information(
				$"Создан файл заметки со следующей информацией:\n" +
				$"{note.Id}," +
				$"{note.CreationTime}," +
				$"{note.Body}," +
				$"{note.IsTemporal}."
			);

			return note;
		}

		public Note Edit(Note note)
		{
			IsolatedStorageFileStream isoStream;
			try
			{
				isoStream = new(
					$"notes/{note.Id}.txt",
					FileMode.Create,
					FileAccess.Write,
					_isoStore
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Файл с названием \"notes/{note.Id}.txt\" не найден.");
				throw new NoteEditException(
					$"NoteEdit: Заметка с идентификатором {note.Id} не найдена.",
					ex
				);
			}

			using StreamWriter writer = new(isoStream);

			writer.WriteLine(note.CreationTime);
			writer.WriteLine(note.Body);
			writer.WriteLine(note.IsTemporal);

			Log.Logger.Information("Изменён файл заметки со следующей информацией:" +
				$"{note.Id}," +
				$"{note.CreationTime}," +
				$"{note.Body}," +
				$"{note.IsTemporal}."
			);

			return note;
		}

		public void Delete(Guid Id)
		{
			IsolatedStorageFileStream isoStream;
			try
			{
				isoStream = new(
					$"notes/{Id}.txt",
					FileMode.Open,
					FileAccess.Write,
					_isoStore
				);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{DateTime.Now}: Файл с названием \"notes/{Id}.txt\" не найден.");
				throw new NoteDeleteException(
					$"NoteDelete: Заметка с идентификатором {Id} не найдена.",
					ex
				);
			}

			isoStream.Close();
			_isoStore.DeleteFile($"notes/{Id}.txt");

			Log.Logger.Information($"Удалён файл заметки. Идентификатор заметки: {Id}.");
		}

		public Note? GetNote(Guid Id)
		{
			string[] filelist;
			try
			{
				filelist = _isoStore.GetFileNames($"notes/{Id}.txt");
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Папка для заметок в защищённом хранилище не найдена.");
				throw new NoteGetException(
					"GetNote: Папка для заметок в защищённом хранилище не найдена.",
					ex
				);
			}

			foreach (string fileName in filelist)
				if (fileName.Replace(".txt", "") == Id.ToString())
				{
					using var readerStream = new StreamReader(new IsolatedStorageFileStream(
						$"notes/{Id}.txt",
						FileMode.Open,
						FileAccess.Read,
						_isoStore
					));
					string? noteCreationTime = readerStream.ReadLine();
					string? noteBody = readerStream.ReadLine();
					string? noteIsTemporal = readerStream.ReadLine();
					if (noteCreationTime == null || noteBody == null || noteIsTemporal == null)
					{
						Log.Logger.Error($"Ошибка разметки файла заметки. Идентификатор заметки: {Id}.");
						throw new ArgumentNullException();
					}

					return new Note(
						Id,
						DateTime.Parse(noteCreationTime),
						noteBody,
						bool.Parse(noteIsTemporal)
					);
				}

			return null;
		}

		public List<Note> GetAllNotes()
		{
			string[] filelist;
			try
			{
				filelist = _isoStore.GetFileNames("notes/");
			}
			catch (Exception ex)
			{
				Log.Logger.Error("Папка для заметок в защищённом хранилище не найдена.");
				throw new NoteGetException(
					"NoteGet: Папка для заметок в защищённом хранилище не найдена.",
					ex
				);
			}

			List<Note> noteList = new();
			foreach (string fileName in filelist)
			{
				var note = GetNote(Guid.Parse(fileName.Replace(".txt", "")));
				noteList.Add(note!);
			}

			return noteList;
		}

		public List<Note> GetNotesByQuery(QueryStringParameters param)
		{
			return GetAllNotes()
				.Skip((param.PageNumber-1) * param.PageSize)
				.Take(param.PageSize)
				.ToList();
		}
	}
}
