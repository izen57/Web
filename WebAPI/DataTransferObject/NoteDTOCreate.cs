using Model;

namespace WebAPI.DataTransferObject
{
	public class NoteDTOCreate
	{
		public string Body { get; set; }
		public bool IsTemporal { get; set; }
		 
		public NoteDTOCreate() { }

		public NoteDTOCreate(string body, bool isTemporal)
		{
			Body = body;
			IsTemporal = isTemporal;
		}
	}
}
