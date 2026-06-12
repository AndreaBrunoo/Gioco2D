using GiocoV1.Modelli;
using GiocoV1.Servizi;
using GiocoV1.Configurazioni;
using GiocoV1.Dtos;
using GiocoV1.Enum;

class Program
{
    // Cose da fare

    // FARE IL SERVIZIO INCONTRO BOSS
    // FARE IN MODO CHE IL PERSONAGGIO IMPOSTI LA SUA PRIMA MOSSA TRAMITE TUTORIAL PER ORA è AUTOMATICO
    // IMPOSTARE UN ARMA PREDEFINITA TRAMITE CLASSE PERSONAGGIO
    // FINIRE L'OPZIONE INVENTARIO DURANTE IL COMBATTIMENTO
    // FINIRE LA DIVISIONE DELL'INVENTARIO NEL MENU
    // GESTIRE I PUNTI ABILITA
    // INIZIARE NPC

    // Appunti

    // Cancella solo la riga del prompt, non tutto lo schermo
    // Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");

    static void Main()
    {
        while (true)
        {
            var servizioSalvataggio = new ServizioSalvataggio();
            StatoGioco? stato = null;

            // ============================
            //       MENU INIZIALE
            // ============================
            while (true)
            {
                Console.Clear();
                string[] righe =
                {
                    "===== MENU INIZIALE =====",
                    "",
                    "[1] Nuova Partita",
                    "[2] Carica Partita",
                    "[3] Chiudi "
                };
                CicloForPerStampaCentrale(righe);

                char scelta = Console.ReadKey(true).KeyChar;
                Console.Clear();

                if (scelta == '1')
                {
                    Console.Clear();
                    MostraMessaggioCentratoWrite("Iniziare nuova partita?");

                    bool decisioneInizio = TastiSiENo();
                    if (decisioneInizio)
                    {
                        stato = NuovaPartita();
                        break;
                    }
                    else continue;
                }
                else if (scelta == '2')
                {
                    stato = MenuCaricamento(servizioSalvataggio);
                    if (stato != null) break;
                }
                else if (scelta == '3') return;
            }
            AvviaGioco(stato);
        }
    }

