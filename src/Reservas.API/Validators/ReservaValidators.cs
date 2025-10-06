using FluentValidation;
using Reservas.API.DTOs;

namespace Reservas.API.Validators;

public class CreateReservaDtoValidator : AbstractValidator<CreateReservaDto>
{
    public CreateReservaDtoValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El ID del cliente es requerido");

        RuleFor(x => x.AirbnbId)
            .NotEmpty().WithMessage("El ID del airbnb es requerido");
    }
}
