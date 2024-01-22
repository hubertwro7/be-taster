using FluentValidation;
using MediatR;

namespace Taster.Application.Validators
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<IRequest>> validators;

        public ValidationBehavior(IEnumerable<IValidator<IRequest>> validators) 
        {
            this.validators = validators.ToList();
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if(!validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);
            var errors = validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .GroupBy(x => new { x.PropertyName, x.ErrorCode })
                .ToList();

            if(errors.Any())
            {
                throw new Exceptions.ValidationException()
                {
                    Errors = errors.Select(x => new Exceptions.ValidationException.FieldError()
                    {
                        Error = x.Key.ErrorCode,
                        FieldName = x.Key.PropertyName,
                    }).ToList(),
                };
            }

            return await next();
        }
    }
}
