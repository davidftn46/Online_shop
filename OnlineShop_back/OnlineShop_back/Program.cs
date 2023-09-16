using Microsoft.EntityFrameworkCore;
using DataAccesLayer.Context;
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.Services.SrvcImplementations;
using DataAccesLayer.Repository.IRepository;
using Addition.Mutual;
using DataAccesLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using BusinessLogicLayer.Services.Map;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<EmailStructure>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddSingleton<IShipmentService, ShipmentService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddGoogle("Google", options =>
{
    options.ClientId = "1096475234419-442tf3u29nsd8qi50qh6rmfe5i1sdqjk.apps.googleusercontent.com";
    options.ClientSecret = "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";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters 
    {
        ValidateIssuer = true, 
        ValidateAudience = false, 
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true, 
        ValidIssuer = "http://localhost:44327", 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]))
    };
});


var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MapProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod();
        });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();


app.MapControllers();

app.Run();
