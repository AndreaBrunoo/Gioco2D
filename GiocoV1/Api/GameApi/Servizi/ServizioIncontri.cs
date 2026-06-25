using GiocoV1.Modelli;
using GiocoV1.Enum;

namespace GiocoV1.Servizi;

public static class ServizioIncontri
{
    public enum StatoMenu
    {
        Principale,
        Mosse,
        Inventario,
        Inventario_Pozioni,
        Inventario_Supporti,
        Inventario_Offensivi,
    }

    private static readonly Random random = new();

    /*
    public static EsitoIncontro Incontro(Personaggio personaggio, Entita entita)
    {
        if (entita is Nemico)
        {
            var nemico = (Nemico)entita;
            Console.WriteLine($"Hai incontrato un {nemico.Nome}");
            Console.WriteLine("La battaglia ha inizio!");
            Program.TastoAvanti();
            while (true)
            {
                while (true)
                {
                    Console.WriteLine("[R] Inventario   Combatti [E]");
                    Console.Write("         [Q] Scappa");

                    char t = Console.ReadKey(true).KeyChar;
                    if (t == 'E' || t == 'e') break;
                    if (t == 'Q' || t == 'q')
                    {
                        if (TentaFuga(personaggio, nemico)) return EsitoIncontro.Fuga; ;

                        var mossaNemico = ScegliMossaNemico(nemico);
                        EseguiAttacco(nemico, personaggio, mossaNemico);
                        if (personaggio.SaluteAttuale <= 0) return EsitoIncontro.Sconfitta;
                        continue;
                    }
                    if (t == 'R' || t == 'r')
                    {
                        while (true)
                        {
                            Console.WriteLine("[1] Pozioni    Supporti [2]");
                            Console.WriteLine("[3] Offensivi  Indietro [4]");
                            char m = Console.ReadKey(true).KeyChar;
                            if (m == '1')
                            {
                                // DA FARE
                            }
                            if (m == '2')
                            {
                                // DA FARE
                            }
                            if (m == '3')
                            {
                                // DA FARE
                            }
                            if (m == '4') break;
                        }
                        continue;
                    }
                }
                while (true)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var mossa = personaggio.Equipaggiamenti.MosseEquipaggiate[i];
                        Console.Write($"[{i + 1}]{(mossa?.Nome ?? "---")}");
                    }
                    Console.Write("[Q] Indietro");
                    char scelta = Console.ReadKey(true).KeyChar;
                    if (scelta == 'Q' || scelta == 'q') break;

                    int indice = scelta - '1';
                    if (indice < 0 || indice > 2) continue;

                    Mossa? mossaScelta = personaggio.Equipaggiamenti.MosseEquipaggiate[indice];
                    if (mossaScelta == null) continue;

                    if (personaggio.Velocita >= nemico.Velocita)
                    {
                        EseguiAttacco(personaggio, nemico, mossaScelta);

                        if (nemico.SaluteAttuale <= 0) return EsitoIncontro.Vittoria;

                        Mossa mossaNemico = ScegliMossaNemico(nemico);
                        EseguiAttacco(nemico, personaggio, mossaNemico);
                    }
                    else if (personaggio.Velocita < nemico.Velocita)
                    {
                        Mossa mossaNemico = ScegliMossaNemico(nemico);
                        EseguiAttacco(nemico, personaggio, mossaNemico);

                        if (personaggio.SaluteAttuale <= 0) return EsitoIncontro.Sconfitta;

                        EseguiAttacco(personaggio, nemico, mossaScelta);
                    }
                }
                continue;
            }
        }
        else if (entita is Boss)
        {

        }
        return EsitoIncontro.NessunIncontro;
    } */

    // ------------------------------
    //  COMBATTIMENTO
    // ------------------------------

