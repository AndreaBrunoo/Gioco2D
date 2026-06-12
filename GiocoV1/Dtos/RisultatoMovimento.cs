using GiocoV1.Modelli;

namespace GiocoV1.Dtos;

public class RisultatoMovimento
{
    public string Messaggio1 { get; set; } = "";
    public string Descrizione { get; set; } = "";
    public Entita? NemicoTrovato { get; set; } = null;
    public CollegamentoCella? Collegamento { get; set; }
}