var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".MySession";
    options.IdleTimeout = TimeSpan.FromSeconds(3600);
});

// Configure Kestrel to use HTTPS
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Set up the HTTP listener
    serverOptions.ListenAnyIP(8080);

    // Set up the HTTPS listener
    serverOptions.ListenAnyIP(8081, listenOptions =>
    {
        // Point to the actual path where the certificate and key are stored
        listenOptions.UseHttps("/home/app/certs/cert.pfx", "EtherealDatabaseStorage!!");
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();
app.MapControllers();

app.Run();
