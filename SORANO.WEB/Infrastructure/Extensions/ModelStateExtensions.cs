using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class ModelStateExtensions
    {
        public static void RemoveDuplicateErrorMessages(this ModelStateDictionary modelStateDictionary)
        {
            //Stores the error messages we have seen
            var knownValues = new HashSet<string>();

            //Create a copy of the modelstatedictionary so we can modify the original.
            var modelStateDictionaryCopy = modelStateDictionary.ToDictionary(
                element => element.Key,
                element => element.Value);

            foreach (var modelState in modelStateDictionaryCopy)
            {
                var modelErrorCollection = modelState.Value.Errors;
                for (var i = 0; i < modelErrorCollection.Count; i++)
                {
                    //Check if we have seen the error message before by trying to add it to the HashSet
                    if (!knownValues.Add(modelErrorCollection[i].ErrorMessage))
                    {
                        modelStateDictionary[modelState.Key].Errors.RemoveAt(i);
                    }
                }
            }
        }
    }
}