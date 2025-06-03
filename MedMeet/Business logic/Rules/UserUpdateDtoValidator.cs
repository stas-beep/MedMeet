using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Users;
using FluentValidation;

namespace Business_logic.Rules
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("ПІБ є обов’язковим.")
                .MaximumLength(150)
                .WithMessage("ПІБ не може перевищувати 150 символів.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email є обов’язковим.")
                .EmailAddress()
                .WithMessage("Невірний формат електронної пошти.")
                .MaximumLength(100)
                .WithMessage("Email не може перевищувати 100 символів.");

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.Password))
                .WithMessage("Пароль має містити щонайменше 6 символів.")
                .MaximumLength(255)
                .WithMessage("Пароль не може перевищувати 255 символів.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Роль є обов’язковою.")
                .MaximumLength(50)
                .WithMessage("Роль не може перевищувати 50 символів.")
                .Must(role => new[] { "customer", "admin", "doctor" }
                .Contains(role.ToLower()));
        }
    }
}
