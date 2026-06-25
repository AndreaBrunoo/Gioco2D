using GiocoV1.Modelli;

namespace GiocoV1.Servizi;

public class ServizioClassi
{
    public List<ClassePersonaggio> ClassiDisponibili => new()
        {
            new ClassePersonaggio
            {
                Nome = "Guerriero",
                Salute = 120,
                Attacco = 15,
                Difesa = 10,
                Velocita = 5
            },
            new ClassePersonaggio
            {
                Nome = "Mago",
                Salute = 80,
                Attacco = 25,
                Difesa = 5,
                Velocita = 10
            },
            new ClassePersonaggio
            {
                Nome = "Ladro",
                Salute = 90,
                Attacco = 12,
                Difesa = 7,
                Velocita = 20
            }
        };

    public void ApplicaClasse(Personaggio p, ClassePersonaggio classe)
    {
        p.SaluteMassima = classe.Salute;
        p.SaluteAttuale = classe.Salute;
        p.Attacco = classe.Attacco;
        p.Difesa = classe.Difesa;
        p.Velocita = classe.Velocita;
    }
}