using Model;

namespace Logic
{
	public interface INoteService
	{
		Note Create(Note note);
		Note Edit(Note note);
		void Delete(Guid id);
		List<Note> GetAllNotesList();
		Note? GetNote(Guid guid);
	}
}
