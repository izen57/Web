using Model;

namespace WebAPI.DataTransferObject
{
	public class NoteDTO
	{
		public Guid Id { get; private set; }
		public DateTime CreationTime { get; set; }
		public string Body { get; set; }
		public bool IsTemporal { get; set; }

		public NoteDTO() { }

		public NoteDTO(Guid id, string body, bool isTemporal)
		{
			Id = id;
			CreationTime = DateTime.Now;
			Body = body;
			IsTemporal = isTemporal;
		}

		public NoteDTO(Guid id, DateTime creationTime, string body, bool isTemporal)
		{
			Id = id;
			CreationTime = creationTime;
			Body = body;
			IsTemporal = isTemporal;
		}

		public TimeSpan RemovalTime()
		{
			return DateTime.Now - CreationTime;
		}

		public static NoteDTO ToDTO(Note note)
		{ 
			return new NoteDTO(
				note.Id,
				note.CreationTime,
				note.Body,
				note.IsTemporal
			);
		}

		public static Note FromDTO(NoteDTO noteDTO, Guid ownerId)
		{
			return new Note(
				noteDTO.Id,
				noteDTO.CreationTime,
				noteDTO.Body,
				noteDTO.IsTemporal,
				ownerId
			);
		}
	}
}
