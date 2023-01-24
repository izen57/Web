namespace Model
{
	public class QueryStringParameters
	{
		const int maxPageSize = 50;
		int _pageSize = 10;
		public int PageNumber { get; set; } = 1;
		public int PageSize
		{
			get => _pageSize;
			set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
		}
	}
}
