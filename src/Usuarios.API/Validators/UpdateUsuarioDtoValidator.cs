using FluentValidation;
using Usuarios.API.DTOs;
using Airbnb.Common.Constants;

namespace Usuarios.API.Validators;

public class UpdateUsuarioDtoValidator : AbstractValidator<UpdateUsuarioDto>
{
    public UpdateUsuarioDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre no puede estar vacío")
            .MaximumLength(255).WithMessage("El nombre no puede exceder 255 caracteres")
            .When(x => x.Name != null);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email no puede estar vacío")
            .EmailAddress().WithMessage("El email no es válido")
            .MaximumLength(200).WithMessage("El email no puede exceder 200 caracteres")
            .When(x => x.Email != null);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña no puede estar vacía")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
            .When(x => x.Password != null);

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("El rol no puede estar vacío")
            .Must(role => role == null || UserRoles.IsValidRole(role))
            .WithMessage($"El rol debe ser uno de los siguientes: {string.Join(", ", UserRoles.GetAllRoles())}")
            .When(x => x.Role != null);
    }
}
