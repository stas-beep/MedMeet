using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Auth;
using FluentValidation;

namespace Business_logic.Rules
{
    public class RegisterDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email є обов’язковим.")
                .EmailAddress()
                .WithMessage("Невірний формат електронної пошти.")
                .MaximumLength(100)
                .WithMessage("Email не може перевищувати 100 символів.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Пароль є обов’язковим.")
                .MinimumLength(6)
                .WithMessage("Пароль має містити щонайменше 6 символів.")
                .MaximumLength(255)
                .WithMessage("Пароль не може перевищувати 255 символів.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Ім'я обов'язкове");
        }
    }
}