using AutoMapper;

namespace fourtynine;

public interface IODataMapperProvider
{
    public IMapper Mapper { get; }
}

public sealed class ODataMapperProvider : IODataMapperProvider
{
    public IMapper Mapper { get; }

    public ODataMapperProvider(IMapper mapper)
    {
        Mapper = mapper;
    }
}