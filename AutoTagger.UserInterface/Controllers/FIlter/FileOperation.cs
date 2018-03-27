namespace AutoTagger.UserInterface.Controllers.FIlter
{
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class FileOperation : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower() != "imagefilepost")
            {
                return;
            }

            operation.Parameters.Clear(); // Clearing parameters
            operation.Parameters.Add(
                new NonBodyParameter
                {
                    Name        = "File",
                    In          = "formData",
                    Description = "Upload Image",
                    Required    = true,
                    Type        = "file"
                });

            operation.Consumes.Add("application/form-data");
        }
    }
}
