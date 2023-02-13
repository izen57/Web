namespace WebAPI.DataTransferObject
{
	public class UserDTOResponse
	{
		public string Name { get; set; }
		public string Password { get; set; }
		public readonly string _token;

		public UserDTOResponse(string name, string password, string token)
		{
			Name = name;
			Password = password;
			_token = token;
		}
	}
}
