﻿using FluentValidation;

namespace infrastructure;

public class AppOptionsValidator : AbstractValidator<AppOptions>
{
    public AppOptionsValidator()
    {
        RuleFor(x => x.DbConnectionString).NotEmpty();
    }
}