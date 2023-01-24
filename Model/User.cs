namespace Model
{
	public class User
	{
		public Guid Id { get; private set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public List<AlarmClock> UserAlarmClocks { get; private set; }
		public List<Note> UserNotes { get; private set; }
		public Stopwatch UserStopwatch { get; private set; }

		public User(Guid id, string name, string password, List<AlarmClock> userAlarmClocks, List<Note> userNotes)
		{
			Id = id;
			Name = name;
			Password = password;
			UserAlarmClocks = userAlarmClocks;
			UserNotes = userNotes;
		}
	}
}
