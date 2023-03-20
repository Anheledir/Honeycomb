using BaseBotService.Modules;
using System.Reflection;
using System.Text.Json;

namespace BaseBotService.Utilities;

public static class DocumentationUtility
{
    public static string GenerateDocumentationJson()
    {
        var allTypes = Assembly.GetExecutingAssembly().GetTypes();
        var interactionModules = allTypes.Where(t => t.IsSubclassOf(typeof(InteractionModuleBase<SocketInteractionContext>)) && t != typeof(BaseModule));

        var modulesDocumentation = new List<object>();

        foreach (var module in interactionModules)
        {
            var methods = module.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            var commandsDocumentation = new List<object>();

            foreach (var method in methods)
            {
                var slashCommandAttribute = method.GetCustomAttribute<SlashCommandAttribute>();
                var userCommandAttribute = method.GetCustomAttribute<UserCommandAttribute>();

                if (slashCommandAttribute is not null || userCommandAttribute is not null)
                {
                    var parameters = method.GetParameters();
                    var parametersDocumentation = parameters.Select(p => new
                    {
                        Name = p.Name,
                        Type = p.ParameterType.Name,
                        IsOptional = p.IsOptional,
                        DefaultValue = p.DefaultValue
                    });

                    commandsDocumentation.Add(new
                    {
                        CommandName = slashCommandAttribute?.Name ?? userCommandAttribute.Name,
                        CommandType = slashCommandAttribute is not null ? "SlashCommand" : "UserCommand",
                        Description = slashCommandAttribute?.Description ?? string.Empty,
                        Parameters = parametersDocumentation
                    });
                }
            }

            modulesDocumentation.Add(new
            {
                ModuleName = module.Name,
                Commands = commandsDocumentation
            });
        }

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(modulesDocumentation, jsonOptions);
    }
}