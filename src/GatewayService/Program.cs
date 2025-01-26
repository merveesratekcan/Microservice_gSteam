using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt=>{
    opt.Authority = builder.Configuration["AuthorirtyServiceUrl"];
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters.ValidateAudience = false;
    opt.TokenValidationParameters.NameClaimType = "name";
});

app.MapGet("/", () => "Hello World!");
app.UseAuthentication();
app.UseAuthorization();
app.Run();