    public static EsitoIncontro Incontro(Personaggio personaggio, Entita entita)
    {
        if (entita is not Nemico nemico)
            return EsitoIncontro.NessunIncontro;

        StatoMenu stato = StatoMenu.Principale;

        while (true)
        {
            DisegnaUIConNemico(personaggio, nemico, stato);
            char t = Console.ReadKey(true).KeyChar;

            // ------------------------------
            // MENU PRINCIPALE
            // ------------------------------
            if (stato == StatoMenu.Principale)
            {
                if (t == 'E' || t == 'e')
                    stato = StatoMenu.Mosse;

                else if (t == 'R' || t == 'r')
                    stato = StatoMenu.Inventario;

                else if (t == 'Q' || t == 'q')
                {
                    if (TentaFuga(personaggio, nemico))
                    {
                        MostraMessaggioFinale("Sei riuscito a fuggire!", personaggio);
                        return EsitoIncontro.Fuga;
                    }
                    MostraMessaggioCombattimento(personaggio, nemico, "Non si scappa!");
                    var mossaNemico = ScegliMossaNemico(nemico);
                    EseguiAttacco(nemico, personaggio, mossaNemico);

                    if (personaggio.SaluteAttuale <= 0)
                    {
                        MostraMessaggioFinale($"Sei stato sconfitto da {nemico.Nome}...", personaggio);
                        return EsitoIncontro.Sconfitta;
                    }
                }
                continue;
            }

            // ------------------------------
            // MENU MOSSE
            // ------------------------------
            if (stato == StatoMenu.Mosse)
            {
                if (t == 'Q' || t == 'q')
                {
                    stato = StatoMenu.Principale;
                    continue;
                }

                int indice = t - '1';
                if (indice < 0 || indice > 2)
                    continue;

                var mossaScelta = personaggio.Equipaggiamenti.MosseEquipaggiate[indice];
                if (mossaScelta == null)
                    continue;

                // Turno
                if (personaggio.Velocita >= nemico.Velocita)
                {
                    EseguiAttacco(personaggio, nemico, mossaScelta);
                    if (nemico.SaluteAttuale <= 0) return EsitoIncontro.Vittoria;

                    var mossaNemico = ScegliMossaNemico(nemico);
                    EseguiAttacco(nemico, personaggio, mossaNemico);
                    if (personaggio.SaluteAttuale <= 0)
                    {
                        MostraMessaggioFinale($"Sei stato sconfitto da {nemico.Nome}...", personaggio);
                        return EsitoIncontro.Sconfitta;
                    }
                }
                else
                {
                    var mossaNemico = ScegliMossaNemico(nemico);
                    EseguiAttacco(nemico, personaggio, mossaNemico);
                    if (personaggio.SaluteAttuale <= 0)
                    {
                        MostraMessaggioFinale($"Sei stato sconfitto da {nemico.Nome}...", personaggio);
                        return EsitoIncontro.Sconfitta;
                    }

                    EseguiAttacco(personaggio, nemico, mossaScelta);
                    if (nemico.SaluteAttuale <= 0) return EsitoIncontro.Vittoria;
                }

                continue;
            }

            // ------------------------------
            // MENU INVENTARIO
            // ------------------------------
            if (stato == StatoMenu.Inventario)
            {
                if (t == '1') stato = StatoMenu.Inventario_Pozioni;
                else if (t == '2') stato = StatoMenu.Inventario_Supporti;
                else if (t == '3') stato = StatoMenu.Inventario_Offensivi;
                else if (t == 'Q' || t == 'q') stato = StatoMenu.Principale;
                continue;
            }

            // ------------------------------
            // SOTTO-MENU INVENTARIO
            // ------------------------------
            if (t == 'Q' || t == 'q')
            {
                stato = StatoMenu.Inventario;
                continue;
            }
        }
    }

    // ------------------------------
    //  UI DINAMICA
    // ------------------------------
    static void MostraMessaggioCombattimento(Personaggio personaggio, Nemico nemico, string messaggio)
    {
        // 1) Ridisegna tutta la UI
        Console.Clear();
        DisegnaNemico(nemico);
        DisegnaGiocatore(personaggio);

        // 2) Scrivi il messaggio al centro
        int centerX = Console.WindowWidth / 2 - messaggio.Length / 2;
        int centerY = Console.WindowHeight / 2;

        Console.SetCursorPosition(centerX, centerY);
        Console.Write(messaggio);

        // 3) Aspetta input
        Program.TastoAvanti();

        // 4) Ridisegna la UI normale
        Console.Clear();
        DisegnaNemico(nemico);
        DisegnaGiocatore(personaggio);
    }

