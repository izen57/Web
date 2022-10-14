using Model;

namespace Repositories {
	public interface INoteRepo {
		void Create(Note note);
		void Edit(Note note);
		void Delete(Guid id);
		Note? GetNote(Guid id);
		List<Note> GetAllNotesList();
	}
}
