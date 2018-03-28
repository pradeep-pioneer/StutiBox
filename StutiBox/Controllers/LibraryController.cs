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
		[HttpGet]
        [Route("")]
        [Route("List")]
        public IActionResult List()
        {
            return Ok(DependencyActor.Container.Resolve<ILibraryActor>().LibraryItems);
        }

        [HttpPost]
        [Route("Search")]
        public IActionResult Search([FromBody]string[] keyWords)
        {
            var result =  DependencyActor.Container.Resolve<ILibraryActor>().Find(keyWords);
            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        [Route("QuickSearch")]
        public IActionResult QuickSearch([FromBody]string[] keyWords)
        {
            var result = DependencyActor.Container.Resolve<ILibraryActor>().LuckySearch(keyWords);
            if (result != null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            var result = (DependencyActor.Container.Resolve<ILibraryActor>() as LibraryActor)[id];
            if (result != null)
                return Ok(result);
            else
                return NotFound();
        }

    }
}
