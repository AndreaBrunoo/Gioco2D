using GiocoV1.Enum;
namespace GiocoV1.Modelli;

public class Equipaggiamento
{
    public Mossa?[] MosseEquipaggiate { get; set; } = new Mossa?[3];

    public OggettoInventario? Arma { get; set; }
    public OggettoInventario? Elmo { get; set; }
    public OggettoInventario? Corazza { get; set; }
    public OggettoInventario? Gambali { get; set; }
    public OggettoInventario? Stivali { get; set; }

    public OggettoInventario? Get(CategoriaOggetto categoria)
    {
        return categoria switch
        {
            CategoriaOggetto.Arma => Arma,
            CategoriaOggetto.Elmo => Elmo,
            CategoriaOggetto.Corazza => Corazza,
            CategoriaOggetto.Gambali => Gambali,
            CategoriaOggetto.Stivali => Stivali,
            _ => null
        };
    }

    public void Set(CategoriaOggetto categoria, OggettoInventario? oggetto)
    {
        switch (categoria)
        {
            case CategoriaOggetto.Arma: Arma = oggetto; break;
            case CategoriaOggetto.Elmo: Elmo = oggetto; break;
            case CategoriaOggetto.Corazza: Corazza = oggetto; break;
            case CategoriaOggetto.Gambali: Gambali = oggetto; break;
            case CategoriaOggetto.Stivali: Stivali = oggetto; break;
        }
    }
}