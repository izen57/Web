using Model;

namespace Logic
{
	public interface INoteService
	{
		void Create(Note note);
		void Edit(Note note);
		void Delete(Guid id);
		List<Note> GetAllNotesList();
		Note? GetNote(Guid guid);
	}
}
