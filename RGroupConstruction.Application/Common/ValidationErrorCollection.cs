namespace RGroupConstruction.Application.Common;

public class ValidationErrorCollection : List<ValidationError>
{
    public bool HasErrors => Count > 0;

    public void Add(string propertyName, string errorMessage)
        => Add(new ValidationError(propertyName, errorMessage));

    public IDictionary<string, string[]> ToDictionary()
        => this.GroupBy(e => e.PropertyName ?? string.Empty)
              .ToDictionary(
                  g => g.Key,
                  g => g.Select(e => e.ErrorMessage ?? string.Empty).ToArray());
}

