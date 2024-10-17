using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SocialApp.Constants;
using SocialApp.Contracts.DataLayer;
using SocialApp.Contracts.DataLayers;
using SocialApp.Contracts.Services;
using SocialApp.Data;
using SocialApp.DataLayers;
using SocialApp.DTOs;
using SocialApp.Middleware;
using SocialApp.Profiles;
using SocialApp.Services;
using SocialApp.Validators;

// You're starting to build the web application. Think of this like gathering all the parts to build a toy set. builder is going to hold all the pieces needed to make your app work.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// This line is saying, "Hey, let's add support for Controllers in our app." Controllers are like the brain of the app. They decide what to do when someone sends a request to your web app
builder.Services.AddControllers();

// This is where you're telling the app, "I need to talk to a database."
// AddDbContext<AppDbContext>: This means you're adding the database context, which is a way for your app to communicate with the database.
// UseNpgsql: This means you're using a PostgreSQL database (Npgsql is a library for talking to PostgreSQL databases).
// builder.Configuration.GetConnectionString(AppSettingsConstants.DBConnection): You’re grabbing the connection string to connect to the DB.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(AppSettingsConstants.DBConnection)));

// These lines are setting up dependency injection
builder.Services.AddScoped<IPostDataLayer, PostDataLayer>();
builder.Services.AddScoped<IUserDataLayer, UserDataLayer>();
builder.Services.AddScoped<ICommentDataLayer, CommentDataLayer>();

builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IUserService, UserController>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<IValidator<CommentCreateDTO>, CommentCreateDTOValidator>();

// This is saying, "When you're sending or receiving JSON (data format like a message), make sure to avoid infinite loops when you have related data."
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// This allows your API to accept requests from different domains, which is necessary when your front-end and back-end are hosted on different servers. You can adjust this to be more restrictive for security.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        build =>
        {
            build.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(CommentProfile), typeof(UserProfile), typeof(PostProfile));


// Now you’re building the app.
WebApplication app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseCors("AllowAllOrigins");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Makes Swagger the home page
    c.DocumentTitle = "Social App";
});

// UseHttpsRedirection() enforces secure connections
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS

// This line tells your app to use all the controllers that were added earlier.
app.MapControllers();

app.Run();

