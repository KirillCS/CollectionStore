using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollectionStore.Controllers
{
    public class ProfileController : Controller
    {
        [HttpGet]
        public IActionResult Index(string id = null)
        {
            if(id == null)
            {
            }
            return View();
        }
    }
}
