namespace RGroupConstruction.Application.DTOs.Base;

public class BaseDto<T>
{
    public T? Id { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}

