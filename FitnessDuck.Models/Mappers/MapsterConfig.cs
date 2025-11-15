using Mapster;

namespace FitnessDuck.Models.Mappers;


public class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        
        // Map Book to BookDto
        //config.NewConfig<Book, BookDto>()
        //    .Map(dest => dest.Id, src => src.Id)
        //    .Map(dest => dest.Title, src => src.Title)
        //    .Map(dest => dest.Author, src => src.Author)
        //    .Map(dest => dest.PriceDisplay,
        //        src => $"${src.Price:F2}")
        //    .TwoWays();
    }
}


