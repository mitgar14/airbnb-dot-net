using FluentValidation;
using Usuarios.API.DTOs;
using Airbnb.Common.Constants;

namespace Usuarios.API.Validators;

public class CreateUsuarioDtoValidator : AbstractValidator<CreateUsuarioDto>
{
    public CreateUsuarioDtoValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("El ID de usuario debe ser mayor que 0");

        ApplyNameRules();
        ApplyEmailRules();
        ApplyPasswordRules();
        ApplyRoleRules();
    }

    protected void ApplyNameRules()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(255).WithMessage("El nombre no puede exceder 255 caracteres");
    }

    protected void ApplyEmailRules()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido")
            .MaximumLength(200).WithMessage("El email no puede exceder 200 caracteres");
    }

    protected void ApplyPasswordRules()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
    }

    protected void ApplyRoleRules()
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("El rol es requerido")
            .Must(UserRoles.IsValidRole)
            .WithMessage($"El rol debe ser uno de los siguientes: {string.Join(", ", UserRoles.GetAllRoles())}");
    }
}
