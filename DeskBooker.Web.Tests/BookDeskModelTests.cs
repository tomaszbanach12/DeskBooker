using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using DeskBooker.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeskBooker.Web.Tests
{

    public class BookDeskModelTests
    {
        private Mock<IDeskBookingRequestProcessor> _deskBookingRequestProcessorMock;
        private BookDeskModel _bookDeskModel;
        private DeskBookingResult _deskBookintResult;

        public BookDeskModelTests()
        {
            _deskBookingRequestProcessorMock = new Mock<IDeskBookingRequestProcessor>();

            _bookDeskModel = new BookDeskModel(_deskBookingRequestProcessorMock.Object)
            {
                DeskBookingRequest = new DeskBookingRequest()
            };

            _deskBookintResult = new DeskBookingResult
            {
                Code = DeskBookingResultCode.Success
            };

            _deskBookingRequestProcessorMock.Setup(x => x.BookDesk(_bookDeskModel.DeskBookingRequest)).Returns(_deskBookintResult);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        public void ShouldCallBookDeskMethodOfProcessorIfModelIsValid(int expectedBookDeskCalls, bool isModelValid)
        {
            // Arrange
            if (!isModelValid)
            {
                _bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");
            }

            // Act
            _bookDeskModel.OnPost();

            // Assert
            _deskBookingRequestProcessorMock.Verify(x => x.BookDesk(_bookDeskModel.DeskBookingRequest), Times.Exactly(expectedBookDeskCalls));
        }

        [Fact]
        public void ShouldAddModelErrorIfNoDeskIsAvailable()
        {
            // Arrange
            _deskBookintResult.Code = DeskBookingResultCode.NoDeskAvailable;

            // Act
            _bookDeskModel.OnPost();

            // Assert
            var modelStateEntry = Assert.Contains("DeskBookingRequest.Date", _bookDeskModel.ModelState);
            var modelError = Assert.Single(modelStateEntry.Errors);
            Assert.Equal("No desk available for selected date", modelError.ErrorMessage);
        }

        [Fact]
        public void ShouldRedirectToBookDeskConfirmationPage()
        {
            // Arrange
            _deskBookintResult.Code = DeskBookingResultCode.Success;
            _deskBookintResult.DeskBookingId = 7;
            _deskBookintResult.FirstName = "Tomasz";
            _deskBookintResult.LastName = "Master";
            _deskBookintResult.Date = new DateTime(2020, 1, 28);

            // Act
            IActionResult actionResult = _bookDeskModel.OnPost();

            // Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(actionResult);
            Assert.Equal("BookDeskConfirmation", redirectToPageResult.PageName);

            IDictionary<string, object> routeValues = redirectToPageResult.RouteValues;
            Assert.Equal(4, routeValues.Count);

            var deskBookingId = Assert.Contains("DeskBookingId", routeValues);
            Assert.Equal(_deskBookintResult.DeskBookingId, deskBookingId);

            var firstName = Assert.Contains("FirstName", routeValues);
            Assert.Equal(_deskBookintResult.FirstName, firstName);

            var lastName = Assert.Contains("LastName", routeValues);
            Assert.Equal(_deskBookintResult.LastName, lastName);

            var date = Assert.Contains("Date", routeValues);
            Assert.Equal(_deskBookintResult.Date, date);
        }

        [Fact]
        public void ShouldNotAddModelErrorIfDeskIsAvailable()
        {
            // Arrange
            _deskBookintResult.Code = DeskBookingResultCode.Success;

            // Act
            _bookDeskModel.OnPost();

            // Assert
            Assert.DoesNotContain("DeskBookingRequest.Date", _bookDeskModel.ModelState);
        }

        [Theory]
        [InlineData(typeof(PageResult), false, null)]
        [InlineData(typeof(PageResult), true, DeskBookingResultCode.NoDeskAvailable)]
        [InlineData(typeof(RedirectToPageResult), true, DeskBookingResultCode.Success)]
        public void ShouldReturnExpectedActionResult(Type expectedActionResult, bool isModelValid,
            DeskBookingResultCode? deskBookingResultCode)
        {
            // Arrange
            if (!isModelValid)
            {
                _bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");
            }

            if (deskBookingResultCode.HasValue)
            {
                _deskBookintResult.Code = deskBookingResultCode.Value;
            }

            // Act
            IActionResult actionResult = _bookDeskModel.OnPost();

            // Assert
            Assert.IsType(expectedActionResult, actionResult);
        }
    }
}
