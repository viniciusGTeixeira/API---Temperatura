using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using Temperatura_Api.Models;

namespace Temperatura_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public WeatherController(IConfiguration configuration){
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        [HttpGet("cidades-com-codigo")]
        public async Task<IActionResult> GetAllCitiesWithCodes(){
            try{
                var url = "https://brasilapi.com.br/api/cptec/v1/cidade";

                using var client = new HttpClient();
                var response = await client.GetAsync(url);

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

                return Ok(cities);
            }catch (HttpRequestException ex){
                return StatusCode(500, $"Erro ao recuperar dados: {ex.Message}");
            }catch (Exception ex){
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }

        [HttpGet("cidade/{cityCode}")]
        public async Task<IActionResult> GetWeatherByCity(string cityCode){
            try{
                var url = $"https://brasilapi.com.br/api/cptec/v1/clima/previsao/{cityCode}";

                using var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode){
                    return StatusCode((int)response.StatusCode, $"Erro ao recuperar dados: {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var weather = JsonSerializer.Deserialize<WeatherCityDto>(content, options);

                return Ok(weather);
            }catch (HttpRequestException ex){
                return StatusCode(500, $"Erro ao recuperar dados: {ex.Message}");
            }catch (Exception ex){
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }

        [HttpGet("aeroporto/{icaoCode}")]
        public async Task<IActionResult> GetWeatherByAirport(string icaoCode)
        {
            try{
                var url = $"https://brasilapi.com.br/api/cptec/v1/clima/aeroporto/{icaoCode}";

                using var client = new HttpClient();
                var response = await client.GetAsync(url);

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

                return Ok(weather);
            }
            catch (HttpRequestException ex){
                return StatusCode(500, $"Erro ao recuperar dados: {ex.Message}");
            }catch (Exception ex){
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }

        [HttpGet("test-Conexao")]
        public async Task<IActionResult> TestConnection()
        {
            try{
                using (var connection = new SqlConnection(_dbConnection.ConnectionString))
                {
                    await connection.OpenAsync();
                    return Ok("Conexão estabelecida com sucesso.");
                }
            }catch (Exception ex){
                return StatusCode(500, $"Erro ao conectar ao banco de dados: {ex.Message}");
            }
        }
    }
}
