using Repositories;
using Model;
using Serilog;
using System.Timers;

namespace Logic {
	public class NoteService: INoteService {
		INoteRepo _repository;
		System.Timers.Timer _checkForTime;

		public NoteService(INoteRepo repo) {
			Log.Logger = new LoggerConfiguration()
				.WriteTo.File("LogNote.txt")
				.CreateLogger();

			_repository = repo ?? throw new ArgumentNullException(nameof(repo));

			_checkForTime = new(60 * 1000);
			_checkForTime.Elapsed += new ElapsedEventHandler(AutoDelete);
			_checkForTime.Enabled = true;
		}

		public Note Create(Note note) {
			return _repository.Create(note);
		}

		public Note Edit(Note note) {
			return _repository.Edit(note);
		}

		public void Delete(Guid id) {
			_repository.Delete(id);
		}

		public List<Note> GetAllNotesList() {
			return _repository.GetAllNotes();
		}

		public Note? GetNote(Guid guid)
		{
			return _repository.GetNote(guid);
		}

		private void AutoDelete(object sender, ElapsedEventArgs e)
		{
			foreach (Note note in GetAllNotesList())
				if (note.IsTemporal == true && DateTime.Now - note.CreationTime >= TimeSpan.FromDays(1))
				{
					Log.Logger.Information($"Заметка удалена автоматически по истечении срока. Идентификатор заметки: {note.Id}.");
					_repository.Delete(note.Id);
				}
		}

		public List<Note> GetNotesByQuery(QueryStringParameters param)
		{
			return _repository.GetNotesByQuery(param);
		}
	}
}
