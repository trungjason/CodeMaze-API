using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.ActionFilters
{
    // This Validation Filter will validate is in action params
    // contain an arguments with DTO type =>
    // it will check if the param is null or modelState is Valid 
    // then throw correctsponding error message
    public class ValidationFilterAttribute : IActionFilter
    {
        public ValidationFilterAttribute()
        {
        }

        public void OnActionExecuting(ActionExecutingContext context) 
        {
            var action = context.RouteData.Values["action"];
            var controller = context.RouteData.Values["controller"];

            var param = context.ActionArguments
                            .SingleOrDefault(x => x.Value
                                                    .ToString()
                                                    .ToLower()
                                                    .Contains("dto")).Value;
            if (param is null)
            {
                context.Result = new BadRequestObjectResult($"Object is null. Controller: { controller }, action: { action}");
                return;
            };

            if (!context.ModelState.IsValid) 
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            };
        }
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
