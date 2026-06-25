using GiocoV1.Modelli;

namespace GiocoV1.Configurazioni;

public class CellaNpc
{
    public int X { get; set; }
    public int Y { get; set; }
    // Nome dell'NPC da associare a questa cella
    public string Nome { get; set; } = "";
}

public class ConfigSezioneNpc
{
    public List<Npc> Npc { get; set; } = new();
    public List<CellaNpc> CelleNpc { get; set; } = new();
}

public class ConfigNpc
{
    public Dictionary<string, ConfigSezioneNpc> Sezioni { get; set; } = new();
}