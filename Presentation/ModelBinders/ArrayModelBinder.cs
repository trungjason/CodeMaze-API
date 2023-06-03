using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace Presentation.ModelBinders
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // if datatype is not Enumerable => just pass
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            };

            // extract the value (a comma-separated string of GUIDs)
            var providedValue = bindingContext.ValueProvider
                                                 .GetValue(bindingContext.ModelName)
                                                 .ToString();

            if (string.IsNullOrEmpty(providedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            };

            // Using Reflection to get the model type (here is GUID)
            var genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];

            // create converter to a GUID type
            var converter = TypeDescriptor.GetConverter(genericType);

            // convert an array of string type object to guid
            var objectArray = providedValue
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray();

            var guidArray = Array.CreateInstance(genericType, objectArray.Length);

            // copy all the values from the objectArray to 
            // the guidArray, and assign it to the bindingContext.
            objectArray.CopyTo(guidArray, 0);

            bindingContext.Model = guidArray;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);

            return Task.CompletedTask;
        }
    }
}
