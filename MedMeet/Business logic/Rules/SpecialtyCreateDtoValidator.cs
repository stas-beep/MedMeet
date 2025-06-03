using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Speciality;
using FluentValidation;

namespace Business_logic.Rules
{
    public class SpecialtyCreateDtoValidator : AbstractValidator<SpecialtyCreateDto>
    {
        public SpecialtyCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Назва спеціальності є обов’язковою.")
                .MaximumLength(100)
                .WithMessage("Назва спеціальності не може перевищувати 100 символів.");
        }
    }
}
