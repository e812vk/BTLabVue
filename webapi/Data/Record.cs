using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json.Serialization;

namespace webapi.Data;

public enum Reason
{
    Reason1 = 0,
    Reason2 = 1,
    Reason3 = 2
}
public record Record
{
    public int id { get; set; }
    public Reason reason { get; set; }
    public DateOnly start_date { get; set; }
    public int duration { get; set; }
    public bool discounted { get; set; }
    public string? description { get; set; }
}
