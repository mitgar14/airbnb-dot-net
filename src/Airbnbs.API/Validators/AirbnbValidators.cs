using FluentValidation;
using Airbnbs.API.DTOs;

namespace Airbnbs.API.Validators;

public class CreateAirbnbDtoValidator : AbstractValidator<CreateAirbnbDto>
{
    public CreateAirbnbDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido");

        ApplyNameRules();
        ApplyHostIdRules();
        ApplyHostNameRules();
        ApplyRoomTypeRules();
        ApplyPriceRules();
    }

    protected void ApplyNameRules()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(255).WithMessage("El nombre no puede exceder 255 caracteres");
    }

    protected void ApplyHostIdRules()
    {
        RuleFor(x => x.HostId)
            .NotEmpty().WithMessage("El ID del host es requerido");
    }

    protected void ApplyHostNameRules()
    {
        RuleFor(x => x.HostName)
            .NotEmpty().WithMessage("El nombre del host es requerido");
    }

    protected void ApplyRoomTypeRules()
    {
        RuleFor(x => x.RoomType)
            .NotEmpty().WithMessage("El tipo de habitación es requerido");
    }

    protected void ApplyPriceRules()
    {
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("El precio es requerido");
    }
}

public class UpdateAirbnbDtoValidator : AbstractValidator<UpdateAirbnbDto>
{
    public UpdateAirbnbDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre no puede estar vacío")
            .MaximumLength(255).WithMessage("El nombre no puede exceder 255 caracteres")
            .When(x => x.Name != null);

        RuleFor(x => x.HostId)
            .NotEmpty().WithMessage("El ID del host no puede estar vacío")
            .When(x => x.HostId != null);

        RuleFor(x => x.HostName)
            .NotEmpty().WithMessage("El nombre del host no puede estar vacío")
            .When(x => x.HostName != null);

        RuleFor(x => x.RoomType)
            .NotEmpty().WithMessage("El tipo de habitación no puede estar vacío")
            .When(x => x.RoomType != null);

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("El precio no puede estar vacío")
            .When(x => x.Price != null);
    }
}
