using VotingApp.Common;
using VotingApp.Server.Domain.Repositories;
using VotingApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IVotingService, VotingService>();
builder.Services.AddSingleton<RsaEncryption>(new RsaEncryption());
builder.Services.AddSingleton<SigratureService>(new SigratureService());
builder.Services.AddSingleton<IVoterRepository>(new VoterRepository());
builder.Services.AddSingleton<ICandidateRepository>(new CandidateRepository());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();