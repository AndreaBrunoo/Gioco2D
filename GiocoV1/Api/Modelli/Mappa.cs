namespace GiocoV1.Modelli;

public class Mappa
{
    public string Nome { get; set; } = "";
    public List<Sezione> Sezioni { get; set; } = new();
}