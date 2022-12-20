using Repositories;
using RepositoriesImplementations;
using Logic;
using Model;

namespace Tests
{
	[TestClass]
	public class NoteWithoutMoqTests
	{
		[TestMethod]
		public void NoteTest()
		{
			INoteRepo notesRepo = new NoteFileRepo();
			INoteService notesService = new NoteService(notesRepo);
			var id = Guid.NewGuid();
			
			Note check1 = new(id, "test1", false);
			notesService.Create(check1);
			Assert.IsNotNull(check1, "NoteCreate");

			Note check2 = new(id, "changed body", false);
			notesService.Edit(check2);
			Assert.IsNotNull(check2, "NoteChange");

			notesService.Delete(id);
			Assert.AreEqual(0, notesService.GetAllNotesList().Count, "AlarmClockDelete");
		}
	}
}