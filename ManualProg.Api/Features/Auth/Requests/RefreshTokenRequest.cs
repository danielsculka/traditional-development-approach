﻿namespace ManualProg.Api.Features.Auth.Requests;

public record RefreshTokenRequest
{
    public required string RefreshToken { get; set; }
}
