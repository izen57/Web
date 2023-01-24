using System.ComponentModel.DataAnnotations;

namespace Model
{
	public class Note
	{
		[Required]
		public Guid Id { get; private set; }
		[Required]
		public DateTime CreationTime { get; set; }
		[Required]
		public string Body { get; set; }
		[Required]
		public Guid OwnerId { get; private set; }
		[Required]
		public bool IsTemporal { get; set; }

		public Note()
		{
		}

		public Note(Guid id, string body, bool isTemporal, Guid ownerId)
		{
			Id = id;
			CreationTime = DateTime.Now;
			Body = body;
			IsTemporal = isTemporal;
			OwnerId = ownerId;
		}

		public Note(Guid id, DateTime creationTime, string body, bool isTemporal, Guid ownerId)
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
	}
}
