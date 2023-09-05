﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Bitbucket.Data;
using Bitbucket.Models.Account;
using Bitbucket.Services.Account;
using System.ComponentModel.DataAnnotations;

namespace Bitbucket.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AccountService _service;
        public AccountController(ILogger<AccountController> logger, AccountService service)
        {
            _logger = logger;
            _service = service;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel Model)
        {
            if (ModelState.IsValid)
            {
                Boolean UserIsExist = await _service.AuthenticateUser(Model);

                if (UserIsExist)
                {
                    await Authenticate(Model);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(Model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel Model)
        {
            if (ModelState.IsValid)
            {
                Boolean UserIsExist = await _service.FindUser(Model);
                if (!UserIsExist)
                {
                    await _service.CreateUser(Model);

                    await Authenticate(Model);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(Model);
        }

        private async Task Authenticate(BaseAuthModel Model)
        {
            ClaimsIdentity Id = _service.SetClaimIdentity(Model);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(Id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}