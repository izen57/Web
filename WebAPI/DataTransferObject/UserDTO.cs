using Model;

namespace WebAPI.DataTransferObject
{
	public class UserDTO
	{
		public Guid Id { get; private set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public string Token { get; set; }
		public List<AlarmClockDTO> UserAlarmClocks { get; private set; }
		public List<NoteDTO> UserNotes { get; private set; }

		public UserDTO(Guid id, string name, string password, string token, List<AlarmClockDTO> userAlarmClocks, List<NoteDTO> userNotes)
		{
			Id = id;
			Name = name;
			Password = password;
			Token = token;
			UserAlarmClocks = userAlarmClocks;
			UserNotes = userNotes;
		}

		public static UserDTO ToDTO(User user, string token)
		{
			List<AlarmClockDTO> alarmClockDTOs = new();
			foreach (AlarmClock alarmClock in user.UserAlarmClocks)
				alarmClockDTOs.Add(AlarmClockDTO.ToDTO(alarmClock));

			List<NoteDTO> noteDTOs = new();
			foreach (Note note in user.UserNotes)
				noteDTOs.Add(NoteDTO.ToDTO(note));

			return new UserDTO(
				user.Id,
				user.Name,
				user.Password,
				token,
				alarmClockDTOs,
				noteDTOs
			);
		}

		public static User FromDTO(UserDTO userDTO)
		{
			List<AlarmClock> alarmClocks = new();
			foreach (AlarmClockDTO alarmClockDTO in userDTO.UserAlarmClocks)
				alarmClocks.Add(AlarmClockDTO.FromDTO(alarmClockDTO, userDTO.Id));

			List<Note> notes = new();
			foreach (NoteDTO noteDTO in userDTO.UserNotes)
				notes.Add(NoteDTO.FromDTO(noteDTO, userDTO.Id));

			return new User(
				userDTO.Id,
				userDTO.Name,
				userDTO.Password,
				alarmClocks,
				notes
			);
		}
	}
}
