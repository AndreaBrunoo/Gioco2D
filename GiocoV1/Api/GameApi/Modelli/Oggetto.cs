using GiocoV1.Enum;

namespace GiocoV1.Modelli;

public class Oggetto
{
    public string Nome { get; set; } = string.Empty;
    public int BonusAttacco { get; set; }
    public int BonusDifesa { get; set; }
    public int BonusVelocita { get; set; }
    public int BonusSalute { get; set; }
    public int ProbabilitaDrop { get; set; }
    public CategoriaOggetto Categoria { get; set; }
}