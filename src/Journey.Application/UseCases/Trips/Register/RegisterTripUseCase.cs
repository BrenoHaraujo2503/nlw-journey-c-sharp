using Journey.Communication.Requests;
using Journey.Exception.ExceptionsBase;
using Journey.Exception;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Journey.Communication.Responses;

namespace Journey.Application.UseCases.Trips.Register
{
    public class RegisterTripUseCase
    {
        public ResponseShortTripJson Execute(RequestRegisterTripJson request)
        {
            Validate(request);

            var dbContext = new JourneyDbContext();

            var entity = new Trip
            {
                Name = request.Name,
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
            };

            dbContext.Trips.Add(entity);

            dbContext.SaveChanges();

            return new ResponseShortTripJson
            {
                Id = entity.Id,
                Name = entity.Name,
                EndDate = entity.EndDate,
                StartDate = entity.StartDate
            };
        }

        private void Validate(RequestRegisterTripJson request)
        {
            var validator = new RegisterTripValidator();
            var result = validator.Validate(request);

            if (result.IsValid == false) 
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }

    }
}
