using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pastebin.Web.Models;
using Pastebin.Web.Services;

namespace Pastebin.Web.Controllers
{
    public class SnippetController : Controller
    {
        private readonly ILogger<SnippetController> _logger;
        private readonly ISnippetService _snippetService;

        public SnippetController(ILogger<SnippetController> logger, ISnippetService snippetService)
        {
            _logger = logger;
            _snippetService = snippetService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SnippetCreateModel snippetCreateModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var snippetModel = await _snippetService.CreateSnippetAsync(snippetCreateModel);

            return RedirectToAction("View", "Snippet", new {id = snippetModel.Id});
        }

        public async Task<IActionResult> View(Guid id)
        {
            var snippetModel = await _snippetService.GetSnippetById(id);

            if (snippetModel is null)
            {
                return View("Error404");
            }

            return View(snippetModel);
        }

        public IActionResult Error404()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}