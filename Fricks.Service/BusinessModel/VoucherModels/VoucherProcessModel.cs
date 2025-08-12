using Fricks.Repository.Enum;
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
        public required string Code { get; set; }

        public required string Name { get; set; }

        public int DiscountPercent { get; set; } = 0;

        public int MaxDiscount { get; set; } = 0;

        public int MinOrderValue { get; set; } = 0;

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu.")]
        public DateTime StartDate { get; set; }

        [DateRange("StartDate", ErrorMessage = "Ngày bắt đầu phải lớn hơn ngày kết thúc.")]
        public DateTime ExpireDate { get; set; }

        public AvailabilityVoucher Availability { get; set; } = AvailabilityVoucher.STORE;
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
                return new ValidationResult("Ngày bắt đầu phải lớn hơn ngày kết thúc.");
            }

            return ValidationResult.Success;
        }
    }
}

