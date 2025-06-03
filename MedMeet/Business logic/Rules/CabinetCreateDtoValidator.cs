using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Cabinets;
using FluentValidation;

namespace Business_logic.Rules
{
    public class CabinetCreateDtoValidator : AbstractValidator<CabinetCreateDto>
    {
        public CabinetCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Назва кабінету є обов’язковою.")
                .MaximumLength(50)
                .WithMessage("Назва кабінету не може перевищувати 50 символів.");
        }
    }
}
