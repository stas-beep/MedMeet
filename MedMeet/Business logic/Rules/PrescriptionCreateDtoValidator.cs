using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Prescription;
using FluentValidation;

namespace Business_logic.Rules
{
    public class PrescriptionCreateDtoValidator : AbstractValidator<PrescriptionCreateDto>
    {
        public PrescriptionCreateDtoValidator()
        {
            RuleFor(x => x.RecordId)
                .GreaterThan(0)
                .WithMessage("RecordId є обов’язковим і має бути більшим за 0.");

            RuleFor(x => x.Medication)
                .NotEmpty()
                .WithMessage("Назва препарату є обов’язковою.")
                .MaximumLength(100)
                .WithMessage("Назва препарату не може перевищувати 100 символів.");

            RuleFor(x => x.Dosage)
                .NotEmpty()
                .WithMessage("Доза є обов'язковою")
                .MaximumLength(50)
                .WithMessage("Дозування не може перевищувати 50 символів.");

            RuleFor(x => x.Instructions)
                .NotEmpty()
                .WithMessage("Інстуркції є обов'язково")
                .MaximumLength(250)
                .WithMessage("Інструкція не може перевищувати 250 символів.");
        }
    }
}
