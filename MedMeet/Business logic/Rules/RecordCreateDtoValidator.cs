using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Business_logic.Data_Transfer_Object.For_Record
{
    public class RecordCreateDtoValidator : AbstractValidator<RecordCreateDto>
    {
        public RecordCreateDtoValidator()
        {
            RuleFor(x => x.DoctorId)
                .GreaterThan(0)
                .WithMessage("Лікар є обов’язковим.");

            RuleFor(x => x.AppointmentDate)
                .NotEmpty()
                .WithMessage("Дата є обов'язковою")
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Дата прийому не може бути в минулому.");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Статус є обов'язковим")
                .MaximumLength(50)
                .WithMessage("Статус не може перевищувати 50 символів.")
                .Must(status => new[] { "scheduled", "completed", "cancelled" }.Contains(status.ToLower()))
                .WithMessage("Статус повинен бути 'Scheduled', 'Completed' або 'Cancelled'.");

            RuleFor(x => x.Notes)
                .NotEmpty()
                .WithMessage("Нотатки є обов'язковоми")
                .MaximumLength(500)
                .WithMessage("Нотатки не можуть перевищувати 500 символів.");
        }
    }
}
