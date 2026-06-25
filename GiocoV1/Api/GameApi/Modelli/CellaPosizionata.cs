using GiocoV1.Dtos;

namespace GiocoV1.Modelli;

public class CellaPosizionata
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Nome { get; set; } = "";
    public string Descrizione { get; set; } = "";
    public CollegamentoCella? CollegaA { get; set; }
}