using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace UserMgt.BLL.DTOs.Request
{
    public class UpdateRequestDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; init; }
        [EmailAddress]
        public string Email { get; init; }
        public string? PhoneNumber { get; init; }
    }

    public class UpdateRequestValidator : AbstractValidator<UpdateRequestDto>
    {
        public UpdateRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().Length(3, 50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }


    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var value in context.ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                context.Result = new BadRequestObjectResult(new { message = "Validation errors", errors });
            }
        }
    }
}
