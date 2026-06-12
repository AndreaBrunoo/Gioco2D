using GiocoV1.Modelli;
using GiocoV1.Enum;

namespace GiocoV1.Servizi;

public static class ServizioDrop
{
    private static readonly Random random = new Random();

    public static void ApplicaDrop(Personaggio personaggio, Nemico nemico, EsitoIncontro esito)
    {
        if (esito != EsitoIncontro.Vittoria)
            return;

        int esperienza = random.Next(
            nemico.Livello * nemico.Livello * 5,
            nemico.Livello * nemico.Livello * 8
        );

        int monete = random.Next(
            nemico.Livello * 3,
            nemico.Livello * 6
        );

        personaggio.Esperienza += esperienza;
        ApplicaLevelUp(personaggio);
        personaggio.Monete += monete;

        List<string> oggettiOttenuti = new();

        foreach (var possibileDrop in nemico.Inventario)
        {
            int numeroCasuale = random.Next(1, 101);

            if (numeroCasuale <= possibileDrop.Oggetto.ProbabilitaDrop)
            {
                var copia = new OggettoInventario
                {
                    Oggetto = possibileDrop.Oggetto,
                    Quantita = random.Next(1, possibileDrop.Quantita)
                };

                ServizioOggetti.AggiungiOggettoAlPersonaggio(personaggio, copia);
                oggettiOttenuti.Add($"{copia.Oggetto.Nome} x{copia.Quantita}");
            }
        }

        List<string> messaggi = new()
        {
            $"Hai sconfitto {nemico.Nome}!",
            $"Hai ottenuto {esperienza} XP e {monete} Monete"
        };

        if (oggettiOttenuti.Count > 0)
        {
            messaggi.Add("Oggetti:");
            messaggi.AddRange(oggettiOttenuti);
        }
        else messaggi.Add("Nessun oggetto ottenuto");

        MostraRicompenseCentrate(messaggi.ToArray());
    }

    public static int CalcolaXpPerLivello(int livello)
    {
        if (livello >= 100)
            return int.MaxValue;

        return livello * livello * 50;
    }

    public static void ApplicaLevelUp(Personaggio personaggio)
    {
        while (personaggio.Livello < 100 &&
               personaggio.Esperienza >= CalcolaXpPerLivello(personaggio.Livello))
        {
            personaggio.Esperienza -= CalcolaXpPerLivello(personaggio.Livello);
            personaggio.Livello++;
            personaggio.PuntiAbilita += 5;
            personaggio.SaluteAttuale = personaggio.SaluteMassima;
        }
    }

    private static void MostraRicompenseCentrate(string[] righe)
    {
        Console.Clear();

        int centerY = Console.WindowHeight / 2 - righe.Length;

        for (int i = 0; i < righe.Length; i++)
        {
            string riga = righe[i];
            int centerX = Console.WindowWidth / 2 - riga.Length / 2;

            Console.SetCursorPosition(centerX, centerY + i);
            Console.Write(riga);
        }
        Program.TastoAvanti();
        Console.Clear();
    }
}