using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper){
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("register")] // must feed in an object to have the request body read properly
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){

            if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken"); //BadRequest is part of <ActionResult>

            // go from RegisterDto to an AppUser
            var user = _mapper.Map<AppUser>(registerDto);

            // using var hmac = new HMACSHA512(); //using statement ensures "Dispose" method is called to release the unmanaged resources of the class

            user.UserName = registerDto.UserName.ToLower();
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            // user.PasswordSalt = hmac.Key;

            // _context.Users.Add(user);
            // await _context.SaveChangesAsync();

            // creates our user and saves the changes into the database
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto{
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
            // _context.Users.FindAsync = used for finding based on primary key
            // _context.Users.FirstOrDefaultAsync
            // _context.Users.SingleOrDefaultAsync same as FirstOrDefault, but throws an exception if there is more than one element in the sequence
            // instead of _context, we now get our Users table via the _userManager
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!result.Succeeded) return Unauthorized();

            // using var hmac = new HMACSHA512(user.PasswordSalt);
            // computedHash will be calculated using the PasswordSalt
            // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            // for(int i = 0; i < computedHash.Length; i++){
            //     if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            // }
            return new UserDto{
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            };
        }

        private async Task<bool> UserExists(string username){
            // change _context to _userManager
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}