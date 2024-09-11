using FSH.BlazorWebAssembly.Client.Infrastructure.Notifications;
using FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;
using FSH.BlazorWebAssembly.Client.Infrastructure.Theme;
using FSH.WebApi.Shared.Notifications;
using MediatR.Courier;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Shared;

public partial class BaseLayout
{

    [Inject]
    private ICourier Courier { get; set; } = default!;
    private ClientPreference? _themePreference;
    private MudTheme _currentTheme = new LightTheme();
    private bool _themeDrawerOpen;
    private bool _rightToLeft;

    protected override async Task OnInitializedAsync()
    {
        _themePreference = await ClientPreferences.GetPreference() as ClientPreference;
        if (_themePreference == null) _themePreference = new ClientPreference();
        SetCurrentTheme(_themePreference);

        Snackbar.Add("Like this boilerplate? ", Severity.Normal, config =>
        {
            config.BackgroundBlurred = true;
            config.Icon = Icons.Custom.Brands.GitHub;
            config.Action = "Star us on Github!";
            config.ActionColor = Color.Primary;
            config.Onclick = snackbar =>
            {
                Navigation.NavigateTo("https://github.com/fullstackhero/blazor-wasm-boilerplate");
                return Task.CompletedTask;
            };
        });
        Courier.SubscribeWeak<NotificationWrapper<BasicNotification>>(async _ =>
        {
            Snackbar.Add(_.Notification.Message,(Severity)_.Notification.Label);
            StateHasChanged();
        });
    }

    private async Task ThemePreferenceChanged(ClientPreference themePreference)
    {
        SetCurrentTheme(themePreference);
        await ClientPreferences.SetPreference(themePreference);
    }

    private void SetCurrentTheme(ClientPreference themePreference)
    {
        _currentTheme = themePreference.IsDarkMode ? new DarkTheme() : new LightTheme();
        if (themePreference.IsDarkMode)
        {
            _currentTheme.PaletteDark.Primary = themePreference.PrimaryColor;
            _currentTheme.PaletteDark.Secondary = themePreference.SecondaryColor;
        }else
        {
            _currentTheme.PaletteLight.Primary = themePreference.PrimaryColor;
            _currentTheme.PaletteLight.Secondary = themePreference.SecondaryColor;
        }
        _currentTheme.LayoutProperties.DefaultBorderRadius = $"{themePreference.BorderRadius}px";
        _currentTheme.LayoutProperties.DefaultBorderRadius = $"{themePreference.BorderRadius}px";
        _rightToLeft = themePreference.IsRTL;
    }
}