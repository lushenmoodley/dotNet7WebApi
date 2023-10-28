﻿using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MagicVilla_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto obj = new();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Login(LoginRequestDto obj)
        {
            APIResponse response=await _authService.LoginAsync<APIResponse>(obj);   
            if(response!=null && response.IsSuccess)
            {
                LoginResponseDTO model=JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                identity.AddClaim(new Claim(ClaimTypes.Name, model.User.Username));
                identity.AddClaim(new Claim(ClaimTypes.Role, model.User.Role));

                var principle = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);

                HttpContext.Session.SetString(SD.SessionToken,model.Token);
                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError("Custom Error", response.ErrorMessages.FirstOrDefault());
                return View(obj);
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegisterationRequestDTO obj = new();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        {

            APIResponse result=await _authService.RegisterAsync<APIResponse>(obj);

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction("Login");
            }

                return View();
        }

        public async Task<IActionResult> Logout(RegisterationRequestDTO obj)
        {

            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken,"");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}

