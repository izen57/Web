using Exceptions.NoteExceptions;

using Logic;

using Microsoft.AspNetCore.Mvc;

using Model;

using WebAPI.DataTransferObject;

namespace WebAPI.Controllers
{
	[ApiController]
	[Produces("application/json")]
	public class NotesController: ControllerBase
	{
		private readonly INoteService _noteService;

		public NotesController(INoteService noteService)
		{
			_noteService = noteService;
		}

		/// <summary>
		/// Возвращает информацию о всех заметках
		/// </summary>
		/// <returns>Список всех заметок</returns>
		/// <param name="param">Номер и размер страницы</param>
		/// <respons code="200">Заметки успешно возвращены</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[HttpGet("api/v1/notes")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NoteDTO>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult GetAllNotes([FromQuery] QueryStringParameters param)
		{
			List<Note> list = _noteService.GetNotesByQuery(param);

			List<NoteDTO> listDTO = new();
			foreach (Note note in list)
				listDTO.Append(NoteDTO.ToDTO(note));

			return new OkObjectResult(list);
		}

		/// <summary>
		/// Обновляет существующую заметку
		/// </summary>
		/// <returns>Изменённая заметка</returns>
		/// <param name="noteDTO">Изменяемая заметка</param>
		/// <param name="Id">Идентификатор заметки</param>
		/// <respons code="200">Существующая заметка изменена</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Заметка не найдена</respons>
		[HttpPatch("api/v1/notes/{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult EditNote([FromBody] NoteDTO noteDTO, [FromRoute] Guid Id)
		{
			try
			{
				Note note = new(Id, noteDTO.CreationTime, noteDTO.Body, noteDTO.IsTemporal);
				noteDTO = NoteDTO.ToDTO(_noteService.Edit(note));
				return new OkObjectResult(noteDTO);
			}
			catch (NoteEditException)
			{
				return new NotFoundResult();
			}
			catch
			{
				return new BadRequestResult();
			}
		}

		/// <summary>
		/// Создаёт новую заметку
		/// </summary>
		/// <param name="noteDTO">Новая заметка</param>
		/// <respons code="201">Заметка успешно создана</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="500">Заметка с таким идентификатором уже существует</respons>
		[HttpPost("api/v1/notes")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NoteDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult CreateNote([FromBody] NoteDTO noteDTO)
		{
			try
			{
				Note note = new(Guid.NewGuid(), noteDTO.CreationTime, noteDTO.Body, noteDTO.IsTemporal);
				noteDTO = NoteDTO.ToDTO(_noteService.Create(note));
			}
			catch (NoteCreateException)
			{
				return StatusCode(500);
			}
			return new CreatedResult("Notes isolated storage", noteDTO);
		}

		/// <summary>
		/// Возвращает заметку по её идентификатору
		/// </summary>
		/// <param name="Id">Идентификатор искомой заметки</param>
		/// <respons code="200">Заметка найдена</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Заметка не найдена</respons>
		[HttpGet("api/v1/notes/{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetNote([FromRoute] Guid Id)
		{
			NoteDTO? note;
			try
			{
				note = NoteDTO.ToDTO(_noteService.GetNote(Id));
			}
			catch (NoteGetException)
			{
				return new NotFoundResult();
			}
			catch
			{
				return StatusCode(500);
			}
			return new OkObjectResult(note);
		}

		/// <summary>
		/// Удаляет заметку по её идентификатору
		/// </summary>
		/// <param name="Id">Идентификатор искомой заметки</param>
		/// <respons code="200">Заметка успешно удалена</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Заметка не найдена</respons>
		[HttpDelete("api/v1/notes/{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteNote([FromRoute] Guid Id)
		{
			try
			{
				_noteService.Delete(Id);
			}
			catch (NoteDeleteException)
			{
				return new NotFoundResult();
			}
			catch
			{
				return StatusCode(500);
			}
			return new OkResult();
		}
	}
}