    static void DisegnaUIConNemico(Personaggio p, Nemico n, StatoMenu stato)
    {
        Console.Clear();
        DisegnaNemico(n);
        DisegnaGiocatore(p);
        DisegnaMenuCombattimento(stato, p);
    }

    static void DisegnaMenuCombattimento(StatoMenu stato, Personaggio p)
    {
        int center = Console.WindowWidth / 2 - 15;
        int middle = Console.WindowHeight / 2;

        switch (stato)
        {
            case StatoMenu.Principale:
                Console.SetCursorPosition(center, middle);
                Console.Write("[R] Inventario   Combatti [E]");
                Program.StampaTasto("[Q] Scappa");
                break;

            case StatoMenu.Mosse:
                Console.SetCursorPosition(center, middle);
                for (int i = 0; i < 3; i++)
                {
                    var mossa = p.Equipaggiamenti.MosseEquipaggiate[i];
                    Console.Write($"[{i + 1}] {(mossa?.Nome ?? "---")}   ");
                }
                Program.StampaTasto("[Q] Indietro");
                break;

            case StatoMenu.Inventario:
                Console.SetCursorPosition(center, middle);
                Console.Write("[1] Pozioni    [2] Supporti");
                Console.SetCursorPosition(center, middle + 1);
                Console.Write("      [3] Offensivi");
                Program.StampaTasto("[Q] Indietro");
                break;

            case StatoMenu.Inventario_Pozioni:
                Console.SetCursorPosition(center, middle);
                Console.Write("Pozioni disponibili:");
                // Qui stamperai le pozioni
                Program.StampaTasto("[Q] Indietro");
                break;

            case StatoMenu.Inventario_Supporti:
                Console.SetCursorPosition(center, middle);
                Console.Write("Supporti disponibili:");
                Program.StampaTasto("[Q] Indietro");
                break;

            case StatoMenu.Inventario_Offensivi:
                Console.SetCursorPosition(center, middle);
                Console.Write("Oggetti offensivi:");
                Program.StampaTasto("[Q] Indietro");
                break;
        }
    }

    public static void DisegnaGiocatore(Personaggio p)
    {
        string barra = BarraHP(p.SaluteAttuale, p.SaluteMassima);

        int top = Console.WindowHeight - 5;

        Console.SetCursorPosition(1, top);
        Console.Write($"┌──────────────────────────┐");

        Console.SetCursorPosition(1, top + 1);
        Console.Write($"│ {p.Nome}  Lv.{p.Livello}");

        Console.SetCursorPosition(1, top + 2);
        Console.Write($"│ HP: {barra}");

        Console.SetCursorPosition(1, top + 3);
        Console.Write($"└──────────────────────────┘");
    }

    static void DisegnaNemico(Nemico nemico)
    {
        string barra = BarraHP(nemico.SaluteAttuale, nemico.SaluteMassima);

        int left = Console.WindowWidth - 30; // sposta a destra

        Console.SetCursorPosition(left, 1);
        Console.Write($"┌──────────────────────────┐");

        Console.SetCursorPosition(left, 2);
        Console.Write($"│ {nemico.Nome}  Lv.{nemico.Livello} ");

        Console.SetCursorPosition(left, 3);
        Console.Write($"│ HP: {barra} ");

        Console.SetCursorPosition(left, 4);
        Console.Write($"└──────────────────────────┘");
    }

