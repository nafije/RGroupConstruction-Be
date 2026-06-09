using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using Microsoft.Extensions.Localization;

namespace RGroupConstruction.Application.Services;

public class MessageLocalizer(IStringLocalizer<Messages> localizer) : IMessageLocalizer
{
    public string this[string key] => localizer[key];
    public string this[string key, params object[] args] => localizer[key, args];
}

