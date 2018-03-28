using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using FluentValidation;
using SFA.DAS.Commitments.Application.Exceptions;
using SFA.DAS.Core.Common;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Commitments.Api
{
    public class CustomExceptionHandler : ExceptionHandler
    {
        private static readonly ILog Logger = new NLogLogger();

        public override void Handle(ExceptionHandlerContext context)
        {
            if (context.Exception is ValidationException)
            {
                var response = context.Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Message = context.Exception.Message,
                        Code = "ValidationError",
                        DomainExceptionId = (context.Exception as DomainException)?.DomainExceptionId
                    });
                context.Result = new ResponseMessageResult(response);
                Logger.Warn(context.Exception, "Validation error");
            }
            else if (context.Exception is UnauthorizedException)
            {
                var response = context.Request.CreateResponse(HttpStatusCode.Unauthorized,
                    new ErrorResponse
                    {
                        Message = context.Exception.Message,
                        Code = "AuthorizationError",
                        DomainExceptionId = (context.Exception as DomainException)?.DomainExceptionId
                    });
                context.Result = new ResponseMessageResult(response);
                Logger.Warn(context.Exception, "Authorisation error");
            }
            else if (context.Exception is ValidationException)
            {
                var response = context.Request.CreateResponse(HttpStatusCode.Unauthorized,
                    new ErrorResponse
                    {
                        Message = context.Exception.Message,
                        Code = "AuthorizationError",
                        DomainExceptionId = (context.Exception as DomainException)?.DomainExceptionId
                    });
                context.Result = new ResponseMessageResult(response);
                Logger.Warn(context.Exception, "Authorisation error");
            }
            else if (context.Exception is ResourceNotFoundException)
            {
                var response = context.Request.CreateResponse(HttpStatusCode.NotFound,
                    new ErrorResponse
                    {
                        Message = context.Exception.Message,
                        Code = "ResourceNotFoundError",
                        DomainExceptionId = (context.Exception as DomainException)?.DomainExceptionId
                    });
                context.Result = new ResponseMessageResult(response);
                Logger.Warn(context.Exception, "Unable to locate resource error");
            }
            else if (context.Exception is DomainException)
            {
                var response = context.Request.CreateResponse(HttpStatusCode.NotFound,
                    new ErrorResponse
                    {
                        Message = context.Exception.Message,
                        Code = "ResourceNotFoundError",
                        DomainExceptionId = (context.Exception as DomainException)?.DomainExceptionId
                    });
                context.Result = new ResponseMessageResult(response);
                Logger.Warn(context.Exception, "Unable to locate resource error");
            }
            else
            {
                var response = context.Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new ErrorResponse
                    {
                        Message = context.Exception.Message,
                        Code = "InternalServerError"
                    });
                context.Result = new ResponseMessageResult(response);
                Logger.Warn(context.Exception, "Unhandled Error");
            }


            Logger.Error(context.Exception, "Unhandled exception");

            base.Handle(context);
        }
    }
}
