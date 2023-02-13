using System.Drawing;

namespace Model
{
	public class AlarmClock
	{
		public Guid Id { get; set; }
		public DateTime AlarmTime { get; set; }
		public string Name { get; set; }
		public Guid OwnerId { get; set; }
		public Color AlarmClockColor { get; set; }
		public bool IsWorking { get; set; }

		public AlarmClock() { }

		public AlarmClock(Guid id, DateTime alarmTime, string name, Guid ownerId, Color alarmClockColor, bool isWorking)
		{
			Id = id;
			AlarmTime = alarmTime;
			Name = name;
			AlarmClockColor = alarmClockColor;
			IsWorking = isWorking;
			OwnerId = ownerId;
		}
	}
}