using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Filters;
using SFA.DAS.CommitmentsV2.Api.Types.Http;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

namespace SFA.DAS.CommitmentsV2.Api.UnitTests.Filters
{
    [TestFixture]
    public class ValidateModelStateFilterTests
    {
        private ValidateModelStateFilterTestsFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new ValidateModelStateFilterTestsFixture();
        }

        [Test]
        public void OnActionExecuting_WhenModelStateIsValid_ThenShouldNotSetResult()
        {
            _fixture.OnActionExecuting();
            
            Assert.IsNull(_fixture.ActionExecutingContext.Result);
        }

        [Test]
        public void OnActionExecuting_WhenModelStateIsNotValid_ThenShouldSetSubStatusCodeHeader()
        {
            _fixture.SetInvalidModelState().OnActionExecuting();
            
            Assert.AreEqual(_fixture.DomainExceptionHttpSubStatusCodeHeaderValue, _fixture.Headers[HttpHeaderNames.SubStatusCode]);
        }

        [Test]
        public void OnActionExecuting_WhenModelStateIsNotValid_ThenShouldSetBadRequestObjectResult()
        {
            _fixture.SetInvalidModelState().OnActionExecuting();

            var badRequestObjectResult = _fixture.ActionExecutingContext.Result as BadRequestObjectResult;
            var errorResponse = badRequestObjectResult?.Value as ErrorResponse;
            
            Assert.IsNotNull(_fixture.ActionExecutingContext.Result);
            Assert.IsNotNull(badRequestObjectResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestObjectResult.StatusCode);
            Assert.IsNotNull(errorResponse);
            Assert.IsTrue(errorResponse.Errors.Exists(e => e.Field == "Foo" && e.Message == "Bar"));
        }
    }

    public class ValidateModelStateFilterTestsFixture
    {
        public IHeaderDictionary Headers { get; set; }
        public Mock<HttpContext> HttpContext { get; set; }
        public Mock<TypeInfo> ControllerTypeInfo { get; set; }
        public Mock<MethodInfo> MethodInfo { get; set; }
        public ControllerActionDescriptor ActionDescriptor { get; set; }
        public ModelStateDictionary ModelState { get; set; }
        public ActionContext ActionContext { get; set; }
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public ValidateModelStateFilter ValidateModelStateFilter { get; set; }
        public HttpSubStatusCode DomainExceptionHttpSubStatusCode { get; set; }
        public string DomainExceptionHttpSubStatusCodeHeaderValue { get; set; }

        public ValidateModelStateFilterTestsFixture()
        {
            Headers = new HeaderDictionary(new Dictionary<string, StringValues>());
            HttpContext = new Mock<HttpContext>();
            ControllerTypeInfo = new Mock<TypeInfo>();
            MethodInfo = new Mock<MethodInfo>();
            
            ActionDescriptor = new ControllerActionDescriptor
            {
                ControllerName = Guid.NewGuid().ToString(),
                ControllerTypeInfo = ControllerTypeInfo.Object,
                ActionName = Guid.NewGuid().ToString(),
                MethodInfo = MethodInfo.Object
            };

            ModelState = new ModelStateDictionary();
            ActionContext = new ActionContext(HttpContext.Object, new RouteData(),  ActionDescriptor, ModelState);
            ActionExecutingContext = new ActionExecutingContext(ActionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), null);
            ValidateModelStateFilter = new ValidateModelStateFilter();
            DomainExceptionHttpSubStatusCode = HttpSubStatusCode.DomainException;
            DomainExceptionHttpSubStatusCodeHeaderValue = ((int)DomainExceptionHttpSubStatusCode).ToString();

            HttpContext.Setup(c => c.Response.Headers).Returns(Headers);
        }

        public void OnActionExecuting()
        {
            ValidateModelStateFilter.OnActionExecuting(ActionExecutingContext);
        }

        public ValidateModelStateFilterTestsFixture SetInvalidModelState()
        {
            ModelState.AddModelError("Foo", "Bar");

            return this;
        }
    }
}