using FluentValidation.Results;
using Journey.Communication.Enums;
using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Journey.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UseCases.Activities.Register
{
    public class RegisterActivityUseCase
    {
        public ResponseActivityJson Execute(Guid tripId, RequestRegisterActivityJson request)
        {


            var dbContext = new JourneyDbContext();

            var trip = dbContext
                .Trips
                .FirstOrDefault(trip => trip.Id == tripId);

            Validate(trip, request);

            var entity = new Activity
            {
                Name = request.Name,
                Date = request.Date,
                TripId = tripId,
            };

            dbContext.Activities.Add(entity);
            dbContext.SaveChanges();

            return new ResponseActivityJson
            {
                Id = entity.Id,
                Date = entity.Date,
                Name = entity.Name,
                Status = (Communication.Enums.ActivityStatus)entity.Status,
            };
        }

        private void Validate(Trip? trip, RequestRegisterActivityJson request)
        {

            if (trip is null)
            {
                throw new NotFoundException(ResourceErrorMessages.TRIP_NOT_FOUND);
            }

            var validator = new RegisterActivityValidator();

            var result = validator.Validate(request);

            if ((request.Date >= trip.StartDate && request.Date <= trip.EndDate) == false)
            {
                result.Errors.Add(new ValidationFailure("date", ResourceErrorMessages.DATE_NOT_WITHIN_TRAVEL_PERIOD));
            }

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
