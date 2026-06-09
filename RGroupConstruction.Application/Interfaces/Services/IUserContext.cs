namespace RGroupConstruction.Application.Interfaces.Services;

public interface IUserContext
{
    Guid? UserId { get; }
    string IpAddress { get; }
}

