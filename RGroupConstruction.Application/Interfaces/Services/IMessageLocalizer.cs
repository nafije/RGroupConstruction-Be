namespace RGroupConstruction.Application.Interfaces.Services;

public interface IMessageLocalizer
{
    string this[string key] { get; }
    string this[string key, params object[] args] { get; }
}

