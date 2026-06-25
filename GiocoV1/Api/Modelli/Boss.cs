using GiocoV1.Dtos;

namespace GiocoV1.Modelli;

public class Boss : Entita
{
    public int Salute { get; set; }
    public int Attacco { get; set; }
    public int Difesa { get; set; }
    public int Velocita { get; set; }
    public int Livello { get; set; }

    // Nomi delle mosse dal JSON
    public List<string> NomiMosse { get; set; } = new();

    // Mosse reali assegnate dal ServizioNemici
    public List<Mossa> Mosse { get; set; } = new();
    
    public List<OggettoInventario> Inventario { get; set; } = new();
    public List<OggettoInventarioJson> NomiOggetti { get; set; } = new();

    public bool Sconfitto { get; set; } = false; // per non respawnare
}