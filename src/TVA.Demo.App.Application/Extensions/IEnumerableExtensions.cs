using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Application.Extensions
{
    internal static class IEnumerableExtensions
    {
        internal static IEnumerable<PersonDto> Filter(this IEnumerable<PersonDto> personDtos, string? filter)
        {
            if (!string.IsNullOrEmpty(filter) && !string.Equals(filter, "null", StringComparison.OrdinalIgnoreCase))
            {
                personDtos = personDtos.Where(p =>
                    p.Name?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true ||
                    p.Surname?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true ||
                    p.Id_Number?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true);
            }
            return personDtos;
        }

        internal static IEnumerable<PersonDto> Sort(this IEnumerable<PersonDto> personDtos, string? sortBy, bool isDescending)
        {
            if (!string.IsNullOrEmpty(sortBy) && !string.Equals(sortBy, "null", StringComparison.OrdinalIgnoreCase))
            {
                var propertyInfo = typeof(PersonDto).GetProperties().FirstOrDefault(prop => string.Equals(prop.Name, sortBy, StringComparison.OrdinalIgnoreCase));
                if (propertyInfo != null)
                {
                    if (isDescending)
                    {
                        personDtos = personDtos.OrderByDescending(p => propertyInfo.GetValue(p));
                    }
                    else
                    {
                        personDtos = personDtos.OrderBy(p => propertyInfo.GetValue(p));
                    }
                }
            }
            else
            {
                personDtos = personDtos.OrderBy(p => p.Code);
            }
            return personDtos;
        }
    }
}
