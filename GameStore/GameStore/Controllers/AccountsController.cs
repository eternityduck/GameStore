﻿using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Models;
using GameStore.JwtFeatures;
using GameStore.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
        
        public AccountsController(UserManager<User> userManager, IMapper mapper, JwtHandler jwtHandler) 
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
        }
        
        [HttpPost("Registration")] 
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userForRegistration) 
        {
            
            if (userForRegistration == null || !ModelState.IsValid) 
                return BadRequest(); 
            
            var user = _mapper.Map<User>(userForRegistration);
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (result.Succeeded) return StatusCode(201);
            
            var errors = result.Errors.Select(e => e.Description); 
                
            return BadRequest(new UserRegistrationResponseDto() { Errors = errors });

        }
        
        
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.Email);
            
            if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });
            
            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            
            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }
    }
}