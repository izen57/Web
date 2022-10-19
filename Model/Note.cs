namespace Model {
	public class Note {
		public Guid Id { get; private set; }
		public DateTime CreationTime { get; set; }
		public string Body { get; set; }
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
