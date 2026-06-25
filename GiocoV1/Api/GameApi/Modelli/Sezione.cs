namespace GiocoV1.Modelli;

public class Sezione
{
    public string Nome { get; set; } = "";
    public int Larghezza { get; set; }
    public int Altezza { get; set; }
    public List<CellaPosizionata> Celle { get; set; } = new();
    public List<Entita> EntitaPresenti { get; set; } = new();
}