    // ============================
    //       NUOVA PARTITA
    // ============================
    static StatoGioco NuovaPartita()
    {
        Console.Clear();
        // 1) CREAZIONE MAPPA
        var mappa = ServizioMappa.CaricaMappa("Mappa_principale.json");
        var sezione = mappa.Sezioni.FirstOrDefault(s => s.Nome == "Foresta Tutorial") ?? mappa.Sezioni.First();
        var griglia = ServizioMappa.CreaGriglia(sezione);
        var movimento = new ServizioMovimento(griglia);

        // 2) CREAZIONE PERSONAGGIO
        string nome;
        do
        {
            MostraMessaggioCentratoWrite("Scrivi il nome del tuo personaggio: ");
            nome = Console.ReadLine()!.Trim();
            if (!string.IsNullOrEmpty(nome))
                nome = char.ToUpper(nome[0]) + nome.Substring(1).ToLower();
            Console.Clear();
        }
        while (string.IsNullOrEmpty(nome));

        var personaggio = new Personaggio
        {
            Nome = nome,
            PosX = 5,
            PosY = 5,
            Mosse = new List<Mossa>
            {
                new Mossa
                {
                    Nome = "Fendente",
                    PotenzaBase = 10,
                    PrecisioneBase = 100,
                    ProbabilitaCritico = 10
                }
            },
            Inventario = new List<OggettoInventario>
            {
                new OggettoInventario
                {
                    Oggetto = new Oggetto
                    {
                        Nome = "Bastone",
                        Categoria = CategoriaOggetto.Arma,
                    },
                    Quantita = 1
                },
                new OggettoInventario
                {
                    Oggetto = new Oggetto
                    {
                        Nome = "Birra",
                        Categoria = CategoriaOggetto.Consumabile,
                    },
                    Quantita = 1
                }
            }
        };

        // Equipaggia automaticamente la mossa nello slot 0 e l'arma
        personaggio.Equipaggiamenti.MosseEquipaggiate[0] = personaggio.Mosse[0];
        ServizioOggetti.EquipaggiaOggettoDaNome(personaggio, "Bastone");

        SchermataBenvenuto(personaggio);
        var servizioClassi = new ServizioClassi();

        // 3) SCELTA CLASSE
        bool deciso = true;
        while (deciso)
        {
            Console.Clear();
            ServizioIncontri.DisegnaGiocatore(personaggio);

            string titolo = "Classi disponibili:";
            int centerY = Console.WindowHeight / 2 - (servizioClassi.ClassiDisponibili.Count + 3) / 2;

            // Titolo centrato
            int tx = Console.WindowWidth / 2 - titolo.Length / 2;
            Console.SetCursorPosition(tx, centerY);
            Console.WriteLine(titolo);

            // Lista classi centrata
            for (int i = 0; i < servizioClassi.ClassiDisponibili.Count; i++)
            {
                string riga = $"{i + 1}) {servizioClassi.ClassiDisponibili[i].Nome}";
                int x = Console.WindowWidth / 2 - riga.Length / 2;
                Console.SetCursorPosition(x, centerY + 2 + i);
                Console.WriteLine(riga);
            }

            // Prompt
            string prompt = "Seleziona una classe ";
            int px = Console.WindowWidth / 2 - prompt.Length / 2;
            Console.SetCursorPosition(px, centerY + servizioClassi.ClassiDisponibili.Count + 4);
            Console.Write(prompt);

            char sceltaClasseChar = Console.ReadKey(true).KeyChar;

            if (int.TryParse(sceltaClasseChar.ToString(), out int sceltaClasseInt))
            {
                if (sceltaClasseInt < 1 || sceltaClasseInt > servizioClassi.ClassiDisponibili.Count)
                    continue;

                while (true)
                {
                    Console.Clear();
                    ServizioIncontri.DisegnaGiocatore(personaggio);

                    var classeSelezionata = servizioClassi.ClassiDisponibili[sceltaClasseInt - 1];

                    string nomeClasse = classeSelezionata.Nome;
                    string stats = $"HP {classeSelezionata.Salute} | ATK {classeSelezionata.Attacco} | DEF {classeSelezionata.Difesa} | VEL {classeSelezionata.Velocita}";
                    string conferma = "[E] Conferma   [Q] Indietro";

                    int cy = Console.WindowHeight / 2;

                    // Nome classe
                    int nx = Console.WindowWidth / 2 - nomeClasse.Length / 2;
                    Console.SetCursorPosition(nx, cy - 2);
                    Console.WriteLine(nomeClasse);

                    // Statistiche
                    int sx = Console.WindowWidth / 2 - stats.Length / 2;
                    Console.SetCursorPosition(sx, cy);
                    Console.WriteLine(stats);

                    // Prompt conferma
                    int cx = Console.WindowWidth / 2 - conferma.Length / 2;
                    Console.SetCursorPosition(cx, cy + 2);
                    Console.Write(conferma);

                    char decisione = Console.ReadKey(true).KeyChar;

                    if (decisione == 'e' || decisione == 'E')
                    {
                        servizioClassi.ApplicaClasse(personaggio, classeSelezionata);
                        Console.Clear();
                        MostraMessaggioConUI(personaggio, $"{personaggio.Nome}, hai scelto: {classeSelezionata.Nome}");
                        deciso = false;
                        break;
                    }
                    else if (decisione == 'q' || decisione == 'Q')
                        break;
                }
            }
        }

        // CREA LO STATO DI GIOCO
        return new StatoGioco
        {
            Personaggio = personaggio,
            AreaCorrente = sezione.Nome,
            PosizioneX = personaggio.PosX,
            PosizioneY = personaggio.PosY
        };
    }

