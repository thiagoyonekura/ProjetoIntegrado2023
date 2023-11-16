using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using api.Data;
using api.Models;

namespace api.BackgroundServices
{
    public class ConsultaStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ConsultaStatusBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SistemaContext>();

                    var consultas = context.Consultas
                       .Where(c => c.Status == StatusConsulta.Agendado && c.DataHora < DateTime.UtcNow)
                       .ToList();


                    foreach (var consulta in consultas)
                    {
                        consulta.Status = StatusConsulta.Concluido;
                    }

                    await context.SaveChangesAsync();
                }

                // Aguarda um tempo antes de rodar novamente
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}