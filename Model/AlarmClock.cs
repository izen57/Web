using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Model {
	public class AlarmClock {
		[Required]
		public DateTime AlarmTime { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public Color AlarmClockColor { get; set; }
		[Required]
		public bool IsWorking { get; set; }

		public AlarmClock()
		{
		}

		public AlarmClock(DateTime alarmTime, string name, Color alarmClockColor, bool isWorking) {
			AlarmTime = alarmTime;
			Name = name;
			AlarmClockColor = alarmClockColor;
			IsWorking = isWorking;
		}
	}
}