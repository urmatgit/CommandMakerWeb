using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Reflection.Emit;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;

public class GameAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<GameAutocomplete> L { get; set; } = default!;
    [Inject]
    private IGamesClient GamesClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<GameDto> _games = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Game"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchGames;
        ToStringFunc = GetGameName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }

    // when the value parameter is set, we have to load that one game to be able to show the name
    // we can't do that in OnInitialized because of a strange bug (https://github.com/MudBlazor/MudBlazor/issues/3818)
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender &&
            _value != default &&
            await ApiHelper.ExecuteCallGuardedAsync(
                () => GamesClient.GetAsync(_value), Snackbar) is { } game)
        {
            _games.Add(game);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<Guid>> SearchGames(string value)
    {
        var filter = new SearchGamesRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => GamesClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfGameDto response)
        {
            _games = response.Data.ToList();
        }

        return _games.Select(x => x.Id);
    }

    private string GetGameName(Guid id) =>
        _games.Find(b => b.Id == id)?.Name ?? string.Empty;
}
