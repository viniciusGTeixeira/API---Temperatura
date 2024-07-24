namespace Temperatura_Api.Models
{
    public class CityDto{
        public string Nome { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class Clima{
        public string Data { get; set; } = string.Empty;
        public string Condicao { get; set; } = string.Empty;
        public string CondicaoDesc { get; set; } = string.Empty;
        public int Min { get; set; }
        public int Max { get; set; }
        public int IndiceUv { get; set; }
    }

    public class WeatherCityDto{
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
        public List<Clima> Clima { get; set; } = new List<Clima>();
    }

    public class WeatherAirportDto{
        public string codigo_icao { get; set; } = string.Empty;
        public string atualizado_em { get; set; } = string.Empty;
        public int pressao_atmosferica { get; set; } // Alterado para int
        public string Visibilidade { get; set; } = string.Empty;
        public int Vento { get; set; }
        public int direcao_vento { get; set; }
        public int Umidade { get; set; }
        public string Condicao { get; set; } = string.Empty;
        public string condicao_Desc { get; set; } = string.Empty;
        public int Temp { get; set; }
    }


    public class WeatherForecast{
        public string Date { get; set; } = string.Empty;
        public string Weather { get; set; } = string.Empty;
        public int MinTemperature { get; set; }
        public int MaxTemperature { get; set; }
    }
}
