using Model;

using System.Drawing;

namespace WebAPI.DataTransferObject
{
	public class AlarmClockDTO
	{
		public Guid Id { get; set; }
		public DateTime AlarmTime { get; set; }
		public Guid OwnerId { get; set; }
		public string Name { get; set; }
		public string AlarmClockColor { get; set; }
		public bool IsWorking { get; set; }

		public AlarmClockDTO() { }

		public AlarmClockDTO(Guid id, DateTime alarmTime, Guid ownerId, string name, string alarmClockColor, bool isWorking)
		{
			Id = id;
			AlarmTime = alarmTime;
			OwnerId = ownerId;
			Name = name;
			AlarmClockColor = alarmClockColor;
			IsWorking = isWorking;
		}

		public static AlarmClockDTO ToDTO(AlarmClock alarmClock)
		{ 
			return new AlarmClockDTO(
				alarmClock.Id,
				alarmClock.AlarmTime,
				alarmClock.OwnerId,
				alarmClock.Name,
				alarmClock.AlarmClockColor.Name,
				alarmClock.IsWorking
			);
		}

		public static AlarmClock FromDTO(AlarmClockDTO alarmClockDTO)
		{
			return new AlarmClock(
				alarmClockDTO.Id,
				alarmClockDTO.AlarmTime,
				alarmClockDTO.Name,
				alarmClockDTO.OwnerId,
				Color.FromName(alarmClockDTO.AlarmClockColor),
				alarmClockDTO.IsWorking
			);
		}
	}
}
