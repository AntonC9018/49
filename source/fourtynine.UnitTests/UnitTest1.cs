using AutoMapper;
using fourtynine.Postings;

namespace fourtynine.UnitTests;

public class MappingConfigurationsTests
{
    private void TestProfileCompiles(Profile profile)
    {
        var config = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile(profile);
        });
        
        config.AssertConfigurationIsValid();
    }
    
    [Fact]
    public void WhenProfilesAreConfigured_ItShouldNotThrowException()
    {
        TestProfileCompiles(new PostingMapperProfile());
        TestProfileCompiles(new ODataPostingMapperProfile());
    }
}