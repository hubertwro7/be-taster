
using Taster.Application.Exceptions;

namespace Taster.Api.Application.Response
{
    public class ValidationErrorResponse
    {
        public ValidationErrorResponse()
        {
        }

        public ValidationErrorResponse(ValidationException validationException)
        {
            if(validationException != null)
            {
                if(validationException.Errors != null)
                {
                    Errors = validationException.Errors.Select(x => new FieldValidationError()
                    {
                        Error = x.Error,
                        FieldName = x.FieldName,
                    }).ToList();
                }
            }
        }

        public List<FieldValidationError> Errors { get; set; } = new List<FieldValidationError>();

        public class FieldValidationError
        {
            public required string FieldName { get; set; }
            public required string Error { get; set; }
        }
    }
}
