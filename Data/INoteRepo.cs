using Model;

namespace Repositories
{
	public interface INoteRepo
	{
		Note Create(Note note);
		Note Edit(Note note);
		void Delete(Guid guid);
		Note? GetNote(Guid guid, Guid ownerId);
		List<Note> GetNotes(Guid ownerId);
		List<Note> GetNotesByQuery(QueryStringParameters param, Guid ownerId);
	}
}
