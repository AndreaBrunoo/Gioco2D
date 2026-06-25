using GiocoV1.Dtos;
namespace GiocoV1.Modelli;

public class Npc : Entita
{
    public List<string> Dialoghi { get; set; } = new();

    // Se false l'NPC non è interagibile
    public bool Interagibile { get; set; } = true;

    // Ruolo (es. mercante, questgiver, ecc.)
    public string Ruolo { get; set; } = "";
    public List<OggettoInventario> Inventario { get; set; } = new();
    public List<OggettoInventarioJson> NomiOggetti { get; set; } = new();
}