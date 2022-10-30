using Logic;

using Microsoft.AspNetCore.Mvc;

using System.Drawing;

using WebAPI.DataTransferObject;

namespace WebAPI.Controllers
{
	[ApiController]
	[Produces("application/json")]
	public class StopwatchController
	{
		private readonly IStopwatchService _stopwatchService;

		public StopwatchController(IStopwatchService stopwatchService)
		{
			_stopwatchService = stopwatchService;
		}

		/// <summary>
		/// Возращает секундомер
		/// </summary>
		/// <returns>Cекундомер</returns>
		/// <remarks cref="StopwatchDTO"></remarks>
		/// <respons code="200">Секундомер найден</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[HttpGet("api/v1/stopwatch")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StopwatchDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetStopwatch()
		{
			return new OkObjectResult(StopwatchDTO.ToDTO(_stopwatchService.Get()));
		}

		//нужно доделать!!!
		/// <summary>
		/// Изменяет параметр секундомера
		/// </summary>
		/// <returns>Изменённый секундомер</returns>
		/// <param name="stopwatchDTO">Параметр секундомера</param>
		/// <respons code="200">Секундомер запущен</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[HttpPatch("api/v1/stopwatch")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StopwatchDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult StopwatchAction([FromBody] object stopwatchDTO)
		{
			switch (stopwatchDTO.GetType().Name)
			{
				case null:
					return new BadRequestResult();
				case "System.String":
					_stopwatchService.EditName(stopwatchDTO.ToString()!);
					break;
				case "System.Color":
					_stopwatchService.EditColor(Color.FromName(stopwatchDTO.ToString()!));
					break;
				case "System.Boolean" when stopwatchDTO.ToString() == "true":
					_stopwatchService.Set();
					break;
				case "System.Boolean" when stopwatchDTO.ToString() == "false":
					if (_stopwatchService.Get().IsWorking == false)
						_stopwatchService.Reset();
					else
						_stopwatchService.Stop();
					break;
				case "System.DateTime":
					_stopwatchService.AddStopwatchFlag();
					break;
			}

			return new OkObjectResult(_stopwatchService.Get());
		}
	}
}
