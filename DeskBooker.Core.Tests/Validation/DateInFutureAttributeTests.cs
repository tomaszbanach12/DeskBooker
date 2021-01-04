using DeskBooker.Core.Validation;
using System;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DateInFutureAttributeTests
    {
        [InlineData(true, 1)]
        [InlineData(true, 3)]
        [InlineData(false, -1)]
        [Theory]
        public void ReturnDateMustBeInTheFuture(bool expectedValue, double daysToAdd)
        {
            DateTime dateTime1 = DateTime.Now;

            var dateWithoutTimeAttribute = new DateInFutureAttribute();

            Assert.Equal(expectedValue, dateWithoutTimeAttribute.IsValid(dateTime1.AddDays(daysToAdd)));
        }
    }
}
