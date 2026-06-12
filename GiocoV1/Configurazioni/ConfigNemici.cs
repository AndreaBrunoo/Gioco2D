using GiocoV1.Modelli;

namespace GiocoV1.Configurazioni;

public class CellaBoss
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class ConfigSezioneNemici
{
    public int ProbabilitaSpawn { get; set; }
    public List<Nemico> Nemici { get; set; } = new();
    public Boss? Boss { get; set; }
    public List<CellaBoss> CelleBoss { get; set; } = new();
}

public class ConfigNemici
{
    public Dictionary<string, ConfigSezioneNemici> Sezioni { get; set; } = new();
}