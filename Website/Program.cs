using MudBlazor;
using MudBlazor.Services;

using Website.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config => {
  // Configuration for a snackbar (or toast) taken from:
  // https://mudblazor.com/components/snackbar#configuration
  config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

  config.SnackbarConfiguration.PreventDuplicates = false;
  config.SnackbarConfiguration.NewestOnTop = false;
  config.SnackbarConfiguration.ShowCloseIcon = true;
  config.SnackbarConfiguration.VisibleStateDuration = 10000;
  config.SnackbarConfiguration.HideTransitionDuration = 500;
  config.SnackbarConfiguration.ShowTransitionDuration = 500;
  config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.Configure<AppSettings>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();