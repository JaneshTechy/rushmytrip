using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Country
{
    public string alpha_2 { get; set; }
    public string alpha_3 { get; set; }
    public string name { get; set; }
    public string numeric { get; set; }
    public string official_name { get; set; }
}

public class CountryWrapper
{
    [JsonPropertyName("3166-1")]
    public List<Country> Countries { get; set; }
}

public class State
{
    public string code { get; set; }
    public string name { get; set; }
    public string type { get; set; }
}

public class StateWrapper
{
    [JsonPropertyName("3166-2")]
    public List<State> States { get; set; }
}