    static void SchermataBenvenuto(Personaggio personaggio)
    {
        Console.Clear();
        ServizioIncontri.DisegnaGiocatore(personaggio);

        string titolo = "PROJECT FRONTIER";
        int larghezza = 50;

        string bordoTop = "╔" + new string('═', larghezza) + "╗";
        string bordoMid = "╠" + new string('═', larghezza) + "╣";
        string bordoBottom = "╚" + new string('═', larghezza) + "╝";

        List<string> righe = new()
        {
            bordoTop,
            "║" + titolo.PadLeft((larghezza + titolo.Length) / 2).PadRight(larghezza) + "║",
            bordoMid,
            $"║ Benvenuto, {personaggio.Nome}!{"".PadRight(larghezza - ($" Benvenuto, {personaggio.Nome}!").Length)}║",
            $"║ Il tuo viaggio sta per iniziare...".PadRight(larghezza + 1) + "║",
            $"║".PadRight(larghezza + 1) + "║",
            $"║ • Esplora terre misteriose".PadRight(larghezza + 1) + "║",
            $"║ • Affronta creature sconosciute".PadRight(larghezza + 1) + "║",
            $"║ • Cresci, combatti, sopravvivi".PadRight(larghezza + 1) + "║",
            $"║".PadRight(larghezza + 1) + "║",
            $"║ Preparati, avventuriero...".PadRight(larghezza + 1) + "║",
            bordoBottom
        };
        int startY = Console.WindowHeight / 2 - righe.Count / 2;

        for (int i = 0; i < righe.Count; i++)
        {
            string riga = righe[i];
            int x = Console.WindowWidth / 2 - (riga.Length / 2);
            Console.SetCursorPosition(x, startY + i);
            Console.Write(riga);
        }
        TastoAvanti();
    }

    // ============================
    //       CARICAMENTO PARTITA
    // ============================
    static StatoGioco MenuCaricamento(ServizioSalvataggio salvataggio)
    {
        var files = salvataggio.ElencaSalvataggi();
        if (files.Count == 0)
        {
            MostraMessaggioCentratoWriteLine("Nessun salvataggio trovato.");
            TastoIndietro();
            return null;
        }

        while (true)
        {
            Console.Clear();
            List<string> righe = new();
            righe.Add("===== SALVATAGGI =====");
            righe.Add("");

            for (int i = 0; i < files.Count; i++)
                righe.Add($"[{i + 1}] {Path.GetFileName(files[i])}");

            int startY = Console.WindowHeight / 2 - righe.Count / 2;

            for (int i = 0; i < righe.Count; i++)
            {
                string riga = righe[i];
                int x = Console.WindowWidth / 2 - riga.Length / 2;
                Console.SetCursorPosition(x, startY + i);
                Console.Write(riga);
            }
            StampaTasto("[Q] Indietro");
            char scelta = Console.ReadKey(true).KeyChar;

            if (scelta == 'q' || scelta == 'Q')
                return null;

            if (int.TryParse(scelta.ToString(), out int fileScelto))
            {
                if (fileScelto >= 1 && fileScelto <= files.Count)
                {
                    Console.Clear();
                    return salvataggio.Carica(files[fileScelto - 1]);
                }
            }
        }
    }

