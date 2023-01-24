using Model;

namespace Logic
{
	public interface INoteService
	{
		Note Create(Note note);
		Note Edit(Note note);
		void Delete(Guid id);
		List<Note> GetAllNotes();
		Note? GetNote(Guid guid);
		List<Note> GetNotesByQuery(QueryStringParameters param);
	}
}
