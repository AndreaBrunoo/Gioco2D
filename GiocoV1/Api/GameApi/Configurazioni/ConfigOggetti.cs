using GiocoV1.Modelli;

namespace GiocoV1.Configurazioni;

public class ConfigOggetti
{
    public List<Oggetto> Consumabili { get; set; } = new();
    public List<Oggetto> Offensivi { get; set; } = new();
    public List<Oggetto> Materiali { get; set; } = new();
    public EquipaggiamentiConfig Equipaggiamenti { get; set; } = new();
}

public class EquipaggiamentiConfig
{
    public List<Oggetto> Elmi { get; set; } = new();
    public List<Oggetto> Corazze { get; set; } = new();
    public List<Oggetto> Gambali { get; set; } = new();
    public List<Oggetto> Stivali { get; set; } = new();
    public List<Oggetto> Armi { get; set; } = new();
}