using GiocoV1.Modelli;
using GiocoV1.Configurazioni;
using GiocoV1.Dtos;

namespace GiocoV1.Servizi;

public class ServizioMovimento
{
    private readonly Cella?[,] _griglia;

    public ServizioMovimento(Cella?[,] griglia) { _griglia = griglia; }

    public RisultatoMovimento Muovi(Personaggio personaggio, char direzione, Sezione sezione, ConfigNemici configNemici)
    {
        var cellaAttuale = _griglia[personaggio.PosY, personaggio.PosX]!;

        // 🔹 Se il tasto non è valido → non muovere e mostra la cella attuale
        if (!"wasdWASD".Contains(direzione))
        {
            var risultato = new RisultatoMovimento();
            ImpostaMessaggiBase(risultato, cellaAttuale);
            risultato.NemicoTrovato = null;
            return risultato;
        }

        int nuovoX = personaggio.PosX;
        int nuovoY = personaggio.PosY;

        if (direzione == 'w' || direzione == 'W') nuovoY--;
        if (direzione == 's' || direzione == 'S') nuovoY++;
        if (direzione == 'd' || direzione == 'D') nuovoX++;
        if (direzione == 'a' || direzione == 'A') nuovoX--;

        if (nuovoX < 0 || nuovoX >= _griglia.GetLength(1) ||
            nuovoY < 0 || nuovoY >= _griglia.GetLength(0))
        {
            var risultato = new RisultatoMovimento();
            ImpostaMessaggiBase(risultato, cellaAttuale);
            risultato.NemicoTrovato = null;
            return risultato;
        }

        if (_griglia[nuovoY, nuovoX] == null)
        {
            var risultato = new RisultatoMovimento();
            ImpostaMessaggiBase(risultato, cellaAttuale);
            risultato.NemicoTrovato = null;
            return risultato;
        }

        personaggio.PosX = nuovoX;
        personaggio.PosY = nuovoY;
        var cella = _griglia[nuovoY, nuovoX]!;

        var cellaPosizionata = new CellaPosizionata
        {
            X = nuovoX,
            Y = nuovoY,
            Nome = cella.Nome,
            Descrizione = cella.Descrizione
        };
        var risultato2 = new RisultatoMovimento();

        if (cella.CollegaA != null)
        {
            ImpostaMessaggiBase(risultato2, cella);
            risultato2.Collegamento = cella.CollegaA;
            return risultato2;
        }

        // ⭐ TENTA LO SPAWN ⭐
        var entita = ServizioNemici.TentaSpawnNemico(sezione, cellaPosizionata, configNemici);

        ImpostaMessaggiBase(risultato2, cella);

        if (entita != null)
        {
            risultato2.NemicoTrovato = entita;
            return risultato2;
        }

        risultato2.NemicoTrovato = null;
        return risultato2;
    }

    private static void ImpostaMessaggiBase(RisultatoMovimento risultato, Cella cella)
    {
        risultato.Messaggio1 = $"Ti trovi in: {cella.Nome}.";
        risultato.Descrizione = cella.Descrizione;
    }
}