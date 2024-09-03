
using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.WebApi.Shared.Authorization;
using Mapster;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;
public partial class Games
{
    protected EntityServerTableContext<GameDto, Guid, UpdateGameRequest> Context { get; set; } = default!;
    protected override void OnInitialized() =>
        Context = new(
            entityName: L["Game"],
            entityNamePlural: L["Games"],
            entityResource: FSHResource.Games,
            fields: new()
                {
                new(brand => brand.Id, L["Id"], "Id"),
                new(brand => brand.Name, L["Name"], "Name"),
                new(brand => brand.DateTime!.Value.ToString("MMM dd, yyyy") , L["Datetime"], "Datetime"),
                new(brand => brand.TimeGame , L["Time"], "Time"),
                new(brand => brand.Description, L["Description"], "Description"),
                },
            idFunc: brand => brand.Id,
            searchFunc: async filter => (await GamesClient
                .SearchAsync(filter.Adapt<SearchGamesRequest>()))
                .Adapt<PaginationResponse<GameDto>>(),
            createFunc: async brand => await GamesClient.CreateAsync(brand.Adapt<CreateGameRequest>()),
            updateFunc: async (id, brand) => await GamesClient.UpdateAsync(id, brand),
            deleteFunc: async id => await GamesClient.DeleteAsync(id),
            exportAction: string.Empty);
    public void ShowGameDialog(ISnackbar snack)
    {
        snack.Add("Open game dialg", Severity.Info);
        Context.AddEditModal.ForceRender();
    }
}
