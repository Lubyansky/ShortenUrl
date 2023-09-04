﻿using Bitbucket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace Bitbucket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            using BitbucketContext db = new();
            IEnumerable<Url> urlList = await db.Urls.ToListAsync();

            Url url = new();

            UrlViewModel viewModel = new(urlList, url);

            return View(viewModel);
        }

        public async Task<IActionResult> Shorten(Url url)
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(12)
                .ToList().ForEach(e => builder.Append(e));

            url.Token = builder.ToString();
            url.UserId = 1;

            using BitbucketContext db = new();
            await db.Urls.AddAsync(url);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}