using System;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Model.BiindingModels;
using VacationRental.Model.ViewModels;
using Xunit;

namespace VacationRental.Api.Tests.Integrations;

[Collection("Integration")]
public class PostRentalTests
{
    private readonly HttpClient _client;

    public PostRentalTests(IntegrationFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
    {
        var request = new RentalBindingModel
        {
            Units = 25
        };

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
        {
            Assert.True(getResponse.IsSuccessStatusCode);

            var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
            Assert.Equal(request.Units, getResult.Units);
        }
    }

    [Fact]
    public async Task GivenCompleteRequest_WhenPostRental_ThenAPutReturnsTheUpdatedRental()
    {
        var request = new RentalBindingModel
        {
            Units = 25,
            PreparationTimeInDays = 5
        };

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        var putRequest = new RentalBindingModel
        {
            Units = 30,
            PreparationTimeInDays = 2
        };

        using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", putRequest))
        {
            Assert.True(putResponse.IsSuccessStatusCode);

            var putResult = await putResponse.Content.ReadAsAsync<RentalViewModel>();
            Assert.Equal(putRequest.Units, putResult.Units);
            Assert.Equal(putRequest.PreparationTimeInDays, putResult.PreparationTimeInDays);
        }
    }

    [Theory]
    [InlineData(1,1,2,1)]
    [InlineData(1,1,1,1)] 
    [InlineData(3,1,1,1)]
    public async Task GivenCompleteRequest_WhenPostRental_ThenBookingTwiceAndThenAPutReturnsTheUpdatedRental(
        int originalUnits, 
        int originalPreparationDays,
        int targetUnits,
        int targetPreparationDays)
    {
        var request = new RentalBindingModel
        {
            Units = originalUnits,
            PreparationTimeInDays = originalPreparationDays
        };

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        var postBooking1Request = new BookingBindingModel
        {
            RentalId = postResult.Id,
            Nights = 1,
            Start = new DateTime(2002, 01, 01)
        };

        using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
        {
            Assert.True(postBooking1Response.IsSuccessStatusCode);
        }

        var postBooking2Request = new BookingBindingModel
        {
            RentalId = postResult.Id,
            Nights = 1,
            Start = new DateTime(2002, 01, 03)
        };


        var putRequest = new RentalBindingModel
        {
            Units = targetUnits,
            PreparationTimeInDays = targetPreparationDays
        };


        using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", putRequest))
        {
            Assert.True(putResponse.IsSuccessStatusCode);

            var putResult = await putResponse.Content.ReadAsAsync<RentalViewModel>();
            Assert.Equal(putRequest.Units, putResult.Units);
            Assert.Equal(putRequest.PreparationTimeInDays, putResult.PreparationTimeInDays);
        }
    }

    [Theory]
    [InlineData(1, 1, 1, 2)] //overlapping preparationday
    [InlineData(1, 1, 0, 0)] //no rooms to book
    [InlineData(1, 1, 5, 2)] //no original rooms available
    public async Task GivenCompleteRequest_WhenPostRental_ThenBookingTwiceAndThenAPutReturnsErrorWhenOverlapping(
        int originalUnits,
        int originalPreparationDays,
        int targetUnits,
        int targetPreparationDays)
    {
        var request = new RentalBindingModel
        {
            Units = originalUnits,
            PreparationTimeInDays = originalPreparationDays
        };

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        var postBooking1Request = new BookingBindingModel
        {
            RentalId = postResult.Id,
            Nights = 1,
            Start = new DateTime(2002, 01, 01)
        };

        using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
        {
            Assert.True(postBooking1Response.IsSuccessStatusCode);
        }

        var postBooking2Request = new BookingBindingModel
        {
            RentalId = postResult.Id,
            Nights = 1,
            Start = new DateTime(2002, 01, 03)
        };


        using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
        {
            Assert.True(postBooking2Response.IsSuccessStatusCode);
        }

        var putRequest = new RentalBindingModel
        {
            Units = targetUnits,
            PreparationTimeInDays = targetPreparationDays
        };


        await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", putRequest))
            {
            }
        });
    }

}