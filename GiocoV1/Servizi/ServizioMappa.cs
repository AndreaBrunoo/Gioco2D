using System.Text.Json;
using GiocoV1.Modelli;

namespace GiocoV1.Servizi;

public static class ServizioMappa
{
    // ---------------------------------------------------------
    // 1) CARICA MAPPA DA FILE JSON
    // ---------------------------------------------------------
    public static Mappa CaricaMappa(string percorso)
    {
        if (!File.Exists(percorso))
            throw new FileNotFoundException($"File JSON non trovato: {percorso}");

        string json = File.ReadAllText(percorso);

        var opzioni = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var mappa = JsonSerializer.Deserialize<Mappa>(json, opzioni);

        if (mappa == null) 
            throw new Exception("Errore nel parsing del JSON: mappa nulla!");

        return mappa;
    }

    // ---------------------------------------------------------
    // 2) CREA LA GRIGLIA 2D DELLA SEZIONE
    // ---------------------------------------------------------
    public static Cella?[,] CreaGriglia(Sezione sezione)
    {
        if (sezione.Larghezza <= 0 || sezione.Altezza <= 0)
            throw new Exception("La sezione ha dimensioni non valide!");

        var griglia = new Cella?[sezione.Altezza, sezione.Larghezza];

        foreach (var c in sezione.Celle)
        {
            // Controllo sicurezza coordinate
            if (c.X < 0 || c.X >= sezione.Larghezza ||
                c.Y < 0 || c.Y >= sezione.Altezza)
            {
                Console.WriteLine($"ATTENZIONE: Cella fuori range ({c.X},{c.Y}) nella sezione {sezione.Nome}");
                continue;
            }

            griglia[c.Y, c.X] = new Cella
            {
                Nome = c.Nome,
                Descrizione = c.Descrizione,
                CollegaA = c.CollegaA
            };
        }
        return griglia;
    }
}