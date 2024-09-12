
using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Auth;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.WebApi.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;
public partial class Games
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewTenants;
    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants;
    protected override async Task OnParametersSetAsync()
    {
        var user = (await AuthState).User;
        _canViewRoles = await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.Roles);
        _canViewUsers = await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.Users);

        _canViewTenants = await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.Tenants);
    }
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
            hasExtraActionsFunc: () => !CanViewAdministrationGroup,
            exportAction: string.Empty);
    public void ShowGameDialog(ISnackbar snack)
    {
        snack.Add("Open game dialg", Severity.Info);
        Context.AddEditModal.ForceRender();
    }
    
}
