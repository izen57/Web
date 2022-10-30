using System.ComponentModel.DataAnnotations;

namespace Model {
	public class Note {
		[Required]
		public Guid Id { get; private set; }
		[Required]
		public DateTime CreationTime { get; set; }
		[Required]
		public string Body { get; set; }
		[Required]
		public bool IsTemporal { get; set; }

		public Note()
		{
		}

		public Note(Guid id, string body, bool isTemporal)
		{
			Id = id;
			CreationTime = DateTime.Now;
			Body = body;
			IsTemporal = isTemporal;
		}

		public Note(Guid id, DateTime creationTime, string body, bool isTemporal) {
			Id = id;
			CreationTime = creationTime;
			Body = body;
			IsTemporal = isTemporal;
		}

		public TimeSpan RemovalTime() {
			return DateTime.Now - CreationTime;
		}
	}
}