    static string BarraHP(int hp, int hpMax, int lunghezza = 20)
    {
        if (hp < 0) hp = 0;
        if (hp > hpMax) hp = hpMax;
        if (hpMax <= 0) hpMax = 1;

        double percentuale = (double)hp / hpMax;

        int pieni = (int)(percentuale * lunghezza);
        if (pieni < 0) pieni = 0;
        if (pieni > lunghezza) pieni = lunghezza;

        int vuoti = lunghezza - pieni;

        return new string('█', pieni) + new string('░', vuoti);
    }

    static void MostraMessaggioFinale(string messaggio, Personaggio personaggio)
    {
        Console.Clear();
        DisegnaGiocatore(personaggio);
        int centerX = Console.WindowWidth / 2 - messaggio.Length / 2;
        int centerY = Console.WindowHeight / 2;

        Console.SetCursorPosition(centerX, centerY);
        Console.Write(messaggio);

        Program.TastoAvanti();
        Console.Clear();
    }

    // ------------------------------
    //  METODI DI COMBATTIMENTO
    // ------------------------------

    public static void EseguiAttacco(object attaccante, object bersaglio, Mossa mossa)
    {
        int danno = CalcolaDanno(attaccante, bersaglio, mossa);

        Personaggio p = attaccante as Personaggio
                    ?? bersaglio as Personaggio
                    ?? throw new Exception("Nessun personaggio trovato nel combattimento!");

        Nemico n = attaccante as Nemico
                   ?? bersaglio as Nemico
                   ?? throw new Exception("Nessun nemico trovato nel combattimento!");

        if (danno == 0)
        {
            MostraMessaggioCombattimento(p, n, $"{Nome(attaccante)} usa {mossa.Nome} ma manca il bersaglio!");
            return;
        }

        if (bersaglio is Personaggio personaggio) personaggio.SaluteAttuale -= danno;
        else if (bersaglio is Nemico nemico) nemico.SaluteAttuale -= danno;

        MostraMessaggioCombattimento(p, n, $"{Nome(attaccante)} usa {mossa.Nome} e infligge {danno} danni!");
    }

    public static int CalcolaDanno(object attaccante, object bersaglio, Mossa mossa)
    {
        int attacco = 0;
        int difesa = 0;

        // --- 1. Estrazione statistiche attaccante ---
        if (attaccante is Personaggio personaggio)
            attacco = personaggio.Attacco;
        else if (attaccante is Nemico nemico)
            attacco = nemico.Attacco;

        // --- 2. Estrazione statistiche bersaglio ---
        if (bersaglio is Personaggio personaggio2)
            difesa = personaggio2.Difesa;
        else if (bersaglio is Nemico nemico2)
            difesa = nemico2.Difesa;

        // --- 3. Precisione ---
        if (random.Next(1, 101) > mossa.PrecisioneBase)
            return 0; // attacco mancato

        // --- 4. Critico ---
        bool critico = random.Next(1, 101) <= mossa.ProbabilitaCritico;

        // --- 5. Danno base ---
        int dannoBase = attacco + mossa.PotenzaBase;

        // --- 6. Riduzione difesa ---
        int dannoFinale = dannoBase - (difesa / 2);
        if (dannoFinale < 1)
            dannoFinale = 1;

        // --- 7. Critico ---
        if (critico)
            dannoFinale = (int)(dannoFinale * 1.5);

        return dannoFinale;
    }

    static string Nome(object entita)
    {
        return entita switch
        {
            Personaggio p => p.Nome,
            Nemico n => n.Nome,
            _ => "Sconosciuto"
        };
    }

    static Mossa ScegliMossaNemico(Nemico nemico)
    {
        int index = random.Next(0, nemico.Mosse.Count);
        return nemico.Mosse[index];
    }

    static bool TentaFuga(Personaggio personaggio, Nemico nemico)
    {
        int fuga = random.Next(1, 101);
        int differenzaLivello = personaggio.Livello - nemico.Livello;

        int probabilitaBase = 30;
        int bonus = differenzaLivello > 0 ? differenzaLivello * 5 : 0;
        int probabilitaFinale = probabilitaBase + bonus;

        if (probabilitaFinale > 95) probabilitaFinale = 95;
        return fuga <= probabilitaFinale;
    }
}