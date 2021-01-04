using DeskBooker.Core.Validation;
using System;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DateWithoutTimeAttributeTests 
    {
        [InlineData(true, 1)]
        [InlineData(true, 3)]
        [InlineData(false, -1)]
        [Theory]
        public void ReturnDateMustBeInTheFuture(bool expectedValue, double daysToAdd)
        {
            DateTime dateTime1 = DateTime.Now;

            var dateInFutureAttribute = new DateInFutureAttribute();

            var isValid = dateInFutureAttribute.IsValid(dateTime1.AddDays(daysToAdd));

            Assert.Equal(expectedValue, isValid);
        }
    }
}
