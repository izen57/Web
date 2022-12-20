using Model;

namespace Repositories {
	public interface INoteRepo {
		Note Create(Note note);
		Note Edit(Note note);
		void Delete(Guid id);
		Note? GetNote(Guid id);
		List<Note> GetAllNotes();
		List<Note> GetNotesByQuery(QueryStringParameters param);
	}
}
