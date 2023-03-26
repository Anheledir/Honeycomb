using BaseBotService.Core.Base;
using System.Reflection;
using System.Text.Json;

namespace BaseBotService.Utilities;

public static class DocumentationUtility
{
    public static string GenerateDocumentationJson()
    {
        Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
        IEnumerable<Type> interactionModules = allTypes.Where(t => t.IsSubclassOf(typeof(InteractionModuleBase<SocketInteractionContext>)) && t != typeof(BaseModule));

        List<object> modulesDocumentation = new();

        foreach (Type? module in interactionModules)
        {
            MethodInfo[] methods = module.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            List<object> commandsDocumentation = new();

            foreach (MethodInfo method in methods)
            {
                SlashCommandAttribute? slashCommandAttribute = method.GetCustomAttribute<SlashCommandAttribute>();
                UserCommandAttribute? userCommandAttribute = method.GetCustomAttribute<UserCommandAttribute>();

                if (slashCommandAttribute is not null || userCommandAttribute is not null)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    var parametersDocumentation = parameters.Select(p => new
                    {
                        p.Name,
                        Type = p.ParameterType.Name,
                        p.IsOptional,
                        p.DefaultValue
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

        JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
        return JsonSerializer.Serialize(modulesDocumentation, jsonOptions);
    }
}