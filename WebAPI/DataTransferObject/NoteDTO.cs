using Model;

namespace WebAPI.DataTransferObject
{
	public class NoteDTO
	{
		public Guid Id { get; private set; }
		public Guid OwnerId { get; private set; }
		public DateTime CreationTime { get; set; }
		public string Body { get; set; }
		public bool IsTemporal { get; set; }

		public NoteDTO() { }

		public NoteDTO(Guid id, string body, bool isTemporal, Guid ownerId)
		{
			Id = id;
			CreationTime = DateTime.Now;
			Body = body;
			IsTemporal = isTemporal;
			OwnerId = ownerId;
		}

		public NoteDTO(Guid id, DateTime creationTime, string body, bool isTemporal, Guid ownerId)
		{
			Id = id;
			CreationTime = creationTime;
			Body = body;
			IsTemporal = isTemporal;
			OwnerId = ownerId;
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
				note.IsTemporal,
				note.OwnerId
			);
		}

		public static Note FromDTO(NoteDTO noteDTO)
		{
			return new Note(
				noteDTO.Id,
				noteDTO.CreationTime,
				noteDTO.Body,
				noteDTO.IsTemporal,
				noteDTO.OwnerId
			);
		}
	}
}
