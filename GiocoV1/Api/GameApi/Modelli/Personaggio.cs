namespace GiocoV1.Modelli;

public class Personaggio
{
    public string Nome { get; set; } = string.Empty;
    public int SaluteMassima { get; set; }
    public int SaluteAttuale { get; set; }
    public int Attacco { get; set; }
    public int Difesa { get; set; }
    public int Velocita { get; set; }
    public int Livello { get; set; } = 1;
    public int Esperienza { get; set; } = 0;
    public List<Mossa> Mosse { get; set; } = new();
    public List<OggettoInventario> Inventario { get; set; } = new();
    public Equipaggiamento Equipaggiamenti { get; set; } = new();
    public int Monete { get; set; } = 0;
    public int PuntiAbilita { get; set; } = 0;

    public int PosX { get; set; }
    public int PosY { get; set; }
}