    // ============================
    //       AVVIO DEL GIOCO
    // ============================
    static void AvviaGioco(StatoGioco stato)
    {
        var personaggio = stato.Personaggio;
        // 🔹 Carico la mappa dal file
        var mappa = ServizioMappa.CaricaMappa("Mappa_principale.json");
        // 🔹 Carico le mosse
        ServizioMosse.CaricaMosse("Mosse.json");
        // 🔹 Carico gli oggetti
        ServizioOggetti.CaricaOggettiPerCategoria("Oggetti.json");
        ServizioOggetti.CaricaTuttiOggetti();
        // 🔹 Carico i nemici
        var configNemici = ServizioNemici.CaricaNemici("Nemici.json");
        // 🔹 Assegno le mosse e gli oggetti ai nemici
        ServizioNemici.AssegnaMosse(configNemici);
        ServizioNemici.AssegnaOggetti(configNemici);
        // 🔹 Trovo la sezione corretta
        var sezione = mappa.Sezioni.First(s => s.Nome == stato.AreaCorrente);
        // 🔹 Creo la griglia per il movimento
        var griglia = ServizioMappa.CreaGriglia(sezione);
        var movimento = new ServizioMovimento(griglia);
        // 🔹 Posizione iniziale
        var cellaIniziale = griglia[personaggio.PosY, personaggio.PosX];

        Console.Clear();
        DisegnaMiniMappa(griglia, personaggio.PosX, personaggio.PosY);

        string[] righe =
        {
            $"Ti trovi nella {cellaIniziale!.Nome}.",
            $"{cellaIniziale.Descrizione}"
        };
        CicloForPerStampaCentrale(righe);

        while (true)
        {
            ServizioIncontri.DisegnaGiocatore(personaggio);
            string testo = "[R] Menù  [W] Su  [S] Giù  [A] Sinistra  [D] Destra";

            int posX = Console.WindowWidth - testo.Length - 2; // margine di 2
            int posY = Console.WindowHeight - 2; // penultima riga

            Console.SetCursorPosition(posX, posY);
            Console.Write(testo);

            char direzione = Console.ReadKey(true).KeyChar;
            Console.Clear();
            if (direzione == 'r' || direzione == 'R')
            {
                bool continua = Menu(personaggio, stato, griglia);
                if (!continua) return;
            }
            var risultato = movimento.Muovi(personaggio, direzione, sezione, configNemici);

            // ⭐ CAMBIO SEZIONE SE SERVE
            if (risultato.Collegamento != null)
            {
                var areaCollegata = risultato.Collegamento;
                stato.AreaCorrente = areaCollegata.Sezione;
                personaggio.PosX = areaCollegata.X;
                personaggio.PosY = areaCollegata.Y;

                sezione = mappa.Sezioni.First(s => s.Nome == areaCollegata.Sezione);
                griglia = ServizioMappa.CreaGriglia(sezione);
                movimento = new ServizioMovimento(griglia);

                Console.Clear();
                DisegnaMiniMappa(griglia, personaggio.PosX, personaggio.PosY);

                string[] righeCambio =
                {
                    $"Sei entrato in {areaCollegata.Sezione}.",
                    $"Una nuova area da esplorare."
                };
                CicloForPerStampaCentrale(righeCambio);
                continue;
            }

            DisegnaMiniMappa(griglia, personaggio.PosX, personaggio.PosY);
            string[] righeRisultato =
            {
                $"{risultato.Messaggio1}.",
                $"{risultato.Descrizione}"
            };
            CicloForPerStampaCentrale(righeRisultato);

            if (risultato.NemicoTrovato != null)
            {
                var esito = ServizioIncontri.Incontro(personaggio, risultato.NemicoTrovato);
                if (risultato.NemicoTrovato is Nemico nemico)
                {
                    if (esito == EsitoIncontro.Vittoria)
                        ServizioDrop.ApplicaDrop(personaggio, nemico, esito);

                    DisegnaMiniMappa(griglia, personaggio.PosX, personaggio.PosY);
                    CicloForPerStampaCentrale(righeRisultato);
                }

                if (esito == EsitoIncontro.Sconfitta)
                {
                    /* Opzione A: ritorni al checkpoint
                    personaggio.PosX = stato.CheckpointX;
                    personaggio.PosY = stato.CheckpointY;
                    */
                    personaggio.SaluteAttuale = personaggio.SaluteMassima;

                    // Opzione B: torni al menu principale
                    // return;

                    continue;
                }
            }
        }
    }

