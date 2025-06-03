using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Prescription;
using FluentValidation;

namespace Business_logic.Rules
{
    public class PrescriptionUpdateDtoValidator : AbstractValidator<PrescriptionUpdateDto>
    {
        public PrescriptionUpdateDtoValidator()
        {
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
                .WithMessage("Інструкція є обов'язковою")
                .MaximumLength(250)
                .WithMessage("Інструкція не може перевищувати 250 символів.");
        }
    }
}
