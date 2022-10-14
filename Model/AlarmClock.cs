using System.Drawing;

namespace Model {
	public class AlarmClock {
		public DateTime AlarmTime { get; set; }
		public string Name { get; set; }
		public Color AlarmClockColor { get; set; }
		public bool IsWorking { get; set; }

		public AlarmClock(DateTime alarmTime, string name, Color alarmClockColor, bool isWorking) {
			AlarmTime = alarmTime;
			Name = name;
			AlarmClockColor = alarmClockColor;
			IsWorking = isWorking;
		}
	}
}