using Model;

namespace Logic
{
	public interface INoteService
	{
		Note Create(Note note);
		Note Edit(Note note);
		void Delete(Guid id, Guid ownerId);
		Note? GetNote(Guid guid);
		List<Note> GetNotes(Guid ownerId);
		List<Note> GetNotes(Guid ownerId, QueryStringParameters param);
	}
}
