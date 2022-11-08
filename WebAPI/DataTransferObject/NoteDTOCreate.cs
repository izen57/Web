using Model;

namespace WebAPI.DataTransferObject
{
	public class NoteDTOCreate
	{
		public DateTime CreationTime { get; set; }
		public string Body { get; set; }
		public bool IsTemporal { get; set; }
		 
		public NoteDTOCreate() { }

		public NoteDTOCreate(string body, bool isTemporal)
		{
			CreationTime = DateTime.Now;
			Body = body;
			IsTemporal = isTemporal;
		}

		public NoteDTOCreate(DateTime creationTime, string body, bool isTemporal)
		{
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

		public static Note FromDTO(NoteDTO noteDTO)
		{
			return new Note(
				noteDTO.Id,
				noteDTO.CreationTime,
				noteDTO.Body,
				noteDTO.IsTemporal
			);
		}
	}
}
