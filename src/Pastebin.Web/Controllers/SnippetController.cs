using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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

            var snippetModel = await _snippetService.CreateAsync(snippetCreateModel);

            return RedirectToAction("View", "Snippet", new {id = snippetModel.Id});
        }

        public async Task<IActionResult> View(Guid id)
        {
            var snippetModel = await _snippetService.GetByIdAsync(id);

            if (snippetModel is null)
            {
                return NotFound();
            }

            return View(snippetModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string id)
        {
            // TODO: Show status code and specific error information
            // TODO: Tidy up ErrorView
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}