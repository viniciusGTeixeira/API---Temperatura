using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using Temperatura_Api.Models;

namespace Temperatura_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private readonly HttpClient _httpClient;

        public WeatherController(IDbConnection dbConnection, IHttpClientFactory httpClientFactory)
        {
            _dbConnection = dbConnection;
            _httpClient = httpClientFactory.CreateClient();
        }

        private async Task LogError(string message, string? stackTrace)
        {
            var query = "INSERT INTO ErrorLogs (Mensagem, StackTrace) VALUES (@Mensagem, @StackTrace)";
            await _dbConnection.ExecuteAsync(query, new { Mensagem = message, StackTrace = stackTrace ?? "N/A" });
        }

        [HttpGet("cidades-com-codigo")]
        public async Task<IActionResult> GetAllCitiesWithCodes()
        {
            try
            {
                var url = "https://brasilapi.com.br/api/cptec/v1/cidade";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Erro ao recuperar dados: {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var cities = JsonSerializer.Deserialize<List<CityDto>>(content, options);

                if (cities == null)
                {
                    return BadRequest("Dados das cidades não foram retornados corretamente.");
                }

                using (var transaction = _dbConnection.BeginTransaction())
                {
                    try
                    {
                        foreach (var city in cities)
                        {
                            var query = "INSERT INTO CityData (Nome, Estado) VALUES (@Nome, @Estado)";
                            await _dbConnection.ExecuteAsync(query, new { Nome = city.Nome, Estado = city.Estado }, transaction);
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                return Ok(cities);
            }
            catch (HttpRequestException ex)
            {
                await LogError(ex.Message, ex.StackTrace);
                return StatusCode(500, $"Erro ao recuperar dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                await LogError(ex.Message, ex.StackTrace);
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }

        [HttpGet("cidade/{cityCode}")]
        public async Task<IActionResult> GetWeatherByCity(string cityCode)
        {
            try
            {
                var url = $"https://brasilapi.com.br/api/cptec/v1/clima/previsao/{cityCode}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Erro ao recuperar dados: {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var weather = JsonSerializer.Deserialize<WeatherCityDto>(content, options);

                if (weather == null)
                {
                    return BadRequest("Dados do clima não foram retornados corretamente.");
                }

                using (var transaction = _dbConnection.BeginTransaction())
                {
                    try
                    {
                        foreach (var clima in weather.Clima)
                        {
                            var query = "INSERT INTO WeatherCityData (Cidade, Estado, AtualizadoEm, Data, Condicao, CondicaoDesc, Min, Max, IndiceUv) VALUES (@Cidade, @Estado, @AtualizadoEm, @Data, @Condicao, @CondicaoDesc, @Min, @Max, @IndiceUv)";
                            await _dbConnection.ExecuteAsync(query, new
                            {
                                Cidade = weather.Cidade,
                                Estado = weather.Estado,
                                AtualizadoEm = weather.AtualizadoEm,
                                Data = clima.Data,
                                Condicao = clima.Condicao,
                                CondicaoDesc = clima.CondicaoDesc,
                                Min = clima.Min,
                                Max = clima.Max,
                                IndiceUv = clima.IndiceUv
                            }, transaction);
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                return Ok(weather);
            }
            catch (HttpRequestException ex)
            {
                await LogError(ex.Message, ex.StackTrace);
                return StatusCode(500, $"Erro ao recuperar dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                await LogError(ex.Message, ex.StackTrace);
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }

        [HttpGet("aeroporto/{icaoCode}")]
        public async Task<IActionResult> GetWeatherByAirport(string icaoCode)
        {
            try
            {
                var url = $"https://brasilapi.com.br/api/cptec/v1/clima/aeroporto/{icaoCode}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Erro ao recuperar dados: {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var weather = JsonSerializer.Deserialize<WeatherAirportDto>(content, options);

                if (weather == null)
                {
                    return BadRequest("Dados do clima do aeroporto não foram retornados corretamente.");
                }

                var query = "INSERT INTO WeatherAirportData (CodigoIcao, AtualizadoEm, PressaoAtmosferica, Visibilidade, Vento, DirecaoVento, Umidade, Condicao, CondicaoDesc, Temp) VALUES (@CodigoIcao, @AtualizadoEm, @PressaoAtmosferica, @Visibilidade, @Vento, @DirecaoVento, @Umidade, @Condicao, @CondicaoDesc, @Temp)";
                await _dbConnection.ExecuteAsync(query, new
                {
                    CodigoIcao = weather.codigo_icao,
                    AtualizadoEm = weather.atualizado_em,
                    PressaoAtmosferica = weather.pressao_atmosferica,
                    Visibilidade = weather.Visibilidade,
                    Vento = weather.Vento,
                    DirecaoVento = weather.direcao_vento,
                    Umidade = weather.Umidade,
                    Condicao = weather.Condicao,
                    CondicaoDesc = weather.condicao_Desc,
                    Temp = weather.Temp
                });

                return Ok(weather);
            }
            catch (HttpRequestException ex)
            {
                await LogError(ex.Message, ex.StackTrace);
                return StatusCode(500, $"Erro ao recuperar dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                await LogError(ex.Message, ex.StackTrace);
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }

        [HttpGet("test-Conexao")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                if (_dbConnection is SqlConnection sqlConnection)
                {
                    await sqlConnection.OpenAsync();
                }
                else
                {
                    return StatusCode(500, "Conexão não é do tipo esperado.");
                }
                return Ok("Conexão estabelecida com sucesso.");
            }
            catch (Exception ex)
            {
                await LogError(ex.Message, ex.StackTrace);
                return StatusCode(500, $"Erro ao conectar ao banco de dados: {ex.Message}");
            }
        }


    }
}
