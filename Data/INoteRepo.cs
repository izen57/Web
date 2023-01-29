using Model;

namespace Repositories
{
	public interface INoteRepo
	{
		Note Create(Note note);
		Note Edit(Note note);
		void Delete(Guid guid, Guid ownerId);
		Note? GetNote(Guid guid);
		List<Note> GetNotes();
		List<Note> GetNotes(Guid ownerId);
		List<Note> GetNotes(Guid ownerId, QueryStringParameters param);
	}
}