    // ============================
    //       MENU DI GIOCO
    // ============================
    static bool Menu(Personaggio personaggio, StatoGioco stato, Cella?[,] griglia)
    {
        while (true)
        {
            Console.Clear();
            if (personaggio.Inventario.Count == 0)
            {
                MostraMessaggioCentratoWriteLine("L'inventario è vuoto");
                TastoIndietro();
                continue;
            }
            string[] righeMenu =
            {
                "===== MENÙ =====",
                "",
                "[1] Inventario   Quest [5]",
                "[2] Statistiche   Salva [6]",
                "[3] Mosse   Punti abilità [7]",
                "[4] Mappa   Esci [8]"
            };
            CicloForPerStampaCentrale(righeMenu);
            StampaTasto("[Q] Indietro");
            char scelta = Console.ReadKey(true).KeyChar;
            Console.Clear();

            switch (scelta)
            {
                case '1':
                    if (personaggio.Inventario.Count == 0)
                    {
                        MostraMessaggioCentratoWriteLine("L'inventario è vuoto.");
                        TastoIndietro();
                        continue;
                    }
                    while (true)
                    {
                        string[] righe =
                        {
                            "======= INVENTARIO =======",
                            "",
                            "[1] Consumabili    Elmi [5]",
                            "[2] Offensivi    Corazze [6]",
                            "[3] Materiali    Gambali [7]",
                            "[4] Armi    Stivali [8]"
                        };
                        CicloForPerStampaCentrale(righe);
                        StampaTasto("[Q] Indietro");
                        char sceltaInventario = Console.ReadKey(true).KeyChar;
                        Console.Clear();

                        switch (sceltaInventario)
                        {
                            case '1':
                                MostraConsumabili(personaggio);
                                continue;
                            case '2':
                                continue;
                            case '3':
                                continue;
                            case '4':
                                continue;
                            case '5':
                                continue;
                            case '6':
                                continue;
                            case '7':
                                continue;
                            case '8':
                                continue;
                            case 'Q': break;
                            case 'q': break;
                            default:
                                continue;
                        }
                        break;
                    }

                    continue;
                case '2':
                    MostraStatistiche(personaggio, griglia);
                    continue;
                case '3':
                    if (personaggio.Mosse.Count == 0)
                    {
                        MostraMessaggioCentratoWriteLine("Non hai mosse disponibili");
                        TastoIndietro();
                        continue;
                    }
                    foreach (var mossa in personaggio.Mosse)
                    {
                        int i = 1;
                        string[] righeMossa =
                        {
                            "===== MOSSE =====",
                            $"[{i}] {mossa.Nome}",
                        };
                        i++;
                        CicloForPerStampaCentrale(righeMossa);
                    }
                    TastoIndietro();
                    continue;
                case '4':
                case '5':
                    Console.WriteLine("DA FARE");
                    TastoIndietro();
                    continue;
                case '6':
                    var servizioSalvataggio = new ServizioSalvataggio();
                    string percorso = servizioSalvataggio.SalvaNuovoSlot(stato);
                    Console.WriteLine("Partita salvata con successo!");
                    Console.WriteLine($"File: {Path.GetFileName(percorso)}");
                    Console.WriteLine($"Salvato il: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    TastoIndietro();
                    continue;
                case '7': 
                case '8': return false;
                case 'Q': return true;
                case 'q': return true;
            }
        }
    }

    static void MostraStatistiche(Personaggio personaggio, Cella?[,] griglia)
    {
        DisegnaMiniMappa(griglia, personaggio.PosX, personaggio.PosY);
        ServizioIncontri.DisegnaGiocatore(personaggio);

        // -------------------------
        // 1) Calcolo dimensioni box
        // -------------------------
        int larghezza = 30;
        string titolo = $"PERSONAGGIO: {personaggio.Nome}";
        string BordoTop = "╔" + new string('═', larghezza) + "╗";
        string BordoMid = "╠" + new string('═', larghezza) + "╣";
        string BordoBottom = "╚" + new string('═', larghezza) + "╝";

        int spazi = (larghezza - titolo.Length) / 2;
        string RigaTitolo = "║" + new string(' ', spazi) + titolo + new string(' ', larghezza - titolo.Length - spazi) + "║";

        // -------------------------
        // 2) Calcolo posizione centrata SOLO per il box
        // -------------------------
        int startX = Console.WindowWidth / 2 - (larghezza + 2) / 2;
        int startY = Console.WindowHeight / 2 - 10;
        // ↑ 10 è un offset verticale per centrarlo "a occhio"
        //   puoi aumentare o diminuire questo valore

        void WriteAt(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }

        // -------------------------
        // 3) Stampa box centrato
        // -------------------------
        int r = startY;

        WriteAt(startX, r++, BordoTop);
        WriteAt(startX, r++, RigaTitolo);
        WriteAt(startX, r++, BordoMid);

        string salute = $"{personaggio.SaluteAttuale}/{personaggio.SaluteMassima}";

        void Stat(string nome, string valore)
        {
            WriteAt(startX, r++, $"║ {nome,-18}{valore,-11}║");
        }

        Stat("Salute", salute);
        Stat("Attacco", personaggio.Attacco.ToString());
        Stat("Difesa", personaggio.Difesa.ToString());
        Stat("Velocità", personaggio.Velocita.ToString());
        Stat("Esperienza", personaggio.Esperienza.ToString());
        Stat("Livello", personaggio.Livello.ToString());
        Stat("Monete", personaggio.Monete.ToString());
        Stat("Punti abilità", personaggio.PuntiAbilita.ToString());

        if (personaggio.Equipaggiamenti.Arma != null)
            Stat("Arma", personaggio.Equipaggiamenti.Arma.Oggetto.Nome);
        if (personaggio.Equipaggiamenti.Elmo != null)
            Stat("Elmo", personaggio.Equipaggiamenti.Elmo.Oggetto.Nome);
        if (personaggio.Equipaggiamenti.Corazza != null)
            Stat("Corazza", personaggio.Equipaggiamenti.Corazza.Oggetto.Nome);
        if (personaggio.Equipaggiamenti.Gambali != null)
            Stat("Gambali", personaggio.Equipaggiamenti.Gambali.Oggetto.Nome);
        if (personaggio.Equipaggiamenti.Stivali != null)
            Stat("Stivali", personaggio.Equipaggiamenti.Stivali.Oggetto.Nome);

        WriteAt(startX, r++, BordoBottom);
        TastoIndietro();
    }

    // ------------------------------
    //  UI DINAMICA
    // ------------------------------
    public static void DisegnaMiniMappa(Cella?[,] griglia, int px, int py)
    {
        const int raggio = 2; // 5x5

        for (int dy = -raggio; dy <= raggio; dy++)
        {
            for (int dx = -raggio; dx <= raggio; dx++)
            {
                int x = px + dx;
                int y = py + dy;

                Console.SetCursorPosition(0 + (dx + raggio) * 2, 0 + (dy + raggio));

                if (x == px && y == py)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("● ");
                    Console.ResetColor();
                }
                else if (y >= 0 && y < griglia.GetLength(0) &&
                         x >= 0 && x < griglia.GetLength(1) &&
                         griglia[y, x] != null)
                {
                    Console.Write("■ ");
                }
                else
                {
                    Console.Write("□ ");
                }
            }
            Console.WriteLine();
        }
    }

