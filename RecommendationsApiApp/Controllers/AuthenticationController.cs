﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ToDoApi.v1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthenticationController(IConfiguration config)
    {
        _config = config;
    }

    public record AuthenticationData(string? UserName, string? Password);
    public record UserData(int Id, string FirstName, string LastName, string UserName);

    [HttpPost("token")]
    [AllowAnonymous]
    [ApiVersionNeutral]
    public ActionResult<string> Authenticate([FromBody] AuthenticationData data)
    {
        var user = ValidateCredentials(data);

        if (user is null)
        {
            return Unauthorized();
        }

        var token = GenerateToken(user);

        return Ok(token);
    }

    private string GenerateToken(UserData user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetValue<string>("Authentication:SecretKey")));

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new();
        claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName.ToString()));
        claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName.ToString()));
        claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName.ToString()));

        var token = new JwtSecurityToken(_config.GetValue<string>("Authentication:Issuer"),
                                        _config.GetValue<string>("Authentication:Audience"),
                                        claims,
                                        DateTime.UtcNow,
                                        DateTime.UtcNow.AddMinutes(1),
                                        signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserData? ValidateCredentials(AuthenticationData data)
    {
        // This is not production code - repleace this with call to chosen third part auth system!
        if (CompareValues(data.UserName, "bellison") && CompareValues(data.Password, "password"))
        {
            return new UserData(1, "Bryn", "Ellison", data.UserName!);
        }

        return null;
    }

    private bool CompareValues(string? actual, string? expected)
    {
        if (actual is not null)
        {
            if (actual.Equals(expected))
            {
                return true;
            }
        }
        return false;
    }
}

