using GiocoV1.Dtos;

namespace GiocoV1.Modelli;

public class Nemico : Entita
{
    public int SaluteAttuale { get; set; } = 1;
    public int SaluteMassima { get; set; } = 1;
    public int Attacco { get; set; } = 1;
    public int Difesa { get; set; } = 1;
    public int Velocita { get; set; } = 1;
    public int Livello { get; set; } = 1;
    public int ProbabilitaSpawn { get; set; } = 0;

    // Nomi delle mosse dal JSON
    public List<string> NomiMosse { get; set; } = new();

    // Mosse reali assegnate dal ServizioNemici
    public List<Mossa> Mosse { get; set; } = new();

    public List<OggettoInventario> Inventario { get; set; } = new();
    public List<OggettoInventarioJson> NomiOggetti { get; set; } = new();
}