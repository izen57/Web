namespace WebAPI.DataTransferObject
{
	public class UserDTOCreate
	{
		public string Name { get; set; }
		public string Password { get; set; }

		public UserDTOCreate(string name, string password)
		{
			Name = name;
			Password = password;
		}
	}
}