    static void MostraMessaggioConUI(Personaggio personaggio, string messaggio)
    {
        // 1) Ridisegna tutta la UI
        Console.Clear();
        ServizioIncontri.DisegnaGiocatore(personaggio);

        // 2) Scrivi il messaggio al centro
        int centerX = Console.WindowWidth / 2 - messaggio.Length / 2;
        int centerY = Console.WindowHeight / 2;

        Console.SetCursorPosition(centerX, centerY);
        Console.Write(messaggio);
        TastoAvanti();

        // 3) Ridisegna la UI normale
        Console.Clear();
        ServizioIncontri.DisegnaGiocatore(personaggio);
    }

    public static void MostraMessaggioCentratoWrite(string messaggio)
    {
        int x = Console.WindowWidth / 2 - messaggio.Length / 2;
        int y = Console.WindowHeight / 2;

        Console.SetCursorPosition(x, y);
        Console.Write(messaggio);
    }

    public static void MostraMessaggioCentratoWriteLine(string messaggio)
    {
        int x = Console.WindowWidth / 2 - messaggio.Length / 2;
        int y = Console.CursorTop;

        Console.SetCursorPosition(x, y);
        Console.WriteLine(messaggio);
    }

    public static void CicloForPerStampaCentrale(string[] righe)
    {
        int Y = Console.WindowHeight / 2 - righe.Length / 2;
        for (int i = 0; i < righe.Length; i++)
        {
            string riga = righe[i];
            int X = Console.WindowWidth / 2 - riga.Length / 2;

            Console.SetCursorPosition(X, Y + i);
            Console.Write(riga);
        }
    }

