using GiocoV1.Dtos;

namespace GiocoV1.Modelli;

public class Cella
{
    public string Nome { get; set; } = "";
    public string Descrizione { get; set; } = "";

    public CollegamentoCella? CollegaA { get; set; }
}