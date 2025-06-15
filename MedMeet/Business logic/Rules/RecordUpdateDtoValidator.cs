using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Record;
using FluentValidation;

namespace Business_logic.Rules
{
    public class RecordUpdateDtoValidator : AbstractValidator<RecordUpdateDto>
    {
        public RecordUpdateDtoValidator()
        {
            RuleFor(x => x.AppointmentDate)
                .NotEmpty()
                .WithMessage("Дата є обов'язковою")
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Дата прийому не може бути в минулому.")
                .Must(BeWithinAllowedHours)
                .WithMessage("Час прийому повинен бути не раніше 08:00 і не пізніше 17:30.");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Статус є обов'язковим")
                .MaximumLength(50)
                .WithMessage("Статус не може перевищувати 50 символів.")
                .Must(status => new[] { "scheduled", "completed", "cancelled" }.Contains(status.ToLower()))
                .WithMessage("Статус повинен бути 'Scheduled', 'Completed' або 'Cancelled'.");

            RuleFor(x => x.Notes)
                .NotEmpty()
                .WithMessage("Нотатки є обов'язковими")
                .MaximumLength(500)
                .WithMessage("Нотатки не можуть перевищувати 500 символів.");
        }

        private bool BeWithinAllowedHours(DateTime dateTime)
        {
            var time = dateTime.TimeOfDay;
            var start = new TimeSpan(8, 0, 0);    
            var end = new TimeSpan(17, 30, 0);   
            return time >= start && time <= end;
        }
    }
}