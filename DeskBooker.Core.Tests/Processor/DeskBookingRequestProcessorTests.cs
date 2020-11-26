﻿using DeskBooker.Core.Domain;
using DeskBooker.Core.DataInterface;
using Moq;
using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
        private readonly Mock<IDeskRepository> _deskRepositoryMock;
        private readonly DeskBookingRequest _request;
        private readonly List<Desk> _availableDesks;
        private readonly DeskBookingRequestProcessor _processor; 

        public DeskBookingRequestProcessorTests()
        {
            
            // Arrange
            _request = new DeskBookingRequest
            {
                FirstName = "Tomasz",
                LastName = "Banan",
                Email = "tomaszbanan12@gman.com",
                Date = new DateTime(2020, 11, 13)
            };

            _availableDesks = new List<Desk> { new Desk { Id = Guid.NewGuid() } };

            _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
            _deskRepositoryMock = new Mock<IDeskRepository>();
            _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_request.Date))
                .Returns(_availableDesks);

            _processor = new DeskBookingRequestProcessor(
                _deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
        }

        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValues()
        {
            // Act
            DeskBookingResult result = _processor.BookDesk(_request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_request.FirstName, result.FirstName);
            Assert.Equal(_request.LastName, result.LastName);
            Assert.Equal(_request.Email, result.Email);
            Assert.Equal(_request.Date, result.Date);
        }

        [Fact]
        public void ThrowExceptionIfRequestIsNull()
        {
            // Arrange & Act
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

            // Assert
            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public void ShouldSaveDeskBooking()
        {
            // Arrange
            DeskBooking savedDeskBooking = null;
            _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                .Callback<DeskBooking>(deskBooking => 
                {
                    savedDeskBooking = deskBooking;
                });

            // Act
            _processor.BookDesk(_request);

            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

            // Assert
            Assert.NotNull(savedDeskBooking);
            Assert.Equal(_request.FirstName, savedDeskBooking.FirstName);
            Assert.Equal(_request.LastName, savedDeskBooking.LastName);
            Assert.Equal(_request.Email, savedDeskBooking.Email);
            Assert.Equal(_request.Date, savedDeskBooking.Date);
            Assert.Equal(_availableDesks.First().Id, savedDeskBooking.DeskId); 
        }

        [Fact]
        public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
        {
            // Arrange
            _availableDesks.Clear();

            // Act
            _processor.BookDesk(_request);

            // Assert
            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
        }

        [Theory]
        [InlineData(DeskBookingResultCode.Success, true)]
        [InlineData(DeskBookingResultCode.NoDeskAvailable, false)]

        public void ShouldReturnExpectedResultCode(DeskBookingResultCode expectedResultCode, bool isDeskAvailable)
        {
            // Arrange
            if (!isDeskAvailable)
            {
                _availableDesks.Clear();
            }

            // Act
            var result = _processor.BookDesk(_request);

            // Assert
            Assert.Equal(expectedResultCode, result.Code);
        }

        [Theory]
        [InlineData("3f77d236-0a5a-4fb6-9e6f-12f46ff9f645", true)]
        [InlineData("00000000-0000-0000-0000-000000000000", false)]
        [InlineData(null, false)]

        public void ShouldReturnExpectedDeskBookingId(string expectedDeskBookingIdInString, bool isDeskAvailable)
        {
            // Arrange
            Guid expectedDeskBookingId = Guid.Empty;
            if (expectedDeskBookingIdInString != null)
            {
                expectedDeskBookingId = new Guid(expectedDeskBookingIdInString);
            }  

            if (!isDeskAvailable)
            {
                _availableDesks.Clear();
            }
            else
            {
                _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                    .Callback<DeskBooking>(deskBooking =>
                    {
                        deskBooking.Id = expectedDeskBookingId;
                    });
            }
            
            // Act
            var result = _processor.BookDesk(_request);

            // Assert
            Assert.Equal(expectedDeskBookingId, result.DeskBookingId);
        }
    }
}
