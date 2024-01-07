namespace NewShoreAir.Business.Application.Validators
{
    public class CalcularRutaValidator : AbstractValidator<CalcularRutaRequest>
    {
        public CalcularRutaValidator()
        {
            RuleFor(p => p.Origen)
                .NotNull().WithMessage("El campo no puede ser nulo")
                .NotEmpty().WithMessage("El campo no puede ser vacío")
                .Length(3).WithMessage("El campo debe tener exactamente 3 caracteres.")
                .Must(SoloMayusculas).WithMessage("El campo debe contener solo letras mayúsculas.");

            RuleFor(p => p.Destino)
                .NotNull().WithMessage("El campo no puede ser nulo")
                .NotEmpty().WithMessage("El campo no puede ser vacío")
                .Length(3).WithMessage("El campo debe tener exactamente 3 caracteres.")
                .Must(SoloMayusculas).WithMessage("El campo debe contener solo letras mayúsculas.");
        }

        private bool SoloMayusculas(string valor)
        {
            return valor.All(char.IsUpper);
        }
    }
}