    static void MostraConsumabili(Personaggio personaggio)
    {
        var consumabili = personaggio.Inventario
            .Where(i => i.Oggetto.Categoria == CategoriaOggetto.Consumabile)
            .ToList();

        int pagina = 0;
        const int perPagina = 5;

        while (true)
        {
            Console.Clear();
            MostraMessaggioCentratoWriteLine("======= CONSUMABILI =======");
            Console.WriteLine();

            int start = pagina * perPagina;
            var paginaCorrente = consumabili
                .Skip(start)
                .Take(perPagina)
                .ToList();

            if (paginaCorrente.Count == 0)
                MostraMessaggioCentratoWriteLine("Nessun consumabile in questa pagina.");
            else
            {
                for (int i = 0; i < paginaCorrente.Count; i++)
                {
                    var item = paginaCorrente[i];
                    int numero = i + 1;

                    string riga = $"[{numero}] X{item.Quantita} {item.Oggetto.Nome}";
                    MostraMessaggioCentratoWriteLine(riga);
                }
            }

            // Paginazione centrata
            StampaTasto("[F] Precedenti   [E] Prossimi   [Q] Indietro");
            char scelta = Console.ReadKey(true).KeyChar;

            switch (scelta)
            {
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                    int index = scelta - '1';
                    if (index < paginaCorrente.Count)
                        MostraDettagliOggetto(paginaCorrente[index]);
                    break;

                case 'E':
                case 'e':
                    if ((pagina + 1) * perPagina < consumabili.Count)
                        pagina++;
                    break;

                case 'F':
                case 'f':
                    if (pagina > 0)
                        pagina--;
                    break;

                case 'Q':
                case 'q':
                    return;

                default:
                    continue;
            }
        }
    }

    static void MostraDettagliOggetto(OggettoInventario item)
    {
        Console.Clear();
        MostraMessaggioCentratoWriteLine("===== DETTAGLI OGGETTO =====");
        Console.WriteLine();
        MostraMessaggioCentratoWriteLine($"X{item.Quantita} {item.Oggetto.Nome}");
        MostraMessaggioCentratoWriteLine($"Categoria: {item.Oggetto.Categoria}");
        MostraMessaggioCentratoWriteLine($"Bonus Attacco: {item.Oggetto.BonusAttacco}");
        MostraMessaggioCentratoWriteLine($"Bonus Difesa: {item.Oggetto.BonusDifesa}");
        MostraMessaggioCentratoWriteLine($"Bonus Velocità: {item.Oggetto.BonusVelocita}");
        MostraMessaggioCentratoWriteLine($"Bonus Salute: {item.Oggetto.BonusSalute}");
        TastoIndietro();
    }

    // ============================
    //       UTILITY
    // ============================
    public static void StampaTasto(string messaggio)
    {
        int posX = Console.WindowWidth - messaggio.Length - 2;
        int posY = Console.WindowHeight - 2;

        Console.SetCursorPosition(posX, posY);
        Console.Write(messaggio);
    }
    static bool TastiSiENo()
    {
        string testo = "[E] Si  [Q] No";

        int posX = Console.WindowWidth - testo.Length - 2;
        int posY = Console.WindowHeight - 2;

        Console.SetCursorPosition(posX, posY);
        Console.Write(testo);

        while (true)
        {
            char t = Console.ReadKey(true).KeyChar;
            if (t == 'E' || t == 'e')
                return true;
            if (t == 'Q' || t == 'q')
                return false;
        }
    }
    static void TastoIndietro()
    {
        string testo = "[Q] Indietro";

        int posX = Console.WindowWidth - testo.Length - 2;
        int posY = Console.WindowHeight - 2;

        Console.SetCursorPosition(posX, posY);
        Console.Write(testo);

        while (true)
        {
            char e = Console.ReadKey(true).KeyChar;
            if (e == 'Q' || e == 'q') break;
        }
    }
    public static void TastoAvanti()
    {
        string testo = "[E] Avanti";

        int posX = Console.WindowWidth - testo.Length - 2; // margine di 2
        int posY = Console.WindowHeight - 2; // penultima riga

        Console.SetCursorPosition(posX, posY);
        Console.Write(testo);

        while (true)
        {
            char e = Console.ReadKey(true).KeyChar;
            if (e == 'E' || e == 'e') break;
        }
    }
}