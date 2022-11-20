using System.Reflection;
using System.Reflection;
using AutoMapper;
using fourtynine.Postings;

namespace fourtynine.UnitTests;

public class MappingConfigurationsTests
{
    [Fact]
    public void WhenProfilesAreConfigured_ItShouldNotThrowException()
    {
        var config = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<PostingMapperProfile>();
        });
		
        config.AssertConfigurationIsValid();
    }
}