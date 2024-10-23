using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.VoucherModels
{
    public class VoucherProcessModel
    {
        public string? Code { get; set; }

        public string? Name { get; set; }

        public int? DiscountPercent { get; set; }

        public int? MaxDiscount { get; set; }

        public int? MinOrderValue { get; set; }

        public DateTime? StartDate { get; set; }

        [DateRange("StartDate", ErrorMessage = "Ngày bắt đầu phải lớn hơn ngày kết thúc")]
        public DateTime? ExpireDate { get; set; }
    }

    public class DateRangeAttribute : ValidationAttribute
    {
        public string StartDateProperty { get; }

        public DateRangeAttribute(string startDateProperty)
        {
            StartDateProperty = startDateProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var startDateProperty = validationContext.ObjectType.GetProperty(StartDateProperty);
            if (startDateProperty == null)
            {
                return new ValidationResult($"Unknown property: {StartDateProperty}");
            }

            var startDateValue = startDateProperty.GetValue(validationContext.ObjectInstance, null) as DateTime?;
            var endDateValue = value as DateTime?;

            if (startDateValue == null || endDateValue == null)
            {
                return ValidationResult.Success; // If one of the dates is null, let other validations handle it
            }

            if (endDateValue < startDateValue)
            {
                return new ValidationResult("The ExpireDate must be after the StartDate.");
            }

            return ValidationResult.Success;
        }
    }
}

