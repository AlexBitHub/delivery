using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public class TransportShould
    {
        //[Theory]
        //[InlineData()]
        public void CanBeFoundByName(string name, int id)
        {

        }

        public void CanBeFoundById(int id, string name) 
        {

        }

        [Fact]
        public void BeEqualToTransportWithTheSameId()
        {

        }
        
        [Fact]
        public void ReturnTypesNamesList()
        {

        }

        public void ReturnErrorWhenTransportNotFoundByName(string name)
        {

        }

        public void ReturnErrorWhenTransportNotFoundById(int id)
        {

        }
    }
}
