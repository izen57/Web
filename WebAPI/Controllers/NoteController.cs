using Exceptions.NoteExceptions;

using Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Model;

using WebAPI.DataTransferObject;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/v1/notes")]
	[Produces("application/json")]
	public class NoteController: ControllerBase
	{
		private readonly INoteService _noteService;

		public NoteController(INoteService noteService)
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
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		[Authorize]
		[HttpGet("")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NoteDTO>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public ActionResult GetNotes([FromQuery] QueryStringParameters param)
		{
			Guid userFromResponse = Guid.Parse(HttpContext.Items["User ID"].ToString());
			List<Note> list = _noteService.GetNotes(userFromResponse, param);

			List<NoteDTO> listDTO = new();
			if (listDTO.Count > 0)
				foreach (Note note in list)
					listDTO.Add(NoteDTO.ToDTO(note));

			return new OkObjectResult(list);
		}

		/// <summary>
		/// Обновляет существующую заметку
		/// </summary>
		/// <returns>Изменённая заметка</returns>
		/// <param name="noteDTOCreate">Изменяемая заметка</param>
		/// <param name="Id">Идентификатор заметки</param>
		/// <respons code="200">Существующая заметка изменена</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		/// <respons code="404">Заметка не найдена</respons>
		[Authorize]
		[HttpPut("{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult EditNote([FromBody] NoteDTOCreate noteDTOCreate, [FromRoute] Guid Id)
		{
			Note? note = _noteService.GetNote(Id);
			if (note == null)
				return new NotFoundResult();

			note.Body = noteDTOCreate.Body;
			note.IsTemporal = noteDTOCreate.IsTemporal;

			try
			{
				note = _noteService.Edit(note);
			}
			catch (NoteEditException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
			}
			catch (NoteEditException)
			{
				return new NotFoundResult();
			}
			catch
			{
				return new BadRequestResult();
			}
			return new OkObjectResult(NoteDTO.ToDTO(note));
		}

		/// <summary>
		/// Создаёт новую заметку
		/// </summary>
		/// <param name="noteDTOCreate">Новая заметка</param>
		/// <returns>Созданная заметка</returns>
		/// <respons code="201">Заметка успешно создана</respons>
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		[Authorize]
		[HttpPost("")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NoteDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult CreateNote([FromBody] NoteDTOCreate noteDTOCreate)
		{
			NoteDTO noteDTO;
			Guid userFromResponse = Guid.Parse(HttpContext.Items["User ID"].ToString());
			try
			{
				Note note = new(
					Guid.NewGuid(),
					noteDTOCreate.Body,
					noteDTOCreate.IsTemporal,
					userFromResponse
				);
				noteDTO = NoteDTO.ToDTO(_noteService.Create(note));
			}
			catch (NoteCreateException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
			}
			catch (NoteCreateException e) when (e.InnerException.Message.Contains("already exists"))
			{
				return StatusCode(500);
			}
			return new CreatedResult("Notes isolated storage", noteDTO);
		}

		/// <summary>
		/// Возвращает заметку по её идентификатору
		/// </summary>
		/// <param name="Id">Идентификатор искомой заметки</param>
		/// <returns>Искомая заметка</returns>
		/// <respons code="200">Заметка найдена</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="404">Заметка не найдена</respons>
		[Authorize]
		[HttpGet("{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		/// <respons code="404">Заметка не найдена</respons>
		[Authorize]
		[HttpDelete("{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteNote([FromRoute] Guid Id)
		{
			Guid userFromResponse = Guid.Parse(HttpContext.Items["User ID"].ToString());

			try
			{
				_noteService.Delete(Id, userFromResponse);
			}
			catch (NoteDeleteException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
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
