using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StutiBox.Actors;
using StutiBox.Models;
using Autofac;
namespace StutiBox.Controllers
{
    [Route("api/[controller]")]
    public class LibraryController : Controller
    {
		private IPlayerActor playerActor;

		public LibraryController():this(DependencyActor.Container.Resolve<IPlayerActor>()){}

		public LibraryController(IPlayerActor player)
		{
			playerActor = player;
		}
		[HttpGet]
        [Route("")]
        [Route("List")]
        public IActionResult List()
        {
			return Ok(new { Status = true, LibraryRefreshedAt = playerActor.LibraryActor.RefreshedAt, Items = playerActor.LibraryActor.LibraryItems });
        }

        [HttpPost]
        [Route("Search")]
        public IActionResult Search([FromBody]string[] keyWords)
        {
			var result = new { Status = true, LibraryRefreshedAt = playerActor.LibraryActor.RefreshedAt, Items = playerActor.LibraryActor.Find(keyWords) }
            if (result.Items.Count > 0)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        [Route("QuickSearch")]
        public IActionResult QuickSearch([FromBody]string[] keyWords)
        {
			var result = new { Status = true, LibraryRefreshedAt = playerActor.LibraryActor.RefreshedAt, Items = playerActor.LibraryActor.LuckySearch(keyWords) }
            if (result != null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
			var result = new { Status = true, LibraryRefreshedAt = playerActor.LibraryActor.RefreshedAt, Item = playerActor.LibraryActor.GetItem(id) }
            if (result.Item != null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("Refresh")]
        public IActionResult Refresh(bool stopPlayer=false)
		{
			if(stopPlayer)
			{
				if (playerActor.PlaybackState == PlaybackState.Playing || playerActor.PlaybackState == PlaybackState.Paused) playerActor.Stop();
			}
			bool result = playerActor.LibraryActor.Refresh();
			var message = result ? $"Library Refreshed!" : "Operation Failed - check status field for details!";
			var status = new { PlayerState = playerActor.PlaybackState, LibraryItemsCount = playerActor.LibraryActor.LibraryItems.Count };
			var response = new { Status = result, Message = message, Items = playerActor.LibraryActor.LibraryItems };
			return Ok(response);
		}

    }